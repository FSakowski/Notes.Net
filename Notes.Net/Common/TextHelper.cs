namespace Notes.Net.Common
{
    public class TextHelper
    {
        public static string TrimLength(string s, int length = 30)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            if (s.Length <= length)
                return s;

            return s.Substring(0, length) + "...";
        }
    }
}
