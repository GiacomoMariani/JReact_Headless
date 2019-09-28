using System.Text.RegularExpressions;
using UnityEngine;

namespace JReact
{
    public static class J_Text_Extensions
    {
        private static readonly string[] _EndLines = new[] { "\r", "\r\n", "\n" };

        public static string ConvertString(this TextAsset textToConvert) => textToConvert.text;

        public static string[] SplitWith(this string stringToConvert, char splitWith) => stringToConvert.Split(splitWith);

        public static string RemoveSpace(this string stringToConvert) => stringToConvert.Replace(" ", "");

        public static string RemoveEmptyLines(this string stringToConvert)
            => Regex.Replace(stringToConvert, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);

        public static string RemoveEndLine(this string stringToConvert)
        {
            for (int i = 0; i < _EndLines.Length; i++) stringToConvert = stringToConvert.Replace(_EndLines[i], "");

            return stringToConvert;
        }

        public static string GetLine(this string stringToConvert, int lineIndex)
            => stringToConvert.Split(new[] { '\r', '\n' })[lineIndex];

        public static string GetFirstLine(this string stringToConvert) => GetLine(stringToConvert, 0);
    }
}
