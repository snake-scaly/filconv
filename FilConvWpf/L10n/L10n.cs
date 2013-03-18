using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Resources;
using System.Reflection;

namespace FilConvWpf.L10n
{
    public class L10n : MarkupExtension
    {
        public L10n()
        {
        }

        public L10n(string key)
        {
            Key = key;
        }

        public string Key { get; set; }

        public string Default { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            object result = Properties.Resources.ResourceManager.GetObject(Key);
            if (result == null)
            {
                if (Default != null)
                {
                    result = Default;
                }
                else
                {
                    result = "#" + Key;
                }
            }
            return result;
        }
    }
}
