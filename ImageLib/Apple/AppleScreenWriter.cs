using System;

namespace ImageLib.Apple
{
    class AppleScreenWriter
    {
        private AppleScreen _appleScreen;
        private int _bytePos;

        public AppleScreenWriter(AppleScreen appleScreen)
        {
            _appleScreen = appleScreen;
            _bytePos = 0;
        }

        /// <summary>
        /// Set the internal pointer to the start of the given line.
        /// </summary>
        /// <param name="lineIndex">line index counting from the top of the screen</param>
        public void MoveToLine(int lineIndex)
        {
            if (lineIndex < 0 || lineIndex >= _appleScreen.Height)
            {
                throw new ArgumentException(string.Format("Line number must be within [0..{0}]: {1}", _appleScreen.Height - 1, lineIndex));
            }
            _bytePos = _appleScreen.GetLineOffset(lineIndex);
        }

        /// <summary>
        /// Put a byte at the current position.
        /// </summary>
        /// The internal pointer is advanced one byte forward after writing.
        /// <param name="b">byte to write</param>
        public void Write(int b)
        {
            _appleScreen.Pixels[_bytePos] = (byte)b;
            _bytePos++;
        }
    }
}
