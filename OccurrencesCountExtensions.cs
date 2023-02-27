using System.Collections.Concurrent;
using System.Text;

namespace StringSearchingExtensions
{
    public static class OccurrencesCountExtensions
    {
        public static int LinearOccurrencesCount(this string text, string pattern, bool ignoreCase = false)
        {
            Helpers.ValidateParameters(text, pattern);
            if (Helpers.CheckForZeroResult(text, pattern))
                return 0;

            if (ignoreCase)
            {
                text = text.ToLowerInvariant();
                pattern = pattern.ToLowerInvariant();
            }

            int counter = 0;

            ReadOnlySpan<char> textSpan = text.AsSpan();
            ReadOnlySpan<char> patternSpan = pattern.AsSpan();

            for (int i = 0; i <= textSpan.Length - patternSpan.Length; i++)
            {
                if (textSpan.Slice(i, patternSpan.Length).SequenceEqual(patternSpan))
                    counter++;
            }

            return counter;
        }

        public static int ConcurentOccurrencesCount(this string text, string pattern, bool ignoreCase = false, int maxDegreeOfParallelism = 8)
        {
            Helpers.ValidateParameters(text, pattern);
            if (Helpers.CheckForZeroResult(text, pattern))
                return 0;

            if (ignoreCase)
            {
                text = text.ToLowerInvariant();
                pattern = pattern.ToLowerInvariant();
            }

            int counter = 0;
            var partition = Partitioner.Create(0, text.Length);

            Parallel.ForEach(partition, (range, loopState) =>
            {
                ReadOnlySpan<char> textSpan = text.AsSpan();
                ReadOnlySpan<char> patternSpan = pattern.AsSpan();

                for (int i = range.Item1; i < range.Item2; i++)
                {
                    if (i < textSpan.Length - patternSpan.Length - 1 && textSpan.Slice(i, patternSpan.Length).SequenceEqual(patternSpan))
                        Interlocked.Increment(ref counter);
                }
            });

            return counter;
        }

        public static int BoyerMooreOccurrencesCount(this string text, string pattern, bool ignoreCase = false)
        {
            Helpers.ValidateParameters(text, pattern);
            if (Helpers.CheckForZeroResult(text, pattern))
                return 0;

            if (ignoreCase)
            {
                text = text.ToLowerInvariant();
                pattern = pattern.ToLowerInvariant();
            }

            const int BlockSize = 256;
            const int SquaredBlockSize = BlockSize * BlockSize;

            Span<byte> skipArray = new byte[SquaredBlockSize];
            skipArray.Fill((byte)pattern.Length);

            for (int i = 0; i < pattern.Length - 1; i++)
            {
                var patternChar = pattern[i];
                var value = (byte)(pattern.Length - i - 1);
                int blockIndex = patternChar / BlockSize;
                skipArray[blockIndex * BlockSize + patternChar % BlockSize] = value;
            }

            int index = 0;
            int counter = 0;

            while (index <= (text.Length - pattern.Length))
            {
                int j = pattern.Length - 1;

                while (j >= 0 && pattern[j] == text[index + j])
                    j--;

                if (j < 0)
                {
                    counter++;
                    index++;
                    continue;
                }

                index += Math.Max(skipArray[text[index + j] / SquaredBlockSize + text[index + j] % BlockSize] - pattern.Length + 1 + j, 1);
            }

            return counter;
        }

        public static int BoyerMooreOccurrencesCount_2(this string text, string pattern, bool ignoreCase = false)
        {
            Helpers.ValidateParameters(text, pattern);
            if (Helpers.CheckForZeroResult(text, pattern))
                return 0;

            if (ignoreCase)
            {
                text = text.ToLowerInvariant();
                pattern = pattern.ToLowerInvariant();
            }

            byte[] valueArray = Encoding.ASCII.GetBytes(text);
            byte[] patternArray = Encoding.ASCII.GetBytes(pattern);

            var valueLength = valueArray.LongLength;
            var patternLength = patternArray.LongLength;

            Span<long> skipArray = new long[256];
            skipArray.Fill(patternLength);

            for (short i = 0; i < 256; ++i)
                skipArray[i] = patternLength;

            long lastPatternByte = patternLength - 1;

            for (long i = 0; i < lastPatternByte; ++i)
                skipArray[patternArray[i]] = lastPatternByte - i;

            long index = 0;
            int counter = 0;

            while (index <= (valueLength - patternLength))
            {
                for (long i = lastPatternByte; valueArray[index + i] == patternArray[i]; --i)
                {
                    if (i == 0)
                    {
                        counter++;
                        break;
                    }
                }

                index += skipArray[valueArray[index + lastPatternByte]];
            }

            return counter;
        }
    }
}
