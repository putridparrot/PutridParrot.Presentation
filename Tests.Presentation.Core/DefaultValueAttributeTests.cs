using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Core;
using Presentation.Core.Attributes;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class DefaultValueAttributeTests
    {
        #region ViewModels
        interface IMyViewModel
        {
            int Numeric { get; set; }
            string[] Array { get; set; }
            ExtendedObservableCollection<string> Collection { get; }
        }

        class MyViewModel : ViewModel,
            IMyViewModel
        {
            [DefaultValue(6)]
            public int Numeric
            {
                get { return GetProperty<int>(); }
                set { SetProperty(value); }
            }

            [DefaultValue(new[] {"A", "B", "C"})]
            public string[] Array
            {
                get { return GetProperty<string[]>(); }
                set { SetProperty(value); }
            }

            [DefaultValue(new[] { "X", "Y", "Z"})]
            [CreateInstance]
            public ExtendedObservableCollection<string> Collection => 
                GetProperty<ExtendedObservableCollection<string>>();
        }

        class MyViewModelWithoutBacking : ViewModelWithoutBacking,
            IMyViewModel
        {
            private int _numeric;
            private string[] _array;
            private ExtendedObservableCollection<string> _collection;

            [DefaultValue(6)]
            public int Numeric
            {
                get { return GetProperty(ref _numeric); }
                set { SetProperty(ref _numeric, value); }
            }

            [DefaultValue(new[] { "A", "B", "C" })]
            public string[] Array
            {
                get { return GetProperty(ref _array); }
                set { SetProperty(ref _array, value); }
            }

            [DefaultValue(new[] { "X", "Y", "Z" })]
            [CreateInstance]
            public ExtendedObservableCollection<string> Collection =>
                GetProperty(ref _collection);
        }

        class MyViewModelWithModel : ViewModelWithModel,
            IMyViewModel
        {
            class Model
            {
                public int Numeric { get; set; }
                public string[] Array { get; set; }
                public ExtendedObservableCollection<string> Collection { get; set; }
            }

            private readonly Model _model = new Model();

            [DefaultValue(6)]
            public int Numeric
            {
                get { return GetProperty(() => _model.Numeric, v => _model.Numeric = v); }
                set { SetProperty(() => _model.Numeric, v => _model.Numeric = v, value); }
            }

            [DefaultValue(new[] { "A", "B", "C" })]
            public string[] Array
            {
                get { return GetProperty(() => _model.Array, v => _model.Array = v); }
                set { SetProperty(() => _model.Array, v => _model.Array = v, value); }
            }

            [DefaultValue(new[] { "X", "Y", "Z" })]
            [CreateInstance]
            public ExtendedObservableCollection<string> Collection =>
                GetProperty(() => _model.Collection, v => _model.Collection = v);
        }
        #endregion

        [TestCase(typeof(MyViewModel))]
        [TestCase(typeof(MyViewModelWithoutBacking))]
        [TestCase(typeof(MyViewModelWithModel))]
        public void PrimitiveTest_ExpectDefaultValueToBeAssigned(Type viewModelType)
        {
            var vm = (IMyViewModel)Activator.CreateInstance(viewModelType);

            vm.Numeric
                .Should()
                .Be(6);
        }

        [TestCase(typeof(MyViewModel))]
        [TestCase(typeof(MyViewModelWithoutBacking))]
        [TestCase(typeof(MyViewModelWithModel))]
        public void ArrayTest_ExpectArrayElementsToBeAssigned(Type viewModelType)
        {
            var vm = (IMyViewModel)Activator.CreateInstance(viewModelType);

            vm.Array.Length
                .Should()
                .Be(3);

            vm.Array[0]
                .Should()
                .Be("A");
            vm.Array[1]
                .Should()
                .Be("B");
            vm.Array[2]
                .Should()
                .Be("C");
        }

        [TestCase(typeof(MyViewModel))]
        [TestCase(typeof(MyViewModelWithoutBacking))]
        [TestCase(typeof(MyViewModelWithModel))]
        public void CollectionTest_ExpectCollectionToBeCreatedAndElementsToBeAssigned(Type viewModelType)
        {
            var vm = (IMyViewModel)Activator.CreateInstance(viewModelType);

            vm.Collection.Count
                .Should()
                .Be(3);

            vm.Collection[0]
                .Should()
                .Be("X");
            vm.Collection[1]
                .Should()
                .Be("Y");
            vm.Collection[2]
                .Should()
                .Be("Z");
        }
    }
}
