namespace EntityProfiler.Tests.Unit.Common.Protocol.Serializer {
    using System;
    using System.IO;
    using EntityProfiler.Common.Protocol.Serializer;
    using NUnit.Framework;

    [TestFixture]
    public class JsonMessageSerializerTests {
        [Test]
        public void JsonMessageSerializer_WhenGivenNullTypeResolver_ThrowsArgumentNullException() {
            // given
            IMessageTypeResolver resolver = null;
            TextWriter textWriter = new StringWriter();

            // when
            ArgumentNullException exception =
                Assert.Throws<ArgumentNullException>(
                    () => new JsonMessageSerializer(resolver, textWriter));

            // then
            Assert.That(() => exception.ParamName, Is.EqualTo("typeResolver"));
        }

        [Test]
        public void JsonMessageSerializer_WhenGivenNullTextWriter_ThrowsArgumentNullException() {
            // given
            IMessageTypeResolver resolver = new UnitTestMessageTypeResolver();
            TextWriter textWriter = null;

            // when
            ArgumentNullException exception =
                Assert.Throws<ArgumentNullException>(
                    () => new JsonMessageSerializer(resolver, textWriter));

            // then
            Assert.That(() => exception.ParamName, Is.EqualTo("textWriter"));
        }

        [Test]
        public void JsonMessageSerializer_WhenGivenNullMessage_ThrowsArgumentNullException() {
            // given
            IMessageTypeResolver resolver = new UnitTestMessageTypeResolver();
            TextWriter textWriter = new StringWriter();

            // when
            ArgumentNullException exception =
                Assert.Throws<ArgumentNullException>(
                    () => new JsonMessageSerializer(resolver, textWriter).SerializeMessage(null));

            // then
            Assert.That(() => exception.ParamName, Is.EqualTo("message"));
        }

        [Test]
        public void JsonMessageSerializer_WhenUnderlyingMediumFails_ThrowsMessageException() {
            // given
            IMessageTypeResolver resolver = new UnitTestMessageTypeResolver();
            TextWriter textWriter = new StringWriter();
            textWriter.Dispose(); // so stringwriter will throw ObjectDisposedException

            // when / then
            Assert.Throws<MessageTransferException>(
                () => new JsonMessageSerializer(resolver, textWriter).SerializeMessage(new FakeMessage()));
        }
    }
}