namespace EntityProfiler.Tests.Unit.Common.Protocol.Serializer {
    using System;
    using System.IO;
    using System.Text;
    using EntityProfiler.Common.Protocol.Serializer;
    using NUnit.Framework;

    [TestFixture]
    public class JsonMessageSerializerRoundTripTests {
        [TestCase("name", 1337)]
        [TestCase(null, 0)]
        public void JsonMessageSerializer_SimpleRoundTripTest(string name, int value) {
            // given
            StringBuilder data = new StringBuilder();
            FakeMessage message = new FakeMessage(name, value);

            // when
            JsonMessageSerializer serializer = CreateSerializer(data);
            serializer.SerializeMessage(message);

            JsonMessageDeserializer deserializer = CreateDeserializer(data);
            FakeMessage deserializedMessage = (FakeMessage) deserializer.DeserializeMessage();

            // then
            Assert.That(() => deserializedMessage.Prop1, Is.EqualTo(name));
            Assert.That(() => deserializedMessage.Prop2, Is.EqualTo(value));
        }

        [Test]
        public void JsonMessageSerializer_OnCorruptIncomingJson_ThrowsMessageException() {
            // given
            StringBuilder data = new StringBuilder();
            CreateSerializer(data).SerializeMessage(GenerateFakeMessage());
            data.Remove(data.Length/2, 10); // at a random position, remove some chars to corrupt the message

            // when / then
            Assert.Throws<MessageTransferException>(() =>
                CreateDeserializer(data).DeserializeMessage());
        }

        private static FakeMessage GenerateFakeMessage() {
            return new FakeMessage(DateTime.UtcNow.ToFileTimeUtc().ToString("x4"), (int) unchecked(DateTime.UtcNow.ToFileTime() % 1024));
        }

        private static JsonMessageSerializer CreateSerializer(StringBuilder stringBuilder) {
            return new JsonMessageSerializer(
                new UnitTestMessageTypeResolver(), 
                new StringWriter(stringBuilder));
        }

        private static JsonMessageDeserializer CreateDeserializer(StringBuilder stringBuilder) {
            return CreateDeserializer(stringBuilder.ToString());
        }

        private static JsonMessageDeserializer CreateDeserializer(string text) {
            return new JsonMessageDeserializer(
                new UnitTestMessageTypeResolver(), 
                new StringReader(text));
        }
    }
}
