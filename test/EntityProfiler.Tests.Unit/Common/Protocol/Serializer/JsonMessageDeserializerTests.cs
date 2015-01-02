namespace EntityProfiler.Tests.Unit.Common.Protocol.Serializer {
    using System;
    using System.IO;
    using EntityProfiler.Common.Protocol.Serializer;
    using NUnit.Framework;

    [TestFixture]
    public class JsonMessageDeserializerTests {
        [Test]
        public void JsonMessageDeserializer_WhenGivenNullTypeResolver_ThrowsArgumentNullException() {
            // given
            IMessageTypeResolver resolver = null;
            TextReader textReader = new StringReader(String.Empty);

            // when
            ArgumentNullException exception =
                Assert.Throws<ArgumentNullException>(
                    () => new JsonMessageDeserializer(resolver, textReader));

            // then
            Assert.That(() => exception.ParamName, Is.EqualTo("typeResolver"));
        }

        [Test]
        public void JsonMessageDeserializer_WhenGivenNullTextWriter_ThrowsArgumentNullException() {
            // given
            IMessageTypeResolver resolver = new UnitTestMessageTypeResolver();
            TextReader textReader = null;

            // when
            ArgumentNullException exception =
                Assert.Throws<ArgumentNullException>(
                    () => new JsonMessageDeserializer(resolver, textReader));

            // then
            Assert.That(() => exception.ParamName, Is.EqualTo("textReader"));
        }
    }
}