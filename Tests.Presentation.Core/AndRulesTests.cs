using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Presentation.Core;

namespace Tests.Presentation.Core
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class AndRulesTests
    {
        [Test]
        public void PostInvoke_IfBothPropertyChangesAreRaisedThenInvokeRules()
        {
            var propertyChainRule = new PropertyChainRule(new[] { "Age" });

            var rules = new AndRules();
            rules.Add(propertyChainRule, "Name");
            rules.Add(propertyChainRule, "Address");

            // model a Name change 
            var vm = new MyViewModel();
            var nl = new NotifyPropertyChangedListener(vm);

            Assert.IsFalse(rules.PostInvoke<MyViewModel>(vm, "Address"));
            Assert.IsTrue(rules.PostInvoke<MyViewModel>(vm, "Name"));
            Assert.IsTrue(nl.Changed.Contains("Age"));
        }

        [Test]
        public void PostInvoke_IfBothPropertyChangesAreRaisedThenInvokeRules_OrderDoesNotMatter()
        {
            var propertyChainRule = new PropertyChainRule(new[] { "Age" });

            var rules = new AndRules();
            rules.Add(propertyChainRule, "Name");
            rules.Add(propertyChainRule, "Address");

            // model a Name change 
            var vm = new MyViewModel();
            var nl = new NotifyPropertyChangedListener(vm);

            Assert.IsFalse(rules.PostInvoke<MyViewModel>(vm, "Name"));
            Assert.IsTrue(rules.PostInvoke<MyViewModel>(vm, "Address"));
            Assert.IsTrue(nl.Changed.Contains("Age"));
        }

        [Test]
        public void PostInvoke_IfNoneOfThePropertiesChangesAreRaisedNoRulesToRun()
        {
            var propertyChainRule = new PropertyChainRule(new[] {"Age"});

            var rules = new AndRules();
            rules.Add(propertyChainRule, "Name");
            rules.Add(propertyChainRule, "Address");

            // model a Name change 
            var vm = new MyViewModel();
            var nl = new NotifyPropertyChangedListener(vm);

            Assert.IsFalse(rules.PostInvoke<MyViewModel>(vm, "Name"));
            Assert.IsFalse(nl.Changed.Contains("Age"));
        }
    }
}