using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Presentation.Core
{
    /// <summary>
    /// Validation class which uses MetadataType capabilities from the DataAnnotations
    /// library.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ViewModelValidation<T> : IValidateViewModel
    {
        private readonly Dictionary<string, string> _validationRules;
        private readonly T _viewModel;

        public ViewModelValidation(T viewModel)
        {
            this._viewModel = viewModel;

            _validationRules = new Dictionary<string, string>();
            Initialize();
        }

        private static void AssociateMetadataType(object viewModel)
        {
            var viewModelType = viewModel.GetType();
            foreach (var attribute in viewModelType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).Cast<MetadataTypeAttribute>())
            {
                TypeDescriptor.AddProviderTransparent(
                    new AssociatedMetadataTypeTypeDescriptionProvider(viewModelType, attribute.MetadataClassType), viewModelType);
            }
        }

        ValidationResult[] IValidateViewModel.Validate(string propertyName, object newValue)
        {
            if (_validationRules.ContainsKey(propertyName))
            {
                var displayName = _validationRules[propertyName];

                var validationContext = new ValidationContext(_viewModel, null, null)
                {
                    DisplayName = displayName,
                    MemberName = propertyName
                };

                var errors = new List<ValidationResult>();
                if (!Validator.TryValidateProperty(newValue, validationContext, errors))
                {
                    return errors.ToArray();
                }
            }

            return null;
        }

        ValidationResult[] IValidateViewModel.Validate()
        {
            var errors = new List<ValidationResult>();
            if (!Validator.TryValidateObject(_viewModel, new ValidationContext(_viewModel, null, null), errors, true))
            {
                return errors.ToArray();
            }
            return null;
        }

        private void Initialize()
        {
            AssociateMetadataType(_viewModel);

            var metadataType = _viewModel.GetType().GetCustomAttributes(typeof(MetadataTypeAttribute), true)
                .OfType<MetadataTypeAttribute>().FirstOrDefault();

            var type = metadataType?.MetadataClassType ?? _viewModel.GetType();

            var setterProperties = type.GetProperties(BindingFlags.Public
                                                      | BindingFlags.Instance
                                                      | BindingFlags.DeclaredOnly);

            foreach (var property in setterProperties.Where(p => p.CanWrite))
            {
                var displayName = property.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().FirstOrDefault();
                _validationRules.Add(property.Name, displayName != null ? displayName.DisplayName : property.Name);
            }
        }
    }

}


