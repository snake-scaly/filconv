namespace FilConvWpf.UI
{
    public interface IMultiChoice<TChoice> : ITool
    {
        TChoice CurrentChoice { get; set; }
    }
}
