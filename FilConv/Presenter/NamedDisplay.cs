using ImageLib;

namespace FilConv.Presenter
{
    public class NamedDisplay : NamedChoice
    {
        public readonly NativeDisplay Display;

        public NamedDisplay(string name, NativeDisplay display)
            : base(name)
        {
            Display = display;
        }
    };
}
