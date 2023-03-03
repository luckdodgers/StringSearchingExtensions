namespace StringSearchingExtensions
{
    internal static class Helpers
    {
        static internal void ValidateParameters(string text, string pattern)
        {
            if (pattern is null)
                throw new ArgumentNullException($"{nameof(pattern)} is null");

            if (text is null)
                throw new ArgumentNullException($"{nameof(text)} is null");
        }

        static internal bool CheckForZeroResult(string text, string pattern)
        {
            if (text.Length == 0 || pattern.Length == 0 || pattern.Length > text.Length)
                return true;

            return false;
        }
    }
}
