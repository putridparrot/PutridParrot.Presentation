using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Presentation.Core
{
    /// <summary>
    /// The base class for view model's implemented in the MVVM pattern. 
    /// </summary>
    public abstract class ViewModel : NotifyPropertyChanged,
       ISupportInitializeNotification, ISupportUpdate, IDataErrorInfo
#if !NET4
        , INotifyDataErrorInfo
#endif
    {
#if !NET4
        private event EventHandler<DataErrorsChangedEventArgs> errorsChanged;
#endif
        private event EventHandler initialized;

        private bool _initializing;
        private int _updateCount;
        private int _busyCount;
        private bool _isDirty;

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

        public IValidateViewModel Validation { get; set; }
        public IExtendedDataErrorInfo DataErrorInfo { get; set; }
        public Rules Rules { get; set; }

        public bool IsDirty
        {
            get { return _isDirty; }
            set { SetProperty(ref _isDirty, value, this.NameOf(x => x.IsDirty)); }
        }

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

        protected override bool OnPropertyChanging(string propertyName)
        {
            bool result = true;
            if (!_initializing)
            {
                result = base.OnPropertyChanging(propertyName);

                var rules = Rules;
                if (rules != null)
                {
                    rules.PreInvoke(this, propertyName);
                }
            }

            return result;
        }

        /// <summary>
        /// Can be overriden by a subclass to allow post property changing 
        /// functionality, such as validation to take place
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentValue"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
#if !NET4
        protected virtual void OnPropertyChanged<T>(T currentValue, T newValue, [CallerMemberName] string propertyName = null)
#else
        protected virtual void OnPropertyChanged<T>(T currentValue, T newValue, string propertyName)
#endif
        {
            OnValidate(propertyName, newValue);
            OnPropertyChanged(propertyName);
        }

#if !NET4
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
#else
        protected override void OnPropertyChanged(string propertyName = null)
#endif
        {
            if (!_initializing)
            {
                base.OnPropertyChanged(propertyName);

                // this is a little too simplistic as it assumes
                // any property change call with a propertyName is 
                // a change, but we could explicitly call this 
                // without a change taking place - also we don't want a change
                // to isDirty to flip the flag to True
                if (!String.IsNullOrEmpty(propertyName) && propertyName != "IsDirty")
                {
                    IsDirty = true;
                }

                var rules = Rules;
                if (rules != null)
                {
                    rules.PostInvoke(this, propertyName);
                }
            }
        }

        protected virtual void OnValidate<T>(string propertyName, T newValue)
        {
            var validation = Validation;
            if (validation != null)
            {
                var errors = validation.Validate(propertyName, newValue);
                SetErrors(propertyName, errors != null ? String.Join("\n", errors.Select(v => v.ErrorMessage)) : null, false);
            }
        }

        public virtual bool Validate()
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

                var errors = validation.Validate();
                if (errors != null)
                {
                    foreach (var validationResult in errors)
                    {
                        foreach (var v in validationResult.MemberNames)
                        {
                            SetErrors(v, validationResult.ErrorMessage, true);
                        }
                    }
                }
            }
            return true;
        }


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

            backingField = newValue;

            OnPropertyChanged(backingField, newValue, propertyName);
            return true;
        }

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

            T currentValue = getter();

            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                return false;
            }

            if (!OnPropertyChanging(currentValue, newValue, propertyName))
                return false;

            setter(newValue);

            OnPropertyChanged(currentValue, newValue, propertyName);
            return true;
        }

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
                OnPropertyChanging,
                OnPropertyChanged);
        }

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
            supportInitialize?.BeginInit();

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
                supportInitialize?.EndInit();

                var initializedHandler = initialized;
                initializedHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets whether the view model is in initialization mode
        /// </summary>
        bool ISupportInitializeNotification.IsInitialized { get { return !_initializing; } }

        /// <summary>
        /// Raised when the view model completes initialization mode, called via EndInit.
        /// </summary>
        event EventHandler ISupportInitializeNotification.Initialized
        {
            add { initialized += value; }
            remove { initialized -= value; }
        }

        /// <summary>
        /// Allows the view model to switch to update mode, whereby property changed
        /// events are paused, potentially whilst many properties are being updated.
        /// </summary>
        public void BeginUpdate()
        {
            _updateCount++;
        }

        /// <summary>
        /// Allows the view model to leave update mode. If not further updates are
        /// expected this will raise a property changed event on all properties.
        /// </summary>
        public void EndUpdate()
        {
            if (_updateCount > 0)
            {
                _updateCount--;
                if (_updateCount == 0)
                {
                    // try to refresh all properties
                    OnPropertyChanged(null);
                }
            }
        }

        /// <summary>
        /// Gets/Sets a busy flag, 
        /// </summary>
        public bool IsBusy
        {
            get { return _busyCount > 0; }
            set
            {
                _busyCount = value
                    ? _busyCount + 1
                    : Math.Max(0, _busyCount - 1);
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

        string IDataErrorInfo.this[string columnName] => DataErrorInfo?[columnName];
        string IDataErrorInfo.Error => DataErrorInfo?.Error;

#if !NET4
        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            var error = DataErrorInfo[propertyName];
            return String.IsNullOrEmpty(error) ? null : new[] { error };
        }

        bool INotifyDataErrorInfo.HasErrors => DataErrorInfo?.Errors?.Length > 0;

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add { errorsChanged += value; }
            remove { errorsChanged -= value; }
        }
#endif
    }
}