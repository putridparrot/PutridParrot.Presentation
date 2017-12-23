using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Presentation.Core.Exceptions;
using Presentation.Core.Helpers;

namespace Presentation.Core
{
    /// <summary>
    /// Acts as a base class for view models, includes
    /// a backing store so derived classes do not need
    /// to create backing fields
    /// </summary>
    public class ViewModel : ViewModelCommon
    {
        /// <summary>
        /// Sets the property value against the property and raises
        /// OnPropertyChanging, OnPropertyChanged etc. as required
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(T value, [CallerMemberName] string propertyName = null)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            return SetProperty(value, null, propertyName);
        }

        /// <summary>
        /// Sets the property value against the property and raises
        /// OnPropertyChanging, OnPropertyChanged etc. as required
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="validationFunc"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(T value, Func<T, ValidationResult> validationFunc, [CallerMemberName] string propertyName = null)
        {
            var property = GetOfCreateProperty<Property<T>, T>(propertyName, () => new Property<T>());
            if (!property.Equals(property.Value, value))
            {
                // ReSharper disable once ExplicitCallerInfoArgument
                if (OnPropertyChanging(propertyName))
                {
                    var previousValue = property.Value;

                    property.Detach();

                    property.Value = value;

                    if (!_propertyDefinitions.NonTrackableProperties?.Contains(propertyName) ?? true)
                    {
                        property.Attach(
                            () => Attach(property.Value),
                            () => Detach(property.Value)
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
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            // if we're recording gets as part of dependent property
            _recordGets?.Record(propertyName);
            return GetOfCreateProperty<Property<T>, T>(propertyName, () => new Property<T>()).Value;
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
            var dependentProperty = GetOfCreateProperty<DependentProperty<T>, T>(propertyName, () => new DependentProperty<T>(generateFunc));

            // in case we go recursive we need to record the top level property
            var parentProperty = _recordGets == null;
            if (_recordGets == null)
            {
                _recordGets = new PropertyRecorder { InitialProperty = propertyName };
            }
            else
            {
                if (_recordGets.InitialProperty == propertyName)
                {
                    throw new PropertyCannotCallItselfException($"Property {propertyName} cannot get itself");
                }
            }

            var result = dependentProperty.Value;

            if (parentProperty)
            {
                dependentProperty.DependentUpon = _recordGets.Playback();
                _recordGets = null;
            }
            return result;
        }

        /// <summary>
        /// Gets or creates a property if it's not already created. If not already
        /// created it will also set-up property state such as created via PropertyAttributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="propertyFactory"></param>
        /// <returns></returns>
        private TProperty GetOfCreateProperty<TProperty, T>(string propertyName, Func<TProperty> propertyFactory) where
            TProperty : PropertyCommon<T>
        {
            if (propertyName == null)
                return default(TProperty);

            if (!_properties.TryGetValue(propertyName, out var property))
            {
                var p = propertyFactory();
                property = p;
                ApplyAttributes(propertyName, p);

                if (property is Property<T> tmp)
                {
                    if (!_propertyDefinitions.NonTrackableProperties?.Contains(propertyName) ?? true)
                    {
                        p.Attach(
                            () => Attach(tmp.Value),
                            () => Detach(tmp.Value)
                        );
                    }
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
        /// <param name="propertyName"></param>
        /// <param name="property"></param>
        private void ApplyAttributes<T>(string propertyName, PropertyCommon<T> property)
        {
            var definition = _propertyDefinitions?[propertyName];
            if (definition != null)
            {
                var p = property as Property<T>;
                if (p != null)
                {
                    p.Value = SafeConvert.ChangeType<T>(definition.Default);
                }

                property.SupportsNotifications = definition.SupportsNotifications;
                property.Comparer = definition.Comparer;
                property.Rules = definition.Rules;
            }
        }
    }
}
