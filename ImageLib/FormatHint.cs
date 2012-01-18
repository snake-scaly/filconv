using System;

namespace ImageLib
{
    /// <summary>
    /// Hints about possible <see cref="NativeImage"/> format.
    /// </summary>
    public struct FormatHint
    {
        /// <summary>
        /// Extension of the file name this image is read from.
        /// </summary>
        /// <para>This field may contain a file extension including
        /// the leading dot, <see cref="String.Empty"/> if the file did
        /// not have any extension, or <c>null</c> if this image wasn't
        /// read from a file.</para>
        public string FileExtension { get; set; }

        /// <summary>
        /// Concrete format of the image.
        /// </summary>
        /// <para>This property may contain <c>null</c> if the format is
        /// not known.</para>
        public NativeImageFormat NativeFormat { get; set; }

        public FormatHint(string extension)
            : this()
        {
            FileExtension = extension;
        }

        public FormatHint(NativeImageFormat format)
            : this()
        {
            NativeFormat = format;
        }
    }
}
