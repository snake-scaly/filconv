using System.Collections.Generic;

namespace FilConvWpf.UI
{
    public interface IMultiChoice<TChoice> : ITool
    {
        IEnumerable<TChoice> Choices { get; set; }
        TChoice CurrentChoice { get; set; }
    }
}
