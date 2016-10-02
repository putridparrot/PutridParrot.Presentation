using System;
using System.Collections.Generic;

namespace Presentation.Core
{
    public class Rules : Rule
    {
        private readonly Dictionary<string, IList<Rule>> _rules;

        public Rules()
        {
            _rules = new Dictionary<string, IList<Rule>>();
        }

        public void Add(Rule rule, string propertyName)
        {
            if (!_rules.ContainsKey(propertyName))
            {
                _rules[propertyName] = new List<Rule>();
            }

            var list = _rules[propertyName];
            if (list != null)
            {
                list.Add(rule);
            }
        }

        private bool InvokeRules<T>(T viewModel, string propertyName, Func<Rule, T, string, bool> invokeFunc)
        {
            bool allSucceeded = true;
            var list = _rules.ContainsKey(propertyName) ? _rules[propertyName] : null;
            if (list != null)
            {
                foreach (var rule in list)
                {
                    if (!invokeFunc(rule, viewModel, propertyName))
                        allSucceeded = false;
                }
            }
            return allSucceeded;
        }

        public override bool PreInvoke<T>(T viewModel, string propertyName)
        {
            return InvokeRules(viewModel, propertyName, (r, vm, pn) => r.PreInvoke(vm, pn));
        }

        public override bool PostInvoke<T>(T viewModel, string propertyName)
        {
            return InvokeRules(viewModel, propertyName, (r, vm, pn) => r.PostInvoke(vm, pn));
        }
    }
}