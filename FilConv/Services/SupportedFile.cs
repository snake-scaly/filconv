using System.Collections.Generic;

namespace FilConv.Services;

public record struct SupportedFile(string Name, IReadOnlyList<string> Suffixes);
