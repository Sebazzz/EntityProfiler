namespace EntityProfiler.Common.Protocol.Serializer {
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    internal class TextReaderUtils {
        public static string ReadPrefixedString(TextReader textReader) {
            // number of character to read
            int stringLength = ReadInteger(textReader);

            // read string itself
            string theString = ReadString(textReader, stringLength);

            return theString;
        }

        public static int ReadInteger(TextReader textReader) {
            string rawInteger = ReadString(textReader, JsonMessageSerializer.Format.FormatString.Length);

            try {
                return Int32.Parse(rawInteger, NumberStyles.None, JsonMessageSerializer.Format.FormatProvider);
            }
            catch (FormatException ex) {
                throw new MessageFormatException(
                    String.Format("Unable to parse '{0}' as an integer", rawInteger), ex);
            }
        }

        public static string ReadString(TextReader textReader, int length) {
            char[] buffer = new char[length];
            int pointer = 0;

            while ((pointer += textReader.Read(buffer, pointer, buffer.Length - pointer)) != buffer.Length) ;

            return new string(buffer);
        }

        public static string ReadToString(string target, TextReader reader) {
            StringBuilder sb = new StringBuilder();

            while (!sb.EndsWith(target)) {
                int ch = reader.Read();
                if (ch == -1) {
                    continue;
                }

                sb.Append((char) ch);
            }

            return sb.ToString();
        }
    }
}