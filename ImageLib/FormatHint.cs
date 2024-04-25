namespace ImageLib;

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
    public string? FileName { get; set; }

    public FormatHint(string fileName)
        : this()
    {
        FileName = fileName;
    }
}
