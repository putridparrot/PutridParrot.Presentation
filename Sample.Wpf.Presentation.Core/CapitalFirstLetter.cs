using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Wpf.Presentation.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class CapitalFirstLetter : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var s = value?.ToString();
            if (!String.IsNullOrEmpty(s))
            {
                return Char.IsUpper(s[0]);
            }

            return base.IsValid(value);
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, "The first letter of {0} must be capitalized", name);
        }
    }
}
