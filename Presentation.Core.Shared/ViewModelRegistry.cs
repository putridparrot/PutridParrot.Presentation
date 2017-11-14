using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Presentation.Patterns.Attributes;
using Presentation.Patterns.Exceptions;
using Presentation.Patterns.Interfaces;

namespace Presentation.Patterns
{
    /// <summary>
    /// This acts as a singleton for view model based code. It's
    /// aim is to handle initial reflection across view model properties
    /// and create shared instances of the property attributes etc. so 
    /// that when a new view model of the same type is created, reflection
    /// may have already occurred and be cached. Also handles sharing instances
    /// of other objects, such as comparison objects, so we're not
    /// creating multiple instances of the same type.
    /// 
    /// The current implementation is not threadsafe
    /// </summary>
    public sealed class ViewModelRegistry
    {
        /// <summary>
        /// Maps the "known" view model property attributes
        /// to this class to ensure we do not have reflect
        /// over the property each time a class uses it
        /// </summary>
        public sealed class PropertyDefinition
        {
            internal DefaultValueAttribute DefaultValue { get; set; }
            internal Type ComparerType { get; set; }
            internal Type PropertyType { get; set; }
            internal CreateInstanceAttribute CreateInstance { get; set; }
            internal CreateInstanceUsingAttribute CreateInstanceUsing { get; set; }

            public bool HasValidationAttributes { get; internal set; }
            public bool SupportsNotifications { get; internal set; }
            public bool ChangeTrackingDisabled { get; internal set; }
            public List<RuleAttribute> Rules { get; internal set; }

