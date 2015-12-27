using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MSPToolkit.Encodings
{
    /// <summary>
    /// Abstract class represents single-byte encoding.
    /// </summary>
    public abstract class BaseSingleByteEncoding : Encoding
    {
        /// <summary>
        /// String containing a set of chars which a being represented by 0-255 bytes in selected encoding.
        /// </summary>
        protected abstract string Alphabet { get; }

        /// <summary>
        /// The IANA name for the current System.Text.Encoding.
        /// </summary>
        public abstract override string WebName { get; }

        /// <summary>
        /// The code page identifier of the current System.Text.Encoding.
        /// </summary>
        public abstract int CodePage { get; }

        /// <summary>
        /// Helps to improve string->byte[] conversion speed
        /// </summary>
        private Dictionary<char, byte> ReverseLookupDictionary = null;

        /// <summary>
        /// Array of instances of all derived from this classes. Lazy-loading.
        /// </summary>
        private static BaseSingleByteEncoding[] Encodings;

        /// <summary>
        /// Loads encodings from this and calling assemblies.
        /// </summary>
        private static void LoadEncodings()
        {
            Encodings = Assembly.GetExecutingAssembly().GetExportedTypes()
                .Union(Assembly.GetCallingAssembly().GetTypes())
                .Where(t => t.IsSubclassOf(typeof(BaseSingleByteEncoding)))
                .Select(t => Activator.CreateInstance(t))
                .Cast<BaseSingleByteEncoding>()
                .ToArray();
        }

        /// <summary>
        /// Returns the encoding associated with the specified code page identifier.
        /// </summary>
        /// <param name="codePage">The code page identifier of the preferred encoding.</param>
        /// <returns>If found the System.Text.Encoding associated with the specified code page, otherwise null.</returns>
        public static Encoding GetEncoding(int codePage)
        {
            if (Encodings == null)
                LoadEncodings();

            return Encodings.Where(t => t.CodePage == codePage).FirstOrDefault();
        }

        /// <summary>
        /// Returns an encoding associated with the specified code page name.
        /// </summary>
        /// <param name="webName">The code page name of the preferred encoding. Any value returned by System.Text.Encoding.WebName is a valid input.</param>
        /// <returns>If found the System.Text.Encoding associated with the specified code page, otherwise null.</returns>
        public static new Encoding GetEncoding(string webName)
        {
            if (Encodings == null)
                LoadEncodings();

            return Encodings.Where(t => string.Compare(webName, t.WebName, StringComparison.InvariantCultureIgnoreCase) == 0).FirstOrDefault();
        }

        /// <summary>
        /// Builds ReverseLookupDictionary
        /// </summary>
        private void BuildReverseLookupDictionary()
        {
            char[] alphabetArray = Alphabet.ToCharArray();
            ReverseLookupDictionary = new Dictionary<char, byte>();
            for (int i = 0; i < alphabetArray.Length; i++)
                ReverseLookupDictionary.Add(alphabetArray[i], (byte)i);
        }

        /// <summary>
        /// Calculates the number of bytes produced by encoding all the characters in the specified character array.
        /// </summary>
        /// <param name="chars">The character array containing the characters to encode</param>
        /// <param name="index">The zero-based index of the first character to encode.</param>
        /// <param name="count">The number of characters to encode.</param>
        /// <returns>The number of bytes produced by encoding the specified characters.</returns>
        /// <exception cref="System.ArgumentNullException">chars is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">index or count is less than zero.-or- index and count do not denote a valid range in chars.</exception>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            if (chars == null)
                throw new ArgumentNullException("chars");
            if (index < 0 || (index >= chars.Length && count != 0))
                throw new ArgumentOutOfRangeException("index");
            if (count < 0 || count + index > chars.Length)
                throw new ArgumentOutOfRangeException("count");
            
            //I understand that there is no matter which values are currently provided in index and
            //count vars, but these checks can help to find and prevent mistakes in calling code

            return count;
        }

        /// <summary>
        /// Encodes a set of characters from the specified character array into a sequence of bytes.
        /// If a char in input array cannot be found in encoding character set it will be replaced with a question mark.
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode.</param>
        /// <param name="charIndex">The zero-based index of the first character to encode.</param>
        /// <param name="charCount">The number of characters to encode.</param>
        /// <param name="bytes">A byte array containing the results of encoding the specified set of characters.</param>
        /// <param name="byteIndex"> The zero-based index at which to start writing the resulting sequence of bytes.</param>
        /// <returns>The actual number of bytes written into bytes.</returns>
        /// <exception cref="System.ArgumentNullException">chars is null.-or- bytes is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">charIndex or charCount or byteIndex is less than zero.-or- charIndex and
        /// charCount do not denote a valid range in chars.-or- byteIndex is not a valid index in bytes.</exception>
        /// <exception cref="System.ArgumentException"> bytes does not have enough capacity from byteIndex to the end of the
        /// array to accommodate the resulting bytes.</exception>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if (chars == null)
                throw new ArgumentNullException("chars");
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            if (charIndex < 0 || (charIndex >= chars.Length && charCount > 0))
                throw new ArgumentOutOfRangeException("chars");
            if (charCount < 0 || charCount + charIndex > chars.Length)
                throw new ArgumentOutOfRangeException("charCount");
            if (byteIndex < 0 || (byteIndex >= bytes.Length && charCount > 0))
                throw new ArgumentOutOfRangeException("byteIndex");

            if (byteIndex + charCount > bytes.Length)
                throw new ArgumentException("Resulting bytes array does not have enough capacity from byteIndex to the end of the array to accommodate the resulting bytes.", "bytes");

            if (ReverseLookupDictionary == null)
                BuildReverseLookupDictionary();

            byte questionIndex = ReverseLookupDictionary.ContainsKey('?') ? ReverseLookupDictionary['?'] : (byte)32;    //32 is a space character in all widely-used encodings
            
            for (int i = 0; i < charCount; i++)
            {
                int toIndex = byteIndex + i;
                char charToEncode = chars[charIndex + i];
                int index = ReverseLookupDictionary.ContainsKey(charToEncode) ? ReverseLookupDictionary[charToEncode] : -1;
                if (index == -1)
                    bytes[toIndex] = questionIndex;
                else
                    bytes[toIndex] = (byte)index;
            }
            return charCount;
        }

        /// <summary>
        /// Calculates the number of characters produced by decoding a sequence of bytes from the specified byte array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="index">The zero-based index of the first byte to decode.</param>
        /// <param name="count">The number of bytes to decode.</param>
        /// <returns>The number of characters produced by decoding the specified sequence of bytes.</returns>
        /// <exception cref="System.ArgumentNullException">bytes is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"> index or count is less than zero.-or- index and count do not denote a valid range in bytes.</exception>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");
            if (index < 0 || (index >= bytes.Length && count > 0))
                throw new ArgumentOutOfRangeException("index");
            if (count < 0 || index + count > bytes.Length)
                throw new ArgumentOutOfRangeException("count");

            //I understand that there is no matter which values are currently provided in index and
            //count vars, but these checks can help to find and prevent mistakes in calling code

            return count;
        }

        /// <summary>
        /// Decodes a sequence of bytes from the specified byte array into the specified character array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="byteIndex">The zero-based index of the first byte to decode.</param>
        /// <param name="byteCount">The number of bytes to decode.</param>
        /// <param name="chars">The character array to contain the resulting set of characters.</param>
        /// <param name="charIndex">The zero-based index at which to start writing the resulting set of characters.</param>
        /// <returns>The actual number of characters written into chars.</returns>
        /// <exception cref="System.ArgumentNullException">bytes is null.-or- chars is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">byteIndex or byteCount or charIndex is less than zero.-or- byteindex and
        /// byteCount do not denote a valid range in bytes.-or- charIndex is not a valid index in chars.</exception>
        /// <exception cref="System.ArgumentException">chars does not have enough capacity from charIndex to the end of the array
        /// to accommodate the resulting characters.</exception>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");
            if (chars == null)
                throw new ArgumentNullException("chars");

            if (byteIndex < 0 || (byteIndex >= bytes.Length && byteCount > 0))
                throw new ArgumentOutOfRangeException("bytes");
            if (byteCount < 0 || byteIndex + byteCount > bytes.Length)
                throw new ArgumentOutOfRangeException("byteCount");

            if (charIndex < 0 || (charIndex >= chars.Length && byteCount > 0))
                throw new ArgumentOutOfRangeException("charIndex");

            if (charIndex + byteCount > chars.Length)
                throw new ArgumentException("Resulting chars array does not have enough capacity from charIndex to the end of the array to accommodate the resulting characters.", "chars");

            StringBuilder builder = new StringBuilder(byteCount);
            char[] alphabetArray = Alphabet.ToCharArray();
            for (int i = 0; i < byteCount; i++)
            {
                chars[i + charIndex] = alphabetArray[bytes[byteIndex + i]];
            }
            return byteCount;
        }

        /// <summary>
        /// Calculates the maximum number of bytes produced by encoding the specified number of characters.
        /// </summary>
        /// <param name="charCount">The number of characters to encode.</param>
        /// <returns>The maximum number of bytes produced by encoding the specified number of characters.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">charCount is less than zero.</exception>
        public override int GetMaxByteCount(int charCount)
        {
            if (charCount < 0)
                throw new ArgumentOutOfRangeException("charCount");

            return charCount;
        }

        /// <summary>
        /// Calculates the maximum number of characters produced by decoding the specified number of bytes.
        /// </summary>
        /// <param name="byteCount">The number of bytes to decode.</param>
        /// <returns>The maximum number of characters produced by decoding the specified number of bytes.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">byteCount is less than zero.</exception>
        public override int GetMaxCharCount(int byteCount)
        {
            if (byteCount < 0)
                throw new ArgumentOutOfRangeException("byteCount");

            return byteCount;
        }
    }
}
