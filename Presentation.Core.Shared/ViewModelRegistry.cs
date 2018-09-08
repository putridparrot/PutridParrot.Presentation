using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using PutridParrot.Presentation.Core.Attributes;
using PutridParrot.Presentation.Core.Exceptions;
using PutridParrot.Presentation.Core.Interfaces;

namespace PutridParrot.Presentation.Core
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
            /// <summary>
            /// The default value to be assigned lazily (if a default vlaue is applied
            /// to a property)
            /// </summary>
            internal DefaultValueAttribute DefaultValue { get; set; }
            /// <summary>
            /// The comparer to be used on the type
            /// </summary>
            internal Type ComparerType { get; set; }
            /// <summary>
            /// The property type
            /// </summary>
            internal Type PropertyType { get; set; }
            /// <summary>
            /// The CreateInstance attribute if one is applied to a property
            /// </summary>
            internal CreateInstanceAttribute CreateInstance { get; set; }
            /// <summary>
            /// The CreateInstanceUsing attribute if one is applied to a property
            /// </summary>
            internal CreateInstanceUsingAttribute CreateInstanceUsing { get; set; }

            /// <summary>
            /// Gets/Sets a boolean return which states whether validation attributes have been
            /// assigned to a property
            /// </summary>
            public bool HasValidationAttributes { get; internal set; }
            /// <summary>
            /// Gets/Sets a boolean which states whether a property supports notifications
            /// </summary>
            public bool SupportsNotifications { get; internal set; }
            /// <summary>
            /// Gets/Sets a boolean if change tracking is applied to a property
            /// </summary>
            public bool ChangeTrackingDisabled { get; internal set; }
            /// <summary>
            /// Gets/sets the list of rules application to a property
            /// </summary>
            public List<RuleAttribute> Rules { get; internal set; }

            /// <summary>
            /// Gets the a value as a default when a property is accessed. This will use
            /// the CreateInstance, CreateInstanceUsing or Default attributes
            /// </summary>
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

            /// <summary>
            /// Gets the comparer object by creating is as needed and storing for use across all properties
            /// that are assigned it's type. Ensures we do not create multiple instances of the same comparer
            /// type.
            /// </summary>
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

            /// <summary>
            /// Gets/Sets the DisplayName which can be used to replace the property name in
            /// a more human readable manner
            /// </summary>
            public string DisplayName { get; internal set; }
        }

        /// <summary>
        /// PropertyDefinitions allows caches property attributes/definitions
        /// to allow us to use without having to use reflection multiple times
        /// on the same type. 
        /// </summary>
        public sealed class PropertyDefinitions
        {
            private readonly Dictionary<string, PropertyDefinition> _properties;

            internal int _refCount;

            public PropertyDefinitions()
            {
                _properties = new Dictionary<string, PropertyDefinition>();
            }

            /// <summary>
            /// Gets/Sets the property defintion for a given property name
            /// </summary>
            /// <param name="propertyName"></param>
            /// <returns></returns>
            public PropertyDefinition this[string propertyName]
            {
                get
                {
                    _properties.TryGetValue(propertyName, out var propertyDefinition);
                    return propertyDefinition;
                }
                set => _properties[propertyName] = value;
            }

            /// <summary>
            /// Gets/Sets a collection of non-trackable properties to allow
            /// faster lookups
            /// </summary>
            public HashSet<string> NonTrackableProperties { get; internal set; }

            /// <summary>
            /// Gets the number of properties within this property definition
            /// </summary>
            public int Count => _properties?.Count ?? 0;
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

        /// <summary>
        /// Clear the stored property definitions for the supplied type
        /// </summary>
        /// <param name="type"></param>
        public void Clear(Type type)
        {
            _typePropertyDefinitions.Remove(type);
        }

        /// <summary>
        /// Gets the property definitions for a given type. If the type does not exist 
        /// the TypeNotRegisterdException is thrown
        /// </summary>
        /// <exception cref="TypeNotRegisteredException">Occurs if the type is not stored</exception>
        /// <param name="type">The type to query for</param>
        /// <returns>The PropertyDefinitions for the given type</returns>
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
                var attributes = property.GetCustomAttributes().ToArray();
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
        /// Currently not supported by .NET Standard 2.0
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
                if (attribute is DisplayNameAttribute displayNameAttribute)
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
            if (attribute is DefaultValueAttribute defaultValueAttribute)
            {
                propertyDefinition.DefaultValue = defaultValueAttribute;
                return;
            }

            if (attribute is CreateInstanceAttribute createInstanceAttribute)
            {
                propertyDefinition.CreateInstance = createInstanceAttribute;
                return;
            }

            if (attribute is CreateInstanceUsingAttribute createInstanceUsing)
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

            if (attribute is ChangeTrackingAttribute trackingAttribute)
            {
                propertyDefinition.ChangeTrackingDisabled = !trackingAttribute.IsTracking;
                return;
            }

            if (attribute is ComparerAttribute comparerAttribute && comparerAttribute.ComparerType != null)
            {
                if (!_comparerTypes.ContainsKey(comparerAttribute.ComparerType))
                {
                    _comparerTypes.Add(comparerAttribute.ComparerType, null);
                }

                propertyDefinition.ComparerType = comparerAttribute.ComparerType;
                return;
            }

            if (attribute is RuleAttribute ruleAttribute)
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
