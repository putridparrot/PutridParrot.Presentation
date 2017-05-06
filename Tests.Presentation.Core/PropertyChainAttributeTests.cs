using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Data;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Patterns;
using Presentation.Patterns.Attributes;
using Tests.Presentation.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class PropertyChainAttributeTests
    {
        public class MyViewModel : ViewModel
        {
            private string[] _all;

            public MyViewModel()
            {
                BeginInit();
                Details = CollectionViewSource.GetDefaultView(All);
                EndInit();
            }

            // public for tests
            public string[] All => _all ?? (_all = new[]
                                   {
                                       "Scooby",
                                       "Shaggy",
                                       "Velma",
                                       "Daphne",
                                       "Fred"
                                   });

            private void ApplyFilter()
            {
                Details.Filter = FilterByHumans ? 
                    Details.Filter = i => ((string)i) != "Scooby" : 
                    null;
            }

            [PropertyChain(nameof(Details))]
            public bool FilterByHumans
            {
                get { return GetProperty<bool>(); }
                set
                {
                    if (SetProperty(value))
                    {
                        ApplyFilter();
                    }
                }
            }

            public ICollectionView Details
            {
                get { return GetProperty<ICollectionView>(); }
                private set { SetProperty(value); }
            }

            [PropertyChain(nameof(Child))]
            public bool Parent
            {
                get { return GetProperty<bool>(); }
                set { SetProperty(value); }
            }

            [PropertyChain(nameof(GrandChild))]
            public bool Child => GetProperty<bool>();
            public bool GrandChild => GetProperty<bool>();

            [PropertyChain(nameof(Child1), nameof(Child2))]
            public bool Multiple
            { 
                get { return GetProperty<bool>(); }
                set { SetProperty(value); }
            }

            public bool Child1 => GetProperty<bool>();
            public bool Child2 => GetProperty<bool>();
        }

        [Test]
        public void Ensure_DefaultStateIsValidAndAsExpected()
        {
            var vm = new MyViewModel();

            vm.FilterByHumans
                .Should()
                .BeFalse();

            vm.Details.SourceCollection
                .Should()
                .BeSameAs(vm.All);
        }

        [Test]
        public void Filtered_ExpectFiltered()
        {
            var vm = new MyViewModel();
            var binding = new ViewBinding(vm);

            vm.FilterByHumans = true;

            vm.Details.SourceCollection
                .Should()
                .BeSameAs(vm.All);

            binding.Changed.Count
                .Should()
                .Be(3);

            binding.Changed[0]
                .Should()
                .Be("IsChanged");
            binding.Changed[1]
                .Should()
                .Be("FilterByHumans");
            binding.Changed[2]
                .Should()
                .Be("Details");
        }

        [Test]
        public void EnsureAChainedPropertyCanRaiseAnotherChangedProperty()
        {
            var vm = new MyViewModel();
            var binding = new ViewBinding(vm);

            vm.FilterByHumans = true;

            vm.Details.SourceCollection
                .Should()
                .BeSameAs(vm.All);

            binding.Changed.Count
                .Should()
                .Be(3);

            binding.Changed[0]
                .Should()
                .Be("IsChanged");
            binding.Changed[1]
                .Should()
                .Be("FilterByHumans");
            binding.Changed[2]
                .Should()
                .Be("Details");
        }

        [Test]
        public void PropogateChained_ExpectParentToRaiseChilToRaiseGrandChild()
        {
            var vm = new MyViewModel();
            var binding = new ViewBinding(vm);

            vm.Parent = true;

            binding.Changed.Count
                .Should()
                .Be(4);
            binding.Changed[0]
                .Should()
                .Be("IsChanged");
            binding.Changed[1]
                .Should()
                .Be("Parent");
            binding.Changed[2]
                .Should()
                .Be("Child");
            binding.Changed[3]
                .Should()
                .Be("GrandChild");
        }

        [Test]
        public void MultipleChainedPropertiesFromSingleProperty_EnsureBothChainedAreRaised()
        {
            var vm = new MyViewModel();
            var binding = new ViewBinding(vm);

            vm.Multiple = true;

            binding.Changed.Count
                .Should()
                .Be(4);
            binding.Changed[0]
                .Should()
                .Be("IsChanged");
            binding.Changed[1]
                .Should()
                .Be("Multiple");
            binding.Changed[2]
                .Should()
                .Be("Child1");
            binding.Changed[3]
                .Should()
                .Be("Child2");
        }
    }
}
