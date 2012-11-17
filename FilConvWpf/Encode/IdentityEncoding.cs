﻿using System;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
    
namespace FilConvWpf.Encode
{
    class IdentityEncoding : IEncoding
    {
        public event EventHandler<EventArgs> EncodingChanged
        {
            add { }
            remove { }
        }

        public string Name { get { return "Оригинал"; } }

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(original, 1);
        }

        public void GrantToolbarFragment(ToolbarFragment fragment)
        {
        }

        public void RevokeToolbarFragment()
        {
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
