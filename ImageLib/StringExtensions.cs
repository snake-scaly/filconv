namespace ImageLib
{
    public static class StringExtensions
    {
        public static bool ContainsIgnoreCase(this string str, string fragment)
        {
            return str.ToLowerInvariant().Contains(fragment.ToLowerInvariant());
        }
    }
}
