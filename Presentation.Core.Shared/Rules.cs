using System;
using System.Collections.Generic;

namespace PutridParrot.Presentation
{
    /// <summary>
    /// A collection of rule objects associated with a property 
    /// name. Rules are themselves a rule so can be assigned 
    /// to other rules.
    /// </summary>
    public class Rules : Rule
    {
        private readonly Dictionary<string, IList<Rule>> _rules;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Rules()
        {
            _rules = new Dictionary<string, IList<Rule>>();
        }

        /// <summary>
        /// Add/associate a rule with a property. Multiple
        /// rules may be associated with each property
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="propertyName"></param>
        public void Add(Rule rule, string propertyName)
        {
            if (!_rules.ContainsKey(propertyName))
            {
                _rules[propertyName] = new List<Rule>();
            }

            var list = _rules[propertyName];
            list?.Add(rule);
        }

        private bool InvokeRules<T>(T viewModel, string propertyName, Func<Rule, T, string, bool> invokeFunc)
        {
            var allSucceeded = true;

            // if propertyName is null or empty we need to run all rules
            if (String.IsNullOrEmpty(propertyName))
            {
                foreach (var kv in _rules)
                {
                    allSucceeded &= InvokeRules(viewModel, kv.Key, invokeFunc);
                }
                return allSucceeded;
            }

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

        /// <summary>
        /// Called prior to invoking rules
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public override bool PreInvoke<T>(T viewModel, string propertyName)
        {
            return InvokeRules(viewModel, propertyName, (r, vm, pn) => r.PreInvoke(vm, pn));
        }

        /// <summary>
        /// Called after invoking rules
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public override bool PostInvoke<T>(T viewModel, string propertyName)
        {
            return InvokeRules(viewModel, propertyName, (r, vm, pn) => r.PostInvoke(vm, pn));
        }
    }

}
