using System.Collections.Generic;

namespace FilConv.UI;

public interface IMultiChoice<TChoice> : ITool
{
    IEnumerable<TChoice> Choices { get; set; }
    TChoice CurrentChoice { get; set; }
}
