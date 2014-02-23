using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageLib.Apple
{
    interface AppleScreen
    {
        /// <summary>
        /// Get screen width, in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Get screen height, in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Get line width, in bytes.
        /// </summary>
        int ByteWidth { get; }

        /// <summary>
        /// Get image pixels in native format.
        /// </summary>
        byte[] Pixels { get; }

        /// <summary>
        /// Get byte offset of the given line.
        /// </summary>
        /// <param name="lineIndex">line index counting from the top of the screen</param>
        /// <returns>Offset of the line in the Pixels array.</returns>
        int GetLineOffset(int lineIndex);
    }
}
