using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearchingExtensions
{
    internal static class Helpers
    {
        static internal void ValidateParameters(string text, string pattern)
        {
            if (text is null)
                throw new ArgumentNullException("Source is null");

            if (pattern is null)
                throw new ArgumentNullException("Pattern is null");
        }

        static internal bool CheckForZeroResult(string text, string pattern)
        {
            if (text.Length == 0 || pattern.Length == 0 || pattern.Length > text.Length)
                return true;

            return false;
        }
    }
}
