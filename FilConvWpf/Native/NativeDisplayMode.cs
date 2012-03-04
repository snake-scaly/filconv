using System;
using ImageLib;
using System.Windows;
using System.Collections.Generic;

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
        public NativeImageFormat Format { get; private set; }

        public void GrantToolbarFragment(ToolbarFragment fragment)
        {
        }

        public void RevokeToolbarFragment()
        {
        }
    }
}
