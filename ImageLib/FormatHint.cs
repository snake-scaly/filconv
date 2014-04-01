using System;

namespace ImageLib
{
    /// <summary>
    /// Hints about possible <see cref="NativeImage"/> format.
    /// </summary>
    public struct FormatHint
    {
        /// <summary>
        /// Name of the file name this image is read from.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c> if this image wasn't read from a file.
        /// </remarks>
        public string FileName { get; set; }

        /// <summary>
        /// Concrete format of the image.
        /// </summary>
        /// <para>This property may contain <c>null</c> if the format is
        /// not known.</para>
        public NativeImageFormat NativeFormat { get; set; }

        public FormatHint(string fileName)
            : this()
        {
            FileName = fileName;
        }

        public FormatHint(NativeImageFormat format)
            : this()
        {
            NativeFormat = format;
        }

        /// <summary>
        /// Test whether the file name contains the given string.
        /// </summary>
        /// <remarks>
        /// The comparison is case-insensitive.
        /// </remarks>
        /// <param name="str">string to find</param>
        /// <returns><c>true</c> if the string is found, <c>false</c> if not found
        /// or if <c>FileName</c> is <c>null</c>.</returns>
        public bool FileNameContains(string str)
        {
            if (FileName == null)
                return false;
            return FileName.ToLower().Contains(str.ToLower());
        }
    }
}
