using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Presentation.Patterns.Helpers;
using Presentation.Patterns.Interfaces;

namespace Presentation.Patterns
{
    /// <summary>
    /// Acts as a base class for view models, requires
    /// derived class to handle backing fields
    /// </summary>
    public class ViewModelWithoutBacking : ViewModelCommon
    {
        /// <summary>
        /// Sets the property value against the property and raises
        /// OnPropertyChanging, OnPropertyChanged etc. as required
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            return SetProperty(ref backingField, value, null, propertyName);
        }

        /// <summary>
        /// Sets the property value against the property and raises
        /// OnPropertyChanging, OnPropertyChanged etc. as required
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="validationFunc"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T backingField, T value, Func<T, ValidationResult> validationFunc, [CallerMemberName] string propertyName = null)
        {
            var property = GetOfCreateProperty(ref backingField, propertyName, () => new PropertyCommon<T>());
            if (!property.Equals(backingField, value))
            {
                // ReSharper disable once ExplicitCallerInfoArgument
                if (OnPropertyChanging(propertyName))
                {
                    var previousValue = backingField;

                    property.Detach();

                    backingField = value;

#if !NET4
                    if (!_propertyDefinitions.NonTrackableProperties?.Contains(propertyName) ?? true)
#else
                    if (_propertyDefinitions.NonTrackableProperties == null || !_propertyDefinitions.NonTrackableProperties.Contains(propertyName))
#endif
                    {
                        var tmp = backingField;
                        property.Attach(
                            () => Attach(tmp),
                            () => Detach(tmp)
                        );

                        IsChanged = true;
                        PushChange(propertyName, previousValue);
                    }

                    OnValidate(value, propertyName, validationFunc);
                    // ReSharper disable once ExplicitCallerInfoArgument
                    OnPropertyChanged(propertyName);

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the current property value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected T GetProperty<T>(ref T value, [CallerMemberName] string propertyName = null)
        {
            // if we're recording gets as part of dependent property
#if !NET4
            _recordGets?.Record(propertyName);
#else
            if (_recordGets != null)
            {
                _recordGets.Record(propertyName);
            }
#endif

            GetOfCreateProperty(ref value, propertyName, () => new PropertyCommon<T>());
            return value;
        }

        /// <summary>
        /// Gets the current value via a Func, this will record 
        /// changes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="generateFunc"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected T ReadOnlyProperty<T>(Func<T> generateFunc, [CallerMemberName] string propertyName = null)
        {
            var defaultValue = default(T);
            var dependentProperty = GetOfCreateProperty(ref defaultValue, propertyName, () => new DependentProperty<T>(generateFunc));

            // in case we go recursive we need to record the top level property
            var topProperty = _recordGets == null;
            if (_recordGets == null)
            {
                _recordGets = new PropertyRecorder();
            }

            var result = dependentProperty.Value;

            if (topProperty)
            {
                dependentProperty.DependentUpon = _recordGets.Playback();
            }
            return result;
        }

        /// <summary>
        /// Gets or creates a property if it's not already created. If not already
        /// created it will also set-up property state such as created via PropertyAttributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyFactory"></param>
        /// <returns></returns>
        private TProperty GetOfCreateProperty<TProperty, T>(ref T value, string propertyName, Func<TProperty> propertyFactory) where
            TProperty : PropertyCommon<T>
        {
            if (propertyName == null)
                return default(TProperty);

            IProperty property;
            if (!_properties.TryGetValue(propertyName, out property))
            {
                var p = propertyFactory();
                property = p;
                ApplyAttributes(ref value, propertyName, p);

#if !NET4
                if (!_propertyDefinitions.NonTrackableProperties?.Contains(propertyName) ?? true)
#else
                    if (_propertyDefinitions.NonTrackableProperties == null || 
                        !_propertyDefinitions.NonTrackableProperties.Contains(propertyName))
#endif
                {
                    var tmp = value;
                    property.Attach(
                        () => Attach(tmp),
                        () => Detach(tmp)
                    );
                }
                _properties.Add(propertyName, property);
            }
            return property as TProperty;
        }

        /// <summary>
        /// Applies PropertyDefinition data (if any) to the property state, for example
        /// the comparable instance for the property, whether trackable or not etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <param name="property"></param>
        private void ApplyAttributes<T>(ref T value, string propertyName, PropertyCommon<T> property)
        {
#if !NET4
            var definition = _propertyDefinitions?[propertyName];
#else
            var definition = _propertyDefinitions != null ? _propertyDefinitions[propertyName] : null;
#endif
            if (definition != null)
            {
                value = SafeConvert.ChangeType<T>(definition.Default);
                property.Comparer = definition.Comparer;
                property.SupportsNotifications = definition.SupportsNotifications;
            }
        }
    }

}
