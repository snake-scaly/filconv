namespace FilConvWpf.Presenter
{
    public class NamedChoice
    {
        public readonly string Name;

        protected NamedChoice(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
