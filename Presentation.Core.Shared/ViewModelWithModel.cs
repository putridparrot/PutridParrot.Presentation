using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using PutridParrot.Presentation.Core.Helpers;

namespace PutridParrot.Presentation.Core
{
    /// <summary>
    /// Acts as a base class for view models, can be used
    /// with backing fields or delegating getter/setter 
    /// functionality to another class - useful for situations
    /// where the underlying model is used directly
    /// </summary>
    public class ViewModelWithModel : ViewModelCommon
    {
        /// <summary>
        /// Sets the property value against the property and raises
        /// OnPropertyChanging, OnPropertyChanged etc. as required.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(Func<T> getter, Func<T, T> setter, T value, [CallerMemberName] string propertyName = null)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            return SetProperty(getter, setter, value, null, propertyName);
        }

        /// <summary>
        /// Sets the property value against the property and raises
        /// OnPropertyChanging, OnPropertyChanged etc. as required
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <param name="value"></param>
        /// <param name="validationFunc"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(Func<T> getter, Func<T, T> setter, T value, Func<T, ValidationResult> validationFunc, [CallerMemberName] string propertyName = null)
        {
            var property = GetOfCreateProperty(getter, setter, propertyName, () => new PropertyCommon<T>());
            var currentValue = getter();
            if (!property.Equals(currentValue, value))
            {
                // ReSharper disable once ExplicitCallerInfoArgument
                if (OnPropertyChanging(propertyName))
                {
                    var previousValue = currentValue;

                    property.Detach();

                    setter(value);

                    if (!_propertyDefinitions.NonTrackableProperties?.Contains(propertyName) ?? true)
                    {
                        var tmp = value;
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
        /// Gets the current property value as type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected T GetProperty<T>(Func<T> getter, Func<T, T> setter, [CallerMemberName] string propertyName = null)
        {
            // if we're recording gets as part of dependent property
            _recordGets?.Record(propertyName);

            GetOfCreateProperty(getter, setter, propertyName, () => new PropertyCommon<T>());
            return getter();
        }

        /// <summary>
        /// Gets the current value via a Func via the <paramref name="generateFunc"/>
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="generateFunc">The function to create the return value</param>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The value of type T</returns>
        protected T ReadOnlyProperty<T>(Func<T> generateFunc, [CallerMemberName] string propertyName = null)
        {
            var defaultGetter = new Func<T>(() => default(T));
            var defaultSetter = new Func<T, T>(v => v);
            var dependentProperty = GetOfCreateProperty(defaultGetter, defaultSetter, propertyName, () => new DependentProperty<T>(generateFunc));

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
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyFactory"></param>
        /// <returns></returns>
        private TProperty GetOfCreateProperty<TProperty, T>(Func<T> getter, Func<T, T> setter, string propertyName, Func<TProperty> propertyFactory) where
            TProperty : PropertyCommon<T>
        {
            if (propertyName == null)
                return default(TProperty);

            if (!_properties.TryGetValue(propertyName, out var property))
            {
                var p = propertyFactory();
                property = p;
                ApplyAttributes(setter, propertyName, p);

                if (!_propertyDefinitions.NonTrackableProperties?.Contains(propertyName) ?? true)
                {
                    var tmp = getter();
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
        /// <param name="setter"></param>
        /// <param name="propertyName"></param>
        /// <param name="property"></param>
        private void ApplyAttributes<T>(Func<T, T> setter, string propertyName, PropertyCommon<T> property)
        {
            var definition = _propertyDefinitions?[propertyName];
            if (definition != null)
            {
                setter(SafeConvert.ChangeType<T>(definition.Default));
                property.Comparer = definition.Comparer;
                property.SupportsNotifications = definition.SupportsNotifications;
            }
        }
    }

}
