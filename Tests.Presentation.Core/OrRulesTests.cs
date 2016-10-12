using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Presentation.Core;

namespace Tests.Presentation.Core
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class OrRulesTests
    {
        [Test]
        public void PostInvoke_IfOneOfThePropertiesExistsExpectAllRulesToRun()
        {
            var propertyChainRule = new PropertyChainRule(new[] {"Age"});

            var rules = new OrRules();
            rules.Add(propertyChainRule, "Name");
            rules.Add(propertyChainRule, "Address");

            // model a Name change 
            var vm = new MyViewModel();
            var nl = new NotifyPropertyChangedListener(vm);

            Assert.IsTrue(rules.PostInvoke<MyViewModel>(vm, "Name"));
            Assert.IsTrue(nl.Changed.Contains("Age"));
        }

        [Test]
        public void PostInvoke_IfNoneOfThePropertiesExistsExpectNoRulesToRun()
        {
            var propertyChainRule = new PropertyChainRule(new[] { "Age" });

            var rules = new OrRules();
            rules.Add(propertyChainRule, "Name");
            rules.Add(propertyChainRule, "Address");

            // model a Name change 
            var vm = new MyViewModel();
            var nl = new NotifyPropertyChangedListener(vm);

            Assert.IsFalse(rules.PostInvoke<MyViewModel>(vm, "IsChanged"));
            Assert.IsFalse(nl.Changed.Contains("Age"));
        }
    }
}
