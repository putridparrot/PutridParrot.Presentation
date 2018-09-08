using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation.Core;
using PutridParrot.Presentation.Core.Helpers;
using Tests.Presentation.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class NotifyPropertyChangedTests
    {
        public class MyNotifyPropertyChanged : NotifyPropertyChanged
        {
            private string _name;
            private string _address;

            public string Name
            {
                get { return _name; }
                set { this.SetProperty(x => x.Name, ref _name, value); }
            }

            public string Address
            {
                get { return _address; }
                set { this.SetProperty(x => x.Address, ref _address, value); }
            }
        }


        [Test]
        public void NotifyPropertyChanged_OnMethod_ExpectException()
        {
            var viewModel = new MyNotifyPropertyChanged();

            Action action = () => viewModel.RaisePropertyChanged(x => x.ToString());
            action
                .Should()
                .Throw<Exception>();
        }

        [Test]
        public void NotifyPropertyChanged_EnsurePropertyChanged()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.Name = "Scooby Doo";

            listener.Changed.Count
                .Should()
                .Be(1);
            listener.Changed[0]
                .Should()
                .Be("Name");
        }

        [Test]
        public void NotifyPropertyChanged_EnsurePropertyChangedOnlyOnce()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.Name = "Scooby Doo";
            viewModel.Name = "Scooby Doo";

            listener.Changed.Count
                .Should()
                .Be(1);
            listener.Changed[0]
                .Should()
                .Be("Name");
        }

        [Test]
        public void NotifyPropertyChanged_EnsurePropertyChangedMultipleTimes()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.Name = "Scooby Doo1";
            viewModel.Name = "Scooby Doo2";

            listener.Changed.Count
                .Should()
                .Be(2);
            listener.Changed[0]
                .Should()
                .Be("Name");
            listener.Changed[1]
                .Should()
                .Be("Name");
        }

        [Test]
        public void NotifyPropertyChanged_EnsurePropertyChanging()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.Name = "Scooby Doo";

            listener.Changing.Count
                .Should()
                .Be(1);
            listener.Changing[0]
                .Should()
                .Be("Name");
        }

        [Test]
        public void NotifyPropertyChanged_EnsurePropertyChangingOnlyOnce()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.Name = "Scooby Doo";
            viewModel.Name = "Scooby Doo";

            listener.Changing.Count
                .Should()
                .Be(1);
            listener.Changing[0]
                .Should()
                .Be("Name");
        }

        [Test]
        public void NotifyPropertyChanged_EnsurePropertyChangingMultipleTimes()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.Name = "Scooby Doo1";
            viewModel.Name = "Scooby Doo2";

            listener.Changing.Count
                .Should()
                .Be(2);
            listener.Changing[0]
                .Should()
                .Be("Name");
            listener.Changing[1]
                .Should()
                .Be("Name");
        }

        [Test]
        public void NotifyPropertyChanged_RaisePropertyChanged()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.RaiseMultiplePropertyChanged(x => x.Name);

            listener.Changed.Count
                .Should()
                .Be(1);
            listener.Changed[0]
                .Should()
                .Be("Name");
            listener.Changing.Count
                .Should()
                .Be(0);
        }

        [Test]
        public void NotifyPropertyChanged_RaisePropertyChangedDirectlyOnViewModel()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.RaiseMultiplePropertyChanged("Name", "Name");

            listener.Changed.Count
                .Should()
                .Be(2);
            listener.Changed[0]
                .Should()
                .Be("Name");
            listener.Changing.Count
                .Should()
                .Be(0);
        }

        [Test]
        public void NotifyPropertyChanged_RaisePropertyChangedWithNull()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.RaisePropertyChanged();

            listener.Changed.Count
                .Should()
                .Be(1);
            listener.Changed[0]
                .Should()
                .BeNull();
            listener.Changing.Count
                .Should()
                .Be(0);
        }
    }
}