            public object Default
            {
                get
                {
                    try
                    {
                        if (CreateInstance != null)
                        {
                            if(DefaultValue != null)
                                return Activator.CreateInstance(PropertyType, DefaultValue.Value);

                            // if the property is an array using CreateInstance
                            // without a default, we simply create a 0 length array
                            return PropertyType.IsArray
                                ? Activator.CreateInstance(PropertyType, 0)
                                : Activator.CreateInstance(PropertyType);
                        }
                        if (CreateInstanceUsing != null)
                        {
                            var factory = (IFactory)Activator.CreateInstance(CreateInstanceUsing.FactoryType);
                            return factory.Create(DefaultValue?.Value);
                        }
                        if (DefaultValue != null)
                        {
                            return DefaultValue.Value;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                    return null;
                }
            }

            public object Comparer
            {
                get
                {
                    if (ComparerType == null)
                        return null;

                    // we want to maintain the only a single instance
                    // of a comparer for each comparer type
                    if (!Instance._comparerTypes.TryGetValue(ComparerType, out var comparer) || comparer == null)
                    {
                        try
                        {
                            comparer = Activator.CreateInstance(ComparerType);
                            Instance._comparerTypes[ComparerType] = comparer;
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }
                    return comparer;
                }
            }

            public string DisplayName { get; internal set; }
        }

        public sealed class PropertyDefinitions
        {
            private readonly Dictionary<string, PropertyDefinition> _properties;

            internal int _refCount;

            public PropertyDefinitions()
            {
                _properties = new Dictionary<string, PropertyDefinition>();
            }

            public PropertyDefinition this[string propertyName]
            {
                get
                {
                    _properties.TryGetValue(propertyName, out var propertyDefinition);
                    return propertyDefinition;
                }
                set => _properties[propertyName] = value;
            }

            public HashSet<string> NonTrackableProperties { get; internal set; }

#if !NET4
            public int Count => _properties?.Count ?? 0;
#else
            public int Count
            {
                get
                {
                    return _properties != null ? _properties.Count : 0;
                }
            }
#endif
        }

        /// <summary>
        /// Maintains the property definitions for a type
        /// </summary>
        private readonly Dictionary<Type, PropertyDefinitions> _typePropertyDefinitions;
        /// <summary>
        /// Maintains a comparer instance for a IEqualityComparer implementation type
        /// </summary>
        private readonly Dictionary<Type, object> _comparerTypes;

        private ViewModelRegistry()
        {
            _typePropertyDefinitions = new Dictionary<Type, PropertyDefinitions>();
            _comparerTypes = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Register a view model type with the registry. This 
        /// basically acts as a reference count to decide when 
        /// to free up resources that are no longer used
        /// </summary>
        /// <param name="type"></param>
        public PropertyDefinitions Register(Type type)
        {
            if (!_typePropertyDefinitions.TryGetValue(type, out var propertyDefinitions))
            {
                propertyDefinitions = new PropertyDefinitions();
                Populate(propertyDefinitions, type);
                _typePropertyDefinitions.Add(type, propertyDefinitions);
            }
            propertyDefinitions._refCount++;
            return propertyDefinitions;
        }

        /// <summary>
        /// Unregister a view mode type with the registry. This
        /// will reduce the reference count and eventually clean up 
        /// resources.
        /// </summary>
        /// <param name="type"></param>
        public void UnRegister(Type type)
        {
            if (_typePropertyDefinitions.TryGetValue(type, out var propertyDefinitions))
            {
                if (--propertyDefinitions._refCount <= 0)
                {
                    _typePropertyDefinitions.Remove(type);
                }
            }
        }

        public void Clear(Type type)
        {
            _typePropertyDefinitions.Remove(type);
        }

        public PropertyDefinitions Get(Type type)
        {
            if (!_typePropertyDefinitions.TryGetValue(type, out var propertyDefinitions))
            {
                throw new TypeNotRegisteredException("Type not registered");
            }

            return propertyDefinitions;
        }

        /// <summary>
        /// Populates the supplied PropertyDefinitions with attribute data 
        /// etc. 
        /// </summary>
        /// <param name="propertyDefinitions">Expects non-null PropertyDefinitions object
        /// which it will populate with property data, including attributes etc.</param>
        /// <param name="type">The type to create properties for</param>
        private void Populate(PropertyDefinitions propertyDefinitions, Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                PropertyDefinition propertyDefinition = null;

                // by default ICommand are not change tracked, this can be overridden
                // by supply a ChangeTrackingAttribute
                if (typeof(ICommand).IsAssignableFrom(property.PropertyType))
                {
                    propertyDefinition = new PropertyDefinition { ChangeTrackingDisabled = true };
                }

                // this needs a bit of love it's going to create a property def
                // when any custom attributes are found, which is not much use
                // if unknown attributes were found
#if !NET4
                var attributes = property.GetCustomAttributes().ToArray();
#else
                var attributes = property.GetCustomAttributes(true).Cast<Attribute>().ToArray();
#endif
                if (attributes.Length > 0)
                {
                    if (propertyDefinition == null)
                    {
                        propertyDefinition = new PropertyDefinition();
                    }

                    foreach (var attribute in attributes)
                    {
                        AssignAttribute(property, propertyDefinition, attribute);
                    }
                }

                if (propertyDefinition != null)
                {
                    if (propertyDefinition.ChangeTrackingDisabled)
                    {
                        if (propertyDefinitions.NonTrackableProperties == null)
                        {
                            propertyDefinitions.NonTrackableProperties = new HashSet<string>();
                        }
                        propertyDefinitions.NonTrackableProperties.Add(property.Name);
                    }

                    propertyDefinitions[property.Name] = propertyDefinition;
                }
            }

            // if the type is associated with a metadata type, then we also need to get info.
            // from that and apply to properties
            AssociateMetadataType(propertyDefinitions, type);
        }

        /// <summary>
        /// Associates the meta data class with the (if it supports one).
        /// </summary>
        /// <param name="propertyDefinitions"></param>
        /// <param name="type"></param>
        private void AssociateMetadataType(PropertyDefinitions propertyDefinitions, Type type)
        {
#if !NETSTANDARD2_0
            foreach (var attribute in type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).Cast<MetadataTypeAttribute>())
            {
                if (type != attribute.MetadataClassType)
                {
                    TypeDescriptor.AddProviderTransparent(
                        new AssociatedMetadataTypeTypeDescriptionProvider(type, attribute.MetadataClassType), type);

                    Populate(propertyDefinitions, attribute.MetadataClassType);
                }
            }
#endif
        }

