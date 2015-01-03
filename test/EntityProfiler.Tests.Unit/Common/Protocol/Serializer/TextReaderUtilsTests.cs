namespace EntityProfiler.Tests.Unit.Common.Protocol.Serializer {
    using System.IO;
    using EntityProfiler.Common.Protocol.Serializer;
    using NUnit.Framework;

    [TestFixture]
    public sealed class TextReaderUtilsTests {
        [TestCase("Lorem ipsum dolor si amet", "Lorem ipsum")]
        [TestCase("@#$!$!@#~!#DFEWR$$%@#%#E", "@#$!$!@#~")]
        [TestCase("Lorem ipsum dolor si amet", "Lorem")]
        public void TextReaderUtilsReadString_ReadString_Test(string streamContents, string stringToRead) {
            // given
            TextReader tr = new StringReader(streamContents);

            // when
            string result = TextReaderUtils.ReadString(tr, stringToRead.Length);

            // then
            Assert.That(() => result, Is.EqualTo(stringToRead));
        }

        [TestCase("Lorem ipsum dolor si amet", "Lorem ipsum", "Lorem ipsum")]
        [TestCase("Lorem ipsum dolor si amet", "ipsum", "Lorem ipsum")]
        [TestCase("Lorem ipsum dolor si amet", "amet", "Lorem ipsum dolor si amet")]
        [TestCase("Lorem ipsum dolor si amet", "Lorem", "Lorem")]
        public void TextReaderUtilsReadString_ReadToString_Test(string streamContents, string stringToRead, string expected) {
            // given
            TextReader tr = new StringReader(streamContents);

            // when
            string result = TextReaderUtils.ReadToString(stringToRead, tr);

            // then
            Assert.That(() => result, Is.EqualTo(expected));
        }
        
        [Test]
        public void TextReaderUtilsReadInteger_WhenGivenProperlyFormat_ReadsInteger() {
            // given
            const int num = 24253;
            TextReader tr = new StringReader(num.ToString(JsonMessageSerializer.Format.FormatString, JsonMessageSerializer.Format.FormatProvider));

            // when
            int result = TextReaderUtils.ReadInteger(tr);

            // then
            Assert.That(() => result, Is.EqualTo(num));
        }

        [Test]
        public void TextReaderUtilsReadInteger_WhenGivenErrorFormat_ThrowsMessageFormatException() {
            // given
            const int num = 24253;
            TextReader tr = new StringReader("0x"+num.ToString(JsonMessageSerializer.Format.FormatString, JsonMessageSerializer.Format.FormatProvider));

            // when / then
            Assert.Throws<MessageFormatException>(() => TextReaderUtils.ReadInteger(tr));
        }
    }
}