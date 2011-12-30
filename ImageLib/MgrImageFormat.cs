﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageLib
{
    public class MgrImageFormat : C16ImageFormatAbstr
    {
        public override string Name
        {
            get { return "MGR"; }
        }

        public override int Width
        {
            get { return 128; }
        }

        public override int Height
        {
            get { return 128; }
        }
    }
}
