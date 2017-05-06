using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Patterns;
using Presentation.Patterns.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class DisposableActionTestscs
    {
        [Test]
        public void OnCreation_ExpectOnCreateActionToRun()
        {
            var onCreateCalled = false;

            var disposable = new DisposableAction(() => onCreateCalled = true, () => { });

            onCreateCalled
                .Should()
                .BeTrue();
        }

        [Test]
        public void OnDispose_ExpectOnCreateActionToRun()
        {
            var onDisposeCalled = false;

            var disposable = new DisposableAction(() => { }, () => onDisposeCalled = true);

            disposable.Dispose();

            onDisposeCalled
                .Should()
                .BeTrue();
        }

        public class SimpleViewModel : ViewModel
        {
            public string Name
            {
                get { return GetProperty<string>(); }
                set { SetProperty(value); }
            }            
        }

        [Test]
        public void WithEvent_ExpectAttachAndOnDisposeDetach()
        {
            var vm = new SimpleViewModel();

            var count = 0;
            var ev = new PropertyChangedEventHandler((sender, args) =>
            {
                // ignore IsChanged for this test
                if(args.PropertyName != "IsChanged")
                    count++;
            });

            var disposable = new DisposableAction(
                () => vm.PropertyChanged += ev,
                () => vm.PropertyChanged -= ev);

            vm.Name = "One";

            count
                .Should()
                .Be(1);

            disposable.Dispose();

            vm.Name = "Two";

            count
                .Should()
                .Be(1);
        }
    }
}
