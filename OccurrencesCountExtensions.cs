namespace StringSearchingExtensions
{
    public static class OccurrencesCountExtensions
    {
        public static int BoyerMooreOccurrencesCount(string text, string pattern, bool ignoreCase = false)
        {
            Helpers.ValidateParameters(text, pattern);
            if (Helpers.CheckForZeroResult(text, pattern))
                return 0;

            if (ignoreCase)
            {
                text = text.ToLowerInvariant();
                pattern = pattern.ToLowerInvariant();
            }

            var textSpan = text.AsSpan();
            var patternSpan = pattern.AsSpan();

            Span<int> badCharacters = new int[char.MaxValue];
            badCharacters.Fill(patternSpan.Length);

            var lastPatternChar = patternSpan.Length - 1;

            for (int i = 0; i < lastPatternChar; ++i)
                badCharacters[patternSpan[i]] = lastPatternChar - i;

            int index = 0;
            int counter = 0;

            while (index <= (textSpan.Length - patternSpan.Length))
            {
                for (int i = lastPatternChar; textSpan[index + i] == patternSpan[i]; --i)
                {
                    if (i == 0)
                    {
                        counter++;
                        break;
                    }
                }

                index += badCharacters[textSpan[index + lastPatternChar]];
            }

            return counter;
        }
    }
}
