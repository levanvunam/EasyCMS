namespace Ez.Framework.Utilities
{
    /// <summary>
    /// Compare utilities
    /// </summary>
    public static class CompareUtilities
    {
        /// <summary>
        /// Check if two string equals
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreEqual(this string a, string b)
        {
            if (string.IsNullOrEmpty(a))
            {
                return string.IsNullOrEmpty(b);
            }

            return string.Equals(a, b);
        }
    }
}
