using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Presentation.Core.Interfaces;

namespace Presentation.Core
{
    /// <summary>
    /// The ExtendedNotifyPropertyChanged extends the bare bones
    /// NotifyPropertyChanged implementation with extra capabilities
    /// such as busy flag etc. This does not include change tracking
    /// or rules or any of the attributes that the ViewModel can support
    /// </summary>
    public class ExtendedNotifyPropertyChanged : NotifyPropertyChanged,
        ISupportInitializeNotification, ISupportUpdate, IExtendedDataErrorInfo
    {
        /// <summary>
        /// Acts as a counter to ensure a single EndInit will not 
        /// turn of initialization if BeginInit was called multiple
        /// times
        /// </summary>
        private int _initializeCounter;
        /// <summary>
        /// Acts as a counter to ensure a single EndUpdate will not 
        /// turn of updates if BeginUpdate was called multiple
        /// times
        /// </summary>
        private int _updateCounter;
        /// <summary>
        /// Used to track the busy flag, allows IsBusy = True to be called
        /// from multiple places and requires the same number of IsBusy = false
        /// to turn it off.
        /// </summary>
        private int _busyCounter;
        /// <summary>
        /// Used to track property changes during suspension
        /// of property changes (i.e. during an BeginUpdate/EndUpdate)
        /// </summary>
        protected PropertyRecorder _deferredPropertyChanges;

        // ReSharper disable once InconsistentNaming
        private event EventHandler _initialized;

        /// <summary>
        /// Used to track error information
        /// </summary>
        private readonly Lazy<IExtendedDataErrorInfo> _dataErrorInfo;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected ExtendedNotifyPropertyChanged()
        {
            _initializeCounter = 0;
            _updateCounter = 0;
            _busyCounter = 0;
            _dataErrorInfo = new Lazy<IExtendedDataErrorInfo>(() => new DataErrorInfo());
        }

        protected bool ChangeNotificationsSuspended => IsInitializing || IsUpdating;
        protected bool ChangeNotificationsDeferred => IsUpdating;
        protected bool IsInitializing => _initializeCounter > 0;
        protected bool IsInitialized => _initializeCounter <= 0;
        protected bool IsUpdating => _updateCounter > 0;
        /// <summary>
        /// Gets whether the view model is in updating mode.
        /// To minimize what the end user see's this is an explicit
        /// implementation calling a private implementation
        /// </summary>
        bool ISupportUpdate.IsUpdating => IsUpdating;
        /// <summary>
        /// Gets whether the view model is initialized.
        /// To minimize what the end user see's this is an explicit
        /// implementation calling a private implementation
        /// </summary>
        bool ISupportInitializeNotification.IsInitialized => IsInitialized;
        /// <summary>
        /// Turns on initialization mode, this will turn
        /// changing/change events etc. and  set the view 
        /// model properties to initialized values. Also
        /// disables change tracking.
        /// </summary>
        public void BeginInit()
        {
            _initializeCounter++;
        }

        /// <summary>
        /// Switches the property store back out of initialization mode,
        /// renables change tracking etc.
        /// 
        /// Note: This does not fire changed events after initialization.
        /// </summary>
        public void EndInit()
        {
            if (_initializeCounter > 0 && --_initializeCounter <= 0)
            {
                var initialized = _initialized;
                initialized?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Switches the property store to update mode so changing/change 
        /// events are disabled to stop "event chatter", whilst changes 
        /// are made.
        /// </summary>
        public void BeginUpdate()
        {
            _updateCounter++;
        }

        /// <summary>
        /// Switches the property store back from update mode, so renables
        /// changing/change events and will cause a update of properties
        /// to signal to the UI to update.
        /// </summary>
        public void EndUpdate()
        {
            if (_updateCounter > 0 && --_updateCounter <= 0)
            {
                // replay deferred properties
                var playback = _deferredPropertyChanges?.Playback();
                if (playback != null)
                {
                    foreach (var property in playback)
                    {
                        // ReSharper disable once ExplicitCallerInfoArgument
                        base.OnPropertyChanged(property);
                    }
                }
                _deferredPropertyChanges = null;
            }
        }

        event EventHandler ISupportInitializeNotification.Initialized
        {
            add { _initialized += value; }
            remove { _initialized -= value; }
        }

        string IDataErrorInfo.this[string columnName] => _dataErrorInfo.IsValueCreated ? _dataErrorInfo.Value?[columnName] : null;
        string IDataErrorInfo.Error => _dataErrorInfo.IsValueCreated ? _dataErrorInfo.Value?.Error : null;

        string[] IExtendedDataErrorInfo.Properties => _dataErrorInfo.IsValueCreated ? _dataErrorInfo.Value.Properties : null;
        string[] IExtendedDataErrorInfo.Errors => _dataErrorInfo.IsValueCreated ? _dataErrorInfo.Value.Errors : null;

        void IExtendedDataErrorInfo.SetError(string errorMessage)
        {
            _dataErrorInfo.Value.SetError(errorMessage);
        }

        bool IExtendedDataErrorInfo.Add(string propertyName, string message)
        {
            return _dataErrorInfo.Value.Add(propertyName, message);
        }

        bool IExtendedDataErrorInfo.Remove(string propertyName)
        {
            return !_dataErrorInfo.IsValueCreated || _dataErrorInfo.Value.Remove(propertyName);
        }

        bool IExtendedDataErrorInfo.Clear()
        {
            return !_dataErrorInfo.IsValueCreated || _dataErrorInfo.Value.Clear();
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            if (_dataErrorInfo.IsValueCreated)
            {
                _dataErrorInfo.Value.GetErrors(propertyName);
            }
            return null;
        }

        bool INotifyDataErrorInfo.HasErrors => _dataErrorInfo.IsValueCreated && _dataErrorInfo.Value.HasErrors;

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add { _dataErrorInfo.Value.ErrorsChanged += value; }
            remove { _dataErrorInfo.Value.ErrorsChanged -= value; }
        }

        /// <summary>
        /// Sets the supplied error against the property in the 
        /// data error info. if error is null, assume we should 
        /// clear the error.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="error">The error message</param>
        /// <param name="raiseChangeEvent">Set to true to raise property change events or false (default)</param>
        protected virtual void SetError(string propertyName, string error, bool raiseChangeEvent = false)
        {
            if (!String.IsNullOrEmpty(error))
            {
                ((IExtendedDataErrorInfo)this).Add(propertyName, error);
            }
            else
            {
                ((IExtendedDataErrorInfo)this).Remove(propertyName);
            }

            if (raiseChangeEvent)
            {
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(propertyName);
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
        protected virtual void OnValidate<T>(T value, string propertyName, Func<T, ValidationResult> validationFunc)
        {
            SetError(propertyName, null);

            var validationResult = validationFunc?.Invoke(value);
            if (validationResult != null)
            {
                SetError(propertyName, !String.IsNullOrEmpty(validationResult.ErrorMessage) ?
                    validationResult.ErrorMessage :
                    $"{propertyName} : {value} validation failure"
                    );
            }
        }

        /// <summary>
        /// Validates the whole of the view model. If a 
        /// property hasn't been validated as part of a
        /// SetProperty call, this method should be
        /// called to ensure all properties are valid.
        /// </summary>
        /// <returns>An array of ValidationResults or null if no errors exist</returns>
        public ValidationResult[] Validate()
        {
            var errors = new List<ValidationResult>();
            if (!Validator.TryValidateObject(this, new ValidationContext(this, null, null), errors, true))
            {
                foreach (var error in errors)
                {
                    foreach (var property in error.MemberNames)
                    {
                        SetError(property, error.ErrorMessage, true);
                    }
                }
                return errors.ToArray();
            }
            return null;
        }

        /// <summary>
        /// A utility flag for alerting the view that the
        /// view model is busy. Multiple IsBusy = true 
        /// may be set and the corresponding IsBusy = false
        /// must be called before the flag switches back to 
        /// false.
        /// </summary>
        public bool IsBusy
        {
            get { return _busyCounter > 0; }
            set
            {
                var previous = _busyCounter;
                _busyCounter = Math.Max(0, value ? _busyCounter + 1 : _busyCounter - 1);
                if (previous != _busyCounter)
                {
                    OnPropertyChanged();
                }
            }
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
            }
            return result;
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
        }
    }
}