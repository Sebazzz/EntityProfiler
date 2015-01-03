namespace EntityProfiler.Tests.Unit.Common.Protocol.Serializer {
    using System;
    using System.Text;
    using EntityProfiler.Common.Protocol.Serializer;
    using NUnit.Framework;

    [TestFixture]
    public sealed class StringBuilderExtensionsTests {
        [TestCase("`END``", "`END`")]
        [TestCase("", " ")]
        public void StringBuilderExtensionEndsWith_OnCase_ReturnsFalse(string sbText, string test) {
            // given
            StringBuilder sb = new StringBuilder(sbText);

            // when
            bool result = sb.EndsWith(test);

            // then
            Assert.IsFalse(result);
        }

        [TestCase("`END`", "`END`")]
        [TestCase("efwfhsdofhosijdfoijsAABBC", "ABBC")]
        [TestCase("", "")]
        public void StringBuilderExtensionEndsWith_OnCase_ReturnsTrue(string sbText, string test) {
            // given
            StringBuilder sb = new StringBuilder(sbText);

            // when
            bool result = sb.EndsWith(test);

            // then
            Assert.IsTrue(result);
        }

        [Test]
        public void StringBuilderExtensionEndsWith_ThrowsArgumentNullException_OnNullStringBuilder() {
            // given
            StringBuilder sb = null;

            // when
            ArgumentNullException exception =
                Assert.Throws<ArgumentNullException>(() =>
                    sb.EndsWith(""));

            // then
            Assert.That(() => exception.ParamName, Is.EqualTo("sb"));
        }

        [Test]
        public void StringBuilderExtensionEndsWith_ThrowsArgumentNullException_OnNullArg() {
            // given
            StringBuilder sb = new StringBuilder();

            // when
            ArgumentNullException exception =
                Assert.Throws<ArgumentNullException>(() =>
                    sb.EndsWith(null));

            // then
            Assert.That(() => exception.ParamName, Is.EqualTo("text"));
        }
    }
}
