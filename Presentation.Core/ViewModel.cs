using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Core
{
    /// <summary>
    /// The base class for view model's implemented in the MVVM pattern. 
    /// </summary>
    public abstract class ViewModel : NotifyPropertyChanged,
        IViewModel, ISupportInitializeNotification, ISupportUpdate,
        IDataErrorInfo, IRevertibleChangeTracking, IDisposable
#if !NET4
        , INotifyDataErrorInfo
#endif
    {
#if !NET4
        private event EventHandler<DataErrorsChangedEventArgs> errorsChanged;
#endif
        private event EventHandler _initialized;

        private readonly ReferenceCounter _busyCount = new ReferenceCounter();
        private readonly ReferenceCounter _updateCount = new ReferenceCounter();

        private bool _initializing;
        private bool _isChanged;

        protected bool disposed;

        protected readonly IBackingStore BackingStore;

        protected ViewModel()
        {
        }

        protected ViewModel(IBackingStore backingStore)
        {
            this.BackingStore = backingStore;
        }

        protected ViewModel(IExtendedDataErrorInfo dataErrorInfo)
        {
            DataErrorInfo = dataErrorInfo;
        }

        protected ViewModel(IBackingStore backingStore, IExtendedDataErrorInfo dataErrorInfo) :
            this(backingStore)
        {
            DataErrorInfo = dataErrorInfo;
        }

        ~ViewModel()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // dispose of any managed resources here
                }
                // clean up any unmanaged resources here
            }
            disposed = true;
        }

        public IValidateViewModel Validation { get; set; }
        public IExtendedDataErrorInfo DataErrorInfo { get; set; }

        /// <summary>
        /// Gets/sets the rules to be associated with the view model
        /// </summary>
        public Rules Rules { get; set; }

        /// <summary>
        /// Called prior to a property changing, return false in a subclass 
        /// to stop the property change happening. This overload is called 
        /// with the current and new values for a subclass to view if required.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentValue"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
#if !NET4
        protected virtual bool OnPropertyChanging<T>(T currentValue, T newValue, [CallerMemberName] string propertyName = null)
#else
        protected virtual bool OnPropertyChanging<T>(T currentValue, T newValue, string propertyName)
#endif
        {
            return OnPropertyChanging(propertyName);
        }

        protected override bool OnPropertyChanging(string propertyName = null)
        {
            bool result = true;
            if (!_initializing && _updateCount.Count <= 0)
            {
                result = base.OnPropertyChanging(propertyName);
            }
            return result;
        }

        /// <summary>
        /// Can be overriden by a subclass to allow post property changing 
        /// functionality, such as validation to take place
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="previousValue"></param>
        /// <param name="currentValue"></param>
        /// <param name="propertyName"></param>
#if !NET4
        protected virtual void OnPropertyChanged<T>(T previousValue, T currentValue, [CallerMemberName] string propertyName = null)
#else
        protected virtual async void OnPropertyChanged<T>(T previousValue, T currentValue, string propertyName)
