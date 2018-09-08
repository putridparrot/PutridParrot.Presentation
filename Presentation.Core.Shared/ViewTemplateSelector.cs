#if !NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PutridParrot.Presentation.Core.Helpers;

namespace PutridParrot.Presentation.Core
{
    /// <summary>
    /// Create a template selector which will select a view automatically
    /// for a given model
    /// 
    /// Declare &ltp:ViewTemplateSelector x:Key="ViewSelector"/%gt;
    /// in your ResourceDictionary and from a TabControl (for example)
    /// set the ContentTemplateSelector to {StaticResource ViewSelector}
    /// and the ItemsSource to a collection of view model types. 
    /// Te ViewTemplateSelector will automatically select the appropriate 
    /// view for the given tab.
    /// </summary>
    public class ViewTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<string, DataTemplate> _dataTemplates = new Dictionary<string, DataTemplate>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var contentPresent = container as ContentPresenter;
            if (contentPresent != null)
            {
                if (item != null)
                {
                    var type = item.GetType();
                    var name = ViewModelConvention.GetViewName(type.Name);
                    if (!String.IsNullOrEmpty(name))
                    {
                        if (_dataTemplates.ContainsKey(name))
                        {
                            return _dataTemplates[name];
                        }

                        var match = type.Assembly.GetTypes().FirstOrDefault(t => t.Name == name);
                        if (match != null)
                        {
                            var factory = new FrameworkElementFactory(match);
                            var dataTemplate = new DataTemplate(type)
                            {
                                VisualTree = factory
                            };
                            _dataTemplates.Add(name, dataTemplate);
                            return dataTemplate;
                        }
                    }
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
#endif