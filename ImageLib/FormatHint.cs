using System;

namespace ImageLib
{
    /// <summary>
    /// Hints about possible <see cref="NativeImage"/> format.
    /// </summary>
    public struct FormatHint
    {
        /// <summary>
        /// Name of the file this image is read from.
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
        public INativeImageFormat NativeFormat { get; set; }

        public FormatHint(string fileName)
            : this()
        {
            FileName = fileName;
        }

        public FormatHint(INativeImageFormat format)
            : this()
        {
            NativeFormat = format;
        }
    }
}