        /// <summary>
        /// Assigns attribute data to the property definition to save us having to find
        /// this on a usage basis for the given type.
        /// </summary>
        /// <param name="property">The PropertyInfo to be applied to the PropertyDefinition</param>
        /// <param name="propertyDefinition">The PropertyDefinition is the locally stored data from PropertyInfo and Attribute</param>
        /// <param name="attribute">The Attribute to be applied to the PropertyDefinition</param>
        private void AssignAttribute(PropertyInfo property, PropertyDefinition propertyDefinition, Attribute attribute)
        {
            propertyDefinition.PropertyType = property.PropertyType;
            propertyDefinition.SupportsNotifications = TypeSupportsNotifications(property.PropertyType);

            if (property.CanWrite)
            {
                // this is used by validation
                var displayNameAttribute = attribute as DisplayNameAttribute;
                if (displayNameAttribute != null)
                {
                    propertyDefinition.DisplayName = displayNameAttribute.DisplayName;
                }

                // register that validation attributes exist
                if (attribute is ValidationAttribute)
                {
                    propertyDefinition.HasValidationAttributes = true;
                }
            }

            // need to think of a more dynamic way to assign these things maybe?
            var defaultValueAttribute = attribute as DefaultValueAttribute;
            if (defaultValueAttribute != null)
            {
                propertyDefinition.DefaultValue = defaultValueAttribute;
                return;
            }

            var createInstanceAttribute = attribute as CreateInstanceAttribute;
            if (createInstanceAttribute != null)
            {
                propertyDefinition.CreateInstance = createInstanceAttribute;
                return;
            }

            var createInstanceUsing = attribute as CreateInstanceUsingAttribute;
            if (createInstanceUsing != null)
            {
                try
                {
                    if (createInstanceUsing.FactoryType != null &&
                        typeof(IFactory).IsAssignableFrom(createInstanceUsing.FactoryType))
                    {
                        propertyDefinition.CreateInstanceUsing = createInstanceUsing;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                return;
            }

            var trackingAttribute = attribute as ChangeTrackingAttribute;
            if (trackingAttribute != null)
            {
                propertyDefinition.ChangeTrackingDisabled = !trackingAttribute.IsTracking;
                return;
            }

            var comparerAttribute = attribute as ComparerAttribute;
            if (comparerAttribute != null && comparerAttribute.ComparerType != null)
            {
                if (!_comparerTypes.ContainsKey(comparerAttribute.ComparerType))
                {
                    _comparerTypes.Add(comparerAttribute.ComparerType, null);
                }

                propertyDefinition.ComparerType = comparerAttribute.ComparerType;
                return;
            }

            var ruleAttribute = attribute as RuleAttribute;
            if (ruleAttribute != null)
            {
                if (propertyDefinition.Rules == null)
                {
                    propertyDefinition.Rules = new List<RuleAttribute>();
                }
                propertyDefinition.Rules.Add(ruleAttribute);
                return;
            }
        }

        /// <summary>
        /// Checks if the type supports notifications, which includes
        /// INotifyCollectionChanges, INotifyPropertyChanged and IItemChanged
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool TypeSupportsNotifications(Type type)
        {
            return typeof(INotifyCollectionChanged).IsAssignableFrom(type) ||
                   typeof(INotifyPropertyChanged).IsAssignableFrom(type) ||
                   typeof(IItemChanged).IsAssignableFrom(type);
        }

        /// <summary>
        /// Singleton representation of the class to reduce instance creation
        /// and lookups of types via reflection.
        /// </summary>
        public static readonly ViewModelRegistry Instance = new ViewModelRegistry();
    }

}
