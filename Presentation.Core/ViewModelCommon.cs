using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Presentation.Patterns.Helpers;
using Presentation.Patterns.Interfaces;

namespace Presentation.Patterns
{
    /// <summary>
    /// Includes common functionality for more specific implementations
    /// of a ViewModel. For example, a view model with backing store and
    /// one without are identical up to the point of setting/getting
    /// properties and code around the backing field itself. This extends
    /// the ExtendedNotifyPropertyChanged class with change tracking and
    /// property attribute capabilities etc.
    /// </summary>
    public class ViewModelCommon : ExtendedNotifyPropertyChanged,
        IRevertibleChangeTracking, IDisposable
    {
        /// <summary>
        /// Tracks if the properties have changed
        /// </summary>
        private bool _isChanged;
        /// <summary>
        /// Denotes whether change tracking has been suspended or not
        /// </summary>
        private bool _isChangeTrackingSuppressed;
        /// <summary>
        /// Used to record any GetProperty calls. Used by
        /// ReadOnlyProperty to find dependencies
        /// </summary>
        protected PropertyRecorder _recordGets;

        private bool _disposed;

        private Stack<Tuple<string, object>> _changeStack;

        // a link to the "global" definition of propeties for this type
        protected readonly ViewModelRegistry.PropertyDefinitions _propertyDefinitions;

        /// <summary>
        /// Used to store the property name and an object representing
        /// it's state
        /// </summary>
        protected readonly IDictionary<string, IProperty> _properties;

        protected ViewModelCommon()
        {
            _propertyDefinitions = ViewModelRegistry.Instance.Register(GetType());
            _properties = new Dictionary<string, IProperty>(_propertyDefinitions.Count);
        }

        ~ViewModelCommon()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose of any managed resources here
                    DisposeManaged();
                }
                // clean up any unmanaged resources here
                DisposeUnmanaged();
            }
            _disposed = true;
        }

        /// <summary>
        /// Should be overriden if a derived class needs to 
        /// clear any managed resources during Dispose
        /// </summary>
        protected virtual void DisposeManaged()
        {
            ViewModelRegistry.Instance.UnRegister(GetType());
            DetachAll();
        }

        /// <summary>
        /// Should be overriden if a derived class needs to 
        /// clear any unmanaged resources during Dispose
        /// </summary>
        protected virtual void DisposeUnmanaged()
        {
        }

        /// <summary>
        /// Called when a property is about to be changed (pre change)
        /// allows cancellation of the change by the view model (if notifications 
        /// are enabled)
        /// </summary>
        /// <param name="propertyName">The property that's changing</param>
        /// <returns>True if successful, otherwise False</returns>
        protected override bool OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            var result = true;
            if (!ChangeNotificationsSuspended)
            {
                // ReSharper disable once ExplicitCallerInfoArgument
                result = base.OnPropertyChanging(propertyName);

                if (!String.IsNullOrEmpty(propertyName))
                {
                    IProperty property;
                    if (_properties.TryGetValue(propertyName, out property))
                    {
                        var propertyRules = property.Rules;
#if !NET4
                        propertyRules?.ForEach(r => r.PreInvoke(this));
#else
                        if (propertyRules != null)
                        {
                            propertyRules.ForEach(r => r.PreInvoke(this));
                        }
#endif
                    }
                }

                var rules = Rules;
#if !NET4
                rules?.PreInvoke(this, propertyName);
#else
                if (rules != null)
                {
                    rules.PreInvoke(this, propertyName);
                }
#endif
            }
            return result;
        }

        /// <summary>
        /// Gets whether the view model is in a changed state. 
        /// For example, view model properties which are changed
        /// tracked will set this to True
        /// </summary>
        public bool IsChanged
        {
            get { return _isChanged; }
            protected set
            {
                if (!IsInitializing)
                {
                    if (_isChanged != value)
                    {
                        _isChanged = value;
                        OnPropertyChanged();
                    }
                }
            }
        }


        /// <summary>
        /// Sets the supplied error against the property in the 
        /// data error info. if error is null, assume we should 
        /// clear the error.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="error">The error message</param>
        /// <param name="raiseChangeEvent">Set to true to raise property change events or false (default)</param>
        protected override void SetError(string propertyName, string error, bool raiseChangeEvent = false)
        {
            if (!String.IsNullOrEmpty(error))
            {
                ((IExtendedDataErrorInfo)this).Add(propertyName, error);
            }
            else
            {
                ((IExtendedDataErrorInfo)this).Remove(propertyName);
            }

            // do we need to trigger chained properties?
            foreach (var property in _properties)
            {
                var dependentProperty = property.Value as IDependentProperty;
                if (dependentProperty != null)
                {
                    ((IExtendedDataErrorInfo)this).Remove(property.Key);
#if !NET4
                    if (dependentProperty.DependentUpon?.Contains(propertyName) ?? false)
#else
                    if (dependentProperty.DependentUpon != null && dependentProperty.DependentUpon.Contains(propertyName))
#endif
                    {
                        var propertiesInError = ((IExtendedDataErrorInfo)this).Properties;
                        if (propertiesInError != null)
                        {
                            var union = dependentProperty.DependentUpon.Intersect(propertiesInError).ToArray();
                            if (union.Length > 0)
                            {
                                // currently we'll just set the dependent property to first dependency error
                                var firstError = ((IExtendedDataErrorInfo)this)[union.First()];
                                ((IExtendedDataErrorInfo)this).Add(property.Key, firstError);
                            }
                        }
                    }
                }
            }

            if (raiseChangeEvent)
            {
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(propertyName);
            }
        }


        /// <summary>
        /// Supports accepting of changes, thus resetting the
        /// IsChanged flag and more (depending on the implementation).
        /// </summary>
        void IChangeTracking.AcceptChanges()
        {
            IsChanged = false;
#if !NET4
            _changeStack?.Clear();
#else
            if (_changeStack != null)
            {
                _changeStack.Clear();
            }
#endif
        }

        /// <summary>
        /// Pushes a change onto the change stack
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        protected void PushChange(string propertyName, object value)
        {
            if (!IsInitializing && !_isChangeTrackingSuppressed)
            {
                if (_changeStack == null)
                {
                    _changeStack = new Stack<Tuple<string, object>>();
                }
                _changeStack.Push(new Tuple<string, object>(propertyName, value));
            }
        }

        /// <summary>
        /// Pops the last property change from the change stack
        /// and sets the property back to this value
        /// </summary>
        /// <returns>True if the change stack was successfully popped, 
        /// otherwise false which would indicate no items exist on the stack</returns>
        protected bool PopChange()
        {
            if (_changeStack != null && _changeStack.Count > 0)
            {
                // suspened change tracking whilst the pop occurs
                _isChangeTrackingSuppressed = true;

                try
                {
                    DynamicSetProperty(_changeStack.Pop());
                }
                finally
                {
                    _isChangeTrackingSuppressed = false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Undo will try to undo the last property change. 
        /// </summary>
        public bool Undo()
        {
            return PopChange();
        }

        /// <summary>
        /// Rejects changes and resets the IsChanged flag
        /// and more (depending on the implementation).
        /// </summary>
        void IRevertibleChangeTracking.RejectChanges()
        {
            try
            {
                if (_changeStack != null)
                {
                    if (_changeStack.Count > 0)
                    {
                        // play back first changes for each property
                        var changeList = _changeStack.ToList();
                        changeList.RemoveDuplicates(
                            (p1, p2) => String.Compare(p1.Item1, p2.Item1, StringComparison.InvariantCultureIgnoreCase));
                        foreach (var change in changeList)
                        {
                            DynamicSetProperty(change);
                        }

                        _changeStack.Clear();
                    }
                    _changeStack = null;
                }
            }
            finally
            {
                IsChanged = false;
            }

            // reset all changes to original values
            // the fire events for them to show the 
            // changes
        }

        /// <summary>
        /// Calls the back through the property to set the value
        /// </summary>
        /// <param name="change"></param>
        private void DynamicSetProperty(Tuple<string, object> change)
        {
            // we don't store any validation functions with the change, so a call to 
            // SetProperty directly will not validate - this might matter if the dev
            // of the view model uses functional validation
            //var property = _properties[change.Item1].GetType().GetGenericArguments()[0];
            //var m = new Func<object, string, bool>(SetProperty);
            //var mi = m.Method.GetGenericMethodDefinition().MakeGenericMethod(property);
            //mi.Invoke(this, new[] {change.Item2, change.Item1});

            // we'll simply call back into the property to set the change value back
            try
            {
                var propertyInfo = GetType().GetProperties().FirstOrDefault(p => p.Name == change.Item1);
#if !NET4
                propertyInfo?.SetMethod.Invoke(this, new[] { change.Item2 });
#else
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(this, change.Item2, null);
                }
#endif
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Validates against the supplied validation function (if one
        /// exists). If none-exists or validation function succeeds
        /// will look for data annotation validation attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <param name="validationFunc"></param>
        protected override void OnValidate<T>(T value, string propertyName, Func<T, ValidationResult> validationFunc)
        {
            base.OnValidate(value, propertyName, validationFunc);

            var definition = _propertyDefinitions[propertyName];
            // if data annotation validation exists on a property
#if !NET4
            if (definition?.HasValidationAttributes ?? false)
#else
            if(definition != null && definition.HasValidationAttributes)
#endif
            {
                var validationContext = new ValidationContext(this, null, null)
                {
                    DisplayName = definition.DisplayName ?? propertyName,
                    MemberName = propertyName
                };

                try
                {
                    var errors = new List<ValidationResult>();
                    if (!Validator.TryValidateProperty(value, validationContext, errors))
                    {
                        SetError(propertyName, String.Join("\n", errors.Select(v => v.ErrorMessage)));
                    }
                }
                catch (Exception e)
                {
                    SetError(propertyName, e.Message);
                }
            }
        }

        /// <summary>
        /// Ensure we detach all event handling from properties to 
        /// ensure no memory leaks
        /// </summary>
        private void DetachAll()
        {
            foreach (var property in _properties.Values)
            {
                property.Detach();
            }
        }

        /// <summary>
        /// Called after a property has changed (if notifications 
        /// are enabled)
        /// </summary>
        /// <param name="propertyName">The property which has changed</param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (!ChangeNotificationsSuspended)
            {
                // ReSharper disable once ExplicitCallerInfoArgument
                base.OnPropertyChanged(propertyName);

                if (!String.IsNullOrEmpty(propertyName))
                {
                    // we need to copy for enumeration in case
                    // a rule causes a late binding to a property, 
                    // hence changes the enumerator
                    var copyForEnumeration = _properties.ToArray();
                    // do we need to trigger chained properties?
                    foreach (var property in copyForEnumeration)
                    {
                        var dependentProperty = property.Value as IDependentProperty;
#if !NET4
                        if (dependentProperty?.DependentUpon?.Contains(propertyName) ?? false)
#else
                        if (dependentProperty != null && dependentProperty.DependentUpon != null &&
                            dependentProperty.DependentUpon.Contains(propertyName))
#endif
                        {
                            // ReSharper disable once ExplicitCallerInfoArgument
                            base.OnPropertyChanged(property.Key);
                        }

                        // this is a little nasty, but we need to ensure we can respond
                        // to a property change on any property then look for rules
                        if (property.Key == propertyName)
                        {
#if !NET4
                            var propertyRules = property.Value?.Rules;
                            propertyRules?.ForEach(r => r.PostInvoke(this));
#else
                            var propertyRules = property.Value != null ? property.Value.Rules : null;
                            if (propertyRules != null)
                            {
                                propertyRules.ForEach(r => r.PostInvoke(this));
                            }
#endif
                        }
                    }
                }
                var rules = Rules;
#if !NET4
                rules?.PostInvoke(this, propertyName);
#else
                if (rules != null)
                {
                    rules.PostInvoke(this, propertyName);
                }
#endif
            }
            // if suspended due to updating, defer property changes
            if (ChangeNotificationsDeferred)
            {
                if (_deferredPropertyChanges == null)
                {
                    _deferredPropertyChanges = new PropertyRecorder();
                }
                _deferredPropertyChanges.Record(propertyName);
            }
        }

        /// <summary>
        /// Called when properties support INotifyPropertyChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="propertyChangedEventArgs"></param>
        private void NotifyPropertyChangedOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            IsChanged = true;
        }

        /// <summary>
        /// Called when properties support INotifyCollectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notifyCollectionChangedEventArgs"></param>
        private void NotifyCollectionChangedOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            IsChanged = true;
        }

        /// <summary>
        /// Called when an object supporting ItemChanged events changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="propertyChangedEventArgs"></param>
        private void ItemChangedOnItemChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            IsChanged = true;
        }

        /// <summary>
        /// Attaches (where applicable) notification handlers to a collection
        /// or view model property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        protected void Attach<T>(T value)
        {
            var notifyCollectionChanged = value as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += NotifyCollectionChangedOnCollectionChanged;
            }

            var notifyPropertyChanged = value as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                notifyPropertyChanged.PropertyChanged += NotifyPropertyChangedOnPropertyChanged;
            }

            var itemChanged = value as IItemChanged;
            if (itemChanged != null)
            {
                itemChanged.ItemChanged += ItemChangedOnItemChanged;
            }
        }

        /// <summary>
        /// Detaches (where applicable) notification handlers to a collection
        /// or viewmodel property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        protected void Detach<T>(T value)
        {
            var notifyCollectionChanged = value as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged -= NotifyCollectionChangedOnCollectionChanged;
            }

            var notifyPropertyChanged = value as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                notifyPropertyChanged.PropertyChanged -= NotifyPropertyChangedOnPropertyChanged;
            }

            var itemChanged = value as IItemChanged;
            if (itemChanged != null)
            {
                itemChanged.ItemChanged -= ItemChangedOnItemChanged;
            }
        }

        /// <summary>
        /// Gets/sets the rules to be associated with the view model.
        /// The view model has rules supplied by attributes, but sometimes
        /// you need might need to have a parent create a rule for child
        /// objects, this is easier to implement via this Rules mechanism
        /// than via the attribute based system.
        /// 
        /// UnlessI can come up with a better solution
        /// </summary>
        public Rules Rules { get; set; }
    }
}