#endif
        {
            await OnValidate(currentValue, propertyName);
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// When a property has changed this method is called. Can be
        /// overridden to supply more/alternate capabilities
        /// </summary>
        /// <param name="propertyName"></param>
#if !NET4
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
#else
        protected override void OnPropertyChanged(string propertyName = null)
#endif
        {
            if (!_initializing && _updateCount.Count <= 0)
            {
                base.OnPropertyChanged(propertyName);
            }
        }

        private void Changing(string propertyName = null)
        {
            if (!_initializing && _updateCount.Count <= 0)
            {
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
        }

        private void Changed(string propertyName = null)
        {
            if (!_initializing && _updateCount.Count <= 0)
            {
                // this is a little too simplistic as it assumes
                // any property change call with a propertyName is 
                // a change, but we could explicitly call this 
                // without a change taking place such as an internal property
                // also we don't want a change to _isChanged itself 
                // to flip the flag to True
                if (!String.IsNullOrEmpty(propertyName) && !IsInternalProperty(propertyName))
                {
                    IsChanged = true;
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
        }

        /// <summary>
        /// Override with logic to exclude any "internal"  properties
        /// from validation etc.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected virtual bool IsInternalProperty(string propertyName)
        {
            return propertyName == "IsChanged" || propertyName == "IsBusy";
        }

        /// <summary>
        /// Called when a new value is to be validated on a property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected virtual async Task OnValidate<T>(T newValue, string propertyName)
        {
            // no need to validation against internal properties
            if (!IsInternalProperty(propertyName))
            {
                var validation = Validation;
                if (validation != null)
                {
                    try
                    {
                        var errors = await validation.Validate(propertyName, newValue);
                        SetErrors(propertyName,
                            errors != null ? String.Join("\n", errors.Select(v => v.ErrorMessage)) : null, false);
                    }
                    catch (Exception e)
                    {
                        // if an exception occurs we'll add it as an error
                        SetErrors(propertyName, e.Message, false);
                    }
                }
            }
        }

        /// <summary>
        /// Validates the whole view model object using the supplied
        /// validation object. Data error info. is updated accordingly.
        /// </summary>
        /// <returns>True for validation success else false for failure</returns>
        public virtual async Task<bool> Validate()
        {
            var validation = Validation;
            if (validation != null)
            {
                if (DataErrorInfo != null)
                {
                    // force clear of all properties including
                    var propertiesInError = new List<string>(DataErrorInfo.Properties);

                    DataErrorInfo.Clear();

                    foreach (var p in propertiesInError)
                    {
                        OnPropertyChanged(p);
                    }
                }

                var errors = await validation.Validate();
                if (errors != null)
                {
                    foreach (var validationResult in errors)
                    {
                        foreach (var v in validationResult.MemberNames)
                        {
                            SetErrors(v, validationResult.ErrorMessage, true);
                        }
                    }
                    return errors.Length == 0;
                }
            }
            return true;
        }

        /// <summary>
        /// Sets the value to the property using supplied 
        /// backing fields.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
#if !NET4
        protected bool SetProperty<T>(ref T backingField,
            T newValue, [CallerMemberName] string propertyName = null)
#else
        protected bool SetProperty<T>(ref T backingField, T newValue, string propertyName)
#endif
        {
            if (String.IsNullOrEmpty(propertyName) || EqualityComparer<T>.Default.Equals(backingField, newValue))
            {
                return false;
            }

            if (!OnPropertyChanging(backingField, newValue, propertyName))
                return false;

            Changing(propertyName);

            backingField = newValue;

            Changed(propertyName);

            OnPropertyChanged(backingField, newValue, propertyName);
            return true;
        }

        /// <summary>
        /// Sets the value for the property using supplied getter
        /// and setter functions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
#if !NET4
        protected bool SetProperty<T>(Func<T> getter, Func<T, T> setter, T newValue,
        [CallerMemberName] string propertyName = null)
#else
        protected bool SetProperty<T>(Func<T> getter, Func<T, T> setter, T newValue,
            string propertyName)
#endif
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return false;
            }

            var currentValue = getter();

            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                return false;
            }

            if (!OnPropertyChanging(currentValue, newValue, propertyName))
                return false;

            Changing(propertyName);

            setter(newValue);

            Changed(propertyName);

            OnPropertyChanged(currentValue, newValue, propertyName);
            return true;
        }

        /// <summary>
        /// Sets the new value to the backing store if one is used.
        /// Do not use this overload unless a backing store is supplied
        /// otherwise and Exception will be raised.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
#if !NET4
        protected bool SetProperty<T>(T newValue, [CallerMemberName] string propertyName = null)
#else
        protected bool SetProperty<T>(T newValue, string propertyName)
#endif
        {
            if (BackingStore == null)
                throw new Exception("BackingStore not assigned");

            if (propertyName == null)
                return false;

            return BackingStore.Set(propertyName, newValue,
                (cv, nv, pn) =>
                {
                    var result = OnPropertyChanging(cv, nv, pn);
                    Changing(pn);
                    return result;
                },
                (cv, nv, pn) =>
                {
                    OnPropertyChanged(cv, nv, pn);
                    Changed(pn);
                });
        }

        /// <summary>
        /// Get's the property from the supplied backing store. If 
        /// no backing store is used then this method should not 
        /// be used and an exceptio will be raised.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns>The item for the property from the backing store</returns>
#if !NET4
        protected T GetProperty<T>([CallerMemberName] string propertyName = null)
#else
        protected T GetProperty<T>(string propertyName)
#endif
        {
            if (BackingStore == null)
                throw new Exception("BackingStore not assigned");

            return BackingStore.Get<T>(propertyName);
        }

        /// <summary>
        /// Allows your view model to support initialization, whereby
        /// updates to properties are seen as an initialization phase 
        /// and no property change events are raised. BeginInit turns
        /// the initialization mode on.
        /// </summary>
        public void BeginInit()
        {
            var supportInitialize = BackingStore as ISupportInitialize;
#if !NET4
            supportInitialize?.BeginInit();
#else
            if (supportInitialize != null)
            {
                supportInitialize.BeginInit();
            }
#endif

            _initializing = true;
        }

        /// <summary>
        /// Allows your view model to support initialization, whilst in 
        /// initialization, property change events are not raised. To 
        /// complete initialization call EndInit.
        /// </summary>
        public void EndInit()
        {
            if (_initializing)
            {
                _initializing = false;

                var supportInitialize = BackingStore as ISupportInitialize;
#if !NET4
                supportInitialize?.EndInit();
#else
                if (supportInitialize != null)
                {
                    supportInitialize.EndInit();
                }
#endif

                var initializedHandler = _initialized;
#if !NET4
                initializedHandler?.Invoke(this, EventArgs.Empty);
#else
                if (initializedHandler != null)
                {
                    initializedHandler(this, EventArgs.Empty);
                }
#endif
            }
        }

        /// <summary>
        /// Gets whether the view model is in initialization mode
        /// </summary>
        bool ISupportInitializeNotification.IsInitialized
        {
            get { return !_initializing; }
        }

        /// <summary>
        /// Raised when the view model completes initialization mode, called via EndInit.
        /// </summary>
        event EventHandler ISupportInitializeNotification.Initialized
        {
            add { _initialized += value; }
            remove { _initialized -= value; }
        }

        /// <summary>
        /// Allows the view model to switch to update mode, whereby property changed
        /// events are paused, potentially whilst many properties are being updated.
        /// </summary>
        public void BeginUpdate()
        {
            _updateCount.AddRef();
        }

        /// <summary>
        /// Allows the view model to leave update mode. If not further updates are
        /// expected this will raise a property changed event on all properties.
        /// </summary>
        public void EndUpdate()
        {
            if (_updateCount.Count > 0)
            {
                if (_updateCount.Release() == 0)
                {
                    // try to refresh all properties
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets/Sets a busy flag, 
        /// </summary>
        public virtual bool IsBusy
        {
            get { return _busyCount.Count > 0; }
            set
            {
                var current = _busyCount.Count;
                if ((value ? _busyCount.AddRef() : _busyCount.Release()) != current)
                    OnPropertyChanged("IsBusy");
            }
        }

        protected virtual void SetErrors(string propertyName, string error, bool raiseChange)
        {
            if (String.IsNullOrEmpty(error))
            {
                if (DataErrorInfo != null && DataErrorInfo.Remove(propertyName))
                {
#if !NET4
                    var err = errorsChanged;
                    err?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
#endif
                    if (raiseChange)
                    {
                        OnPropertyChanged(propertyName);
                    }
                }
            }
            else
            {
                if (DataErrorInfo != null && DataErrorInfo.Add(propertyName, error))
                {
#if !NET4
                    var err = errorsChanged;
                    err?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
#endif
                    if (raiseChange)
                    {
                        OnPropertyChanged(propertyName);
                    }
                }
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
#if !NET4
                return DataErrorInfo?[columnName];
#else
                return DataErrorInfo != null ? DataErrorInfo[columnName] : null;
#endif
            }
        }

        string IDataErrorInfo.Error
        {
            get
            {
#if !NET4
                return DataErrorInfo?.Error;
#else
                return DataErrorInfo != null ? DataErrorInfo.Error : null;
#endif
            }
        }

#if !NET4
        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            var error = DataErrorInfo[propertyName];
            return String.IsNullOrEmpty(error) ? null : new[] { error };
        }

        bool INotifyDataErrorInfo.HasErrors 
        {
            get{ return DataErrorInfo?.Errors?.Length > 0; }
        }

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add { errorsChanged += value; }
            remove { errorsChanged -= value; }
        }
#endif

        public bool IsChanged
        {
            get
            {
                var revertible = BackingStore as ISupportRevertibleChangeTracking;
                if (revertible != null)
                {
                    return revertible.IsChanged || _isChanged;
                }
                return _isChanged;
            }
            set { SetProperty(ref _isChanged, value, this.NameOf(x => x.IsChanged)); }
        }

        public virtual void AcceptChanges()
        {
            var revertible = BackingStore as ISupportRevertibleChangeTracking;
            if (revertible != null)
            {
                revertible.AcceptChanges(
                    pn =>
                    {
                        OnPropertyChanging(pn);
                        Changing(pn);
                    },
                    pn =>
                    {
                        OnPropertyChanged(pn);
                        Changed(pn);
                    });
            }
        }

        public virtual void RejectChanges()
        {
            var revertible = BackingStore as ISupportRevertibleChangeTracking;
            if (revertible != null)
            {
                revertible.RejectChanges(
                    pn =>
                    {
                        OnPropertyChanging(pn);
                        Changing(pn);
                    },
                    pn =>
                    {
                        OnPropertyChanged(pn);
                        Changed(pn);
                    });
            }
        }
    }
}