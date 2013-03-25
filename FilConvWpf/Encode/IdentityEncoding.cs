﻿using System;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Controls;
    
namespace FilConvWpf.Encode
{
    class IdentityEncoding : IEncoding
    {
        public event EventHandler<EventArgs> EncodingChanged
        {
            add { }
            remove { }
        }

        public string Name { get { return "FormatNameOriginal"; } }

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(original, 1);
        }

        public ToolBar ToolBar
        {
            get
            {
                return null;
            }
        }

        public bool IsContainerSupported(Type type)
        {
            return false;
        }

        public void Encode(BitmapSource original, object container)
        {
            throw new NotSupportedException("Containers are not supported");
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
        }
    }
}