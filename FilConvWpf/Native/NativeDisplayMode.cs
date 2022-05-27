using System;
using System.Collections.Generic;
using FilConvWpf.UI;
using ImageLib;

namespace FilConvWpf.Native
{
    public class NativeDisplayMode : INativeDisplayMode
    {
        public event EventHandler<EventArgs> FormatChanged;

        public NativeDisplayMode(string name, INativeImageFormat format)
        {
            Name = name;
            Format = format;
        }

        public string Name { get; }
        public INativeImageFormat Format { get; protected set; }
        public IEnumerable<ITool> Tools { get; protected set; } = new ITool[] {};

        public virtual void StoreSettings(IDictionary<string, object> settings)
        {
        }

        public virtual void AdoptSettings(IDictionary<string, object> settings)
        {
        }

        protected virtual void OnFormatChanged()
        {
            FormatChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
