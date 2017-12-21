using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Core;
using Presentation.Core.Attributes;
using Presentation.Core.Interfaces;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ViewModelCommandTests
    {
        class SimpleCommand : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return false;
            }

            public void Execute(object parameter)
            {
            }

            public event EventHandler CanExecuteChanged;
        }

        class CommandFactory : IFactory
        {
            // creates and ActionCoommand with default ctor, therefore no
            // actions, but within the vm we can set these via the ActionCommand
            // ExecuteCommand etc. properties
            public object Create(params object[] args)
            {
                return new ActionCommand();
            }
        }

        class CommandViewModel : ViewModel
        {
            public ICommand Close
            {
                get { return GetProperty<ICommand>(); }
                set { SetProperty(value); }
            }

            [ChangeTracking]
            public ICommand Ok
            {
                get { return GetProperty<ICommand>(); }
                set { SetProperty(value); }
            }

            [CreateInstanceUsing(typeof(CommandFactory))]
            public ICommand AutoCreated => GetProperty<ICommand>();

            [CreateInstance]
            public ICommand AutoCreateWithoutFactory => GetProperty<ICommand>();
        }

        [Test]
        public void Command_AutoCreateWithoutFactory_ShouldFailWithInterfacePropertyType()
        {
            var vm = new CommandViewModel();

            vm.AutoCreateWithoutFactory
                .Should()
                .BeNull();
        }

        [Test]
        public void Command_AutoCreateUsingFactory()
        {
            var vm = new CommandViewModel();

            vm.AutoCreated
                .Should()
                .NotBeNull();
        }

        [Test]
        public void Command_EnsureHandledBySetPropertyGetProperty()
        {
            // if it makes sense to include command's within the 
            // ViewModel mechanisms
            var vm = new CommandViewModel();
            var cmd = new SimpleCommand();

            vm.Close = cmd;

            vm.Close
                .Should()
                .BeSameAs(cmd);
        }

        [Test]
        public void Command_NotCreatedAutomaticallyExpectNull()
        {
            var vm = new CommandViewModel();

            vm.Close
                .Should()
                .BeNull();
        }

        [Test]
        public void Command_ByDefaultSetPropertyOnICommandWillNotBeTracked()
        {
            var vm = new CommandViewModel();
            var cmd = new SimpleCommand();

            vm.Close = cmd;

            vm.IsChanged
                .Should()
                .BeFalse();
        }

        [Test]
        public void Command_OverrideDefaultTrackingToEnabledSetPropertyOnICommandWillToBeTracked()
        {
            var vm = new CommandViewModel();
            var cmd = new SimpleCommand();

            vm.Ok = cmd;

            vm.IsChanged
                .Should()
                .BeTrue();
        }
    }
}
