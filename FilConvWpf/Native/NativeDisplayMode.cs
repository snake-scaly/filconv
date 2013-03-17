using System;
using ImageLib;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace FilConvWpf.Native
{
    class NativeDisplayMode : INativeDisplayMode
    {
        public event EventHandler<EventArgs> FormatChanged;

        public NativeDisplayMode(string name, NativeImageFormat format)
        {
            Name = name;
            Format = format;
        }

        public string Name { get; private set; }
        public double Aspect
        {
            get
            {
                return Format.Aspect;
            }
        }
        public NativeImageFormat Format { get; protected set; }
        public ToolBar ToolBar { get; protected set; }

        public virtual void StoreSettings(IDictionary<string, object> settings)
        {
        }

        public virtual void AdoptSettings(IDictionary<string, object> settings)
        {
        }

        protected virtual void OnFormatChanged()
        {
            if (FormatChanged != null)
            {
                FormatChanged(this, EventArgs.Empty);
            }
        }
    }
}
