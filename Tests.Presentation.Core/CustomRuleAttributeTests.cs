using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Core;
using Presentation.Core.Attributes;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class CustomRuleAttributeTests
    {   
        // this is a custom rule attribute which, when 
        // the associated property changes will cause 
        // the rule to run, in this case ensuring
        // the child selected property is in sync with
        // the parent selected - this could be achieved
        // in another way, but shows the rule attribute
        // in a less general usage
        class SelectedChangedAttribute : RuleAttribute
        {
            public override bool PostInvoke(object o)
            {
                var vm = o as MyViewModel;
                if (vm != null && vm.Child != null)
                {
                    vm.Child.Selected = vm.Selected;
                }

                return true;
            }
        }

        class MyViewModel : ViewModel
        {
            public class ChildViewModel : ViewModel
            {
                public bool Selected
                {
                    get { return GetProperty<bool>(); }
                    set { SetProperty(value); }
                }
            }

            [CreateInstance]
            public ChildViewModel Child => GetProperty<ChildViewModel>();

            [SelectedChanged]
            public bool Selected
            {
                get { return GetProperty<bool>(); }
                set { SetProperty(value); }
            }
        }

        [Test]
        public void CustomRuleAttribute_EnsureWhenParentSelectedChangesChildSelectedChanges()
        {
            var vm = new MyViewModel {Selected = true};

            vm.Child.Selected
                .Should()
                .BeTrue();

            vm.Selected = false;

            vm.Child.Selected
                .Should()
                .BeFalse();
        }
    }
}