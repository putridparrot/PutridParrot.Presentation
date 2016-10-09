using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Presentation.Core;

namespace Tests.Presentation.Core
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class NotifyPropertyChangedTests
    {
        [Test]
        public void NotifyPropertyChanged_OnMethod_ExpectException()
        {
            var viewModel = new MyNotifyPropertyChanged();

            Assert.Throws<Exception>(() => viewModel.RaisePropertyChanged(x => x.ToString()));
        }

        [Test]
        public void NotifyPropertyChanged_EnsurePropertyChanged()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.Name = "Scooby Doo";

            Assert.AreEqual(1, listener.Changed.Count);
            Assert.AreEqual("Name", listener.Changed[0]);
        }

        [Test]
        public void NotifyPropertyChanged_EnsurePropertyChangedOnlyOnce()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.Name = "Scooby Doo";
            viewModel.Name = "Scooby Doo";

            Assert.AreEqual(1, listener.Changed.Count);
            Assert.AreEqual("Name", listener.Changed[0]);
        }

        [Test]
        public void NotifyPropertyChanged_EnsurePropertyChangedMultipleTimes()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.Name = "Scooby Doo1";
            viewModel.Name = "Scooby Doo2";

            Assert.AreEqual(2, listener.Changed.Count);
            Assert.AreEqual("Name", listener.Changed[0]);
            Assert.AreEqual("Name", listener.Changed[1]);
        }

        [Test]
        public void NotifyPropertyChanged_EnsurePropertyChanging()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.Name = "Scooby Doo";

            Assert.AreEqual(1, listener.Changing.Count);
            Assert.AreEqual("Name", listener.Changing[0]);
        }

        [Test]
        public void NotifyPropertyChanged_EnsurePropertyChangingOnlyOnce()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.Name = "Scooby Doo";
            viewModel.Name = "Scooby Doo";

            Assert.AreEqual(1, listener.Changing.Count);
            Assert.AreEqual("Name", listener.Changing[0]);
        }

        [Test]
        public void NotifyPropertyChanged_EnsurePropertyChangingMultipleTimes()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.Name = "Scooby Doo1";
            viewModel.Name = "Scooby Doo2";

            Assert.AreEqual(2, listener.Changing.Count);
            Assert.AreEqual("Name", listener.Changing[0]);
            Assert.AreEqual("Name", listener.Changing[1]);
        }

        [Test]
        public void NotifyPropertyChanged_RaisePropertyChanged()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.RaiseMultiplePropertyChanged(x => x.Name);

            Assert.AreEqual(1, listener.Changed.Count);
            Assert.AreEqual("Name", listener.Changed[0]);

            Assert.AreEqual(0, listener.Changing.Count);
        }

        [Test]
        public void NotifyPropertyChanged_RaisePropertyChangedDirectlyOnViewModel()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.RaiseMultiplePropertyChanged("Name", "Name");

            Assert.AreEqual(2, listener.Changed.Count);
            Assert.AreEqual("Name", listener.Changed[0]);

            Assert.AreEqual(0, listener.Changing.Count);
        }

        [Test]
        public void NotifyPropertyChanged_RaisePropertyChangedWithNull()
        {
            var viewModel = new MyNotifyPropertyChanged();
            var listener = new NotifyPropertyChangedListener(viewModel);

            viewModel.RaisePropertyChanged();

            Assert.AreEqual(1, listener.Changed.Count);
            Assert.AreEqual(null, listener.Changed[0]);

            Assert.AreEqual(0, listener.Changing.Count);
        }
    }

}
