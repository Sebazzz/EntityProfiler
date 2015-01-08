namespace EntityProfiler.Tests.Unit.Common.Protocol {
    using EntityProfiler.Common.Protocol;
    using NUnit.Framework;

    [TestFixture]
    public sealed class VersionTests {
        [Test]
        public void VersionIsGreaterThan_ReturnsTrue_OnGreaterVersion() {
            // given
            Version left = new Version(1, 0);
            Version right = new Version(0, 9);

            // when
            bool result = left > right;

            // then
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void VersionIsGreaterThan_ReturnsFalse_OnEqualVersion() {
            // given
            Version left = new Version(1, 0);
            Version right = new Version(1, 0);

            // when
            bool result = left > right;

            // then
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void VersionIsGreaterThan_ReturnsFalse_OnSmallerVersion() {
            // given
            Version left = new Version(1, 0);
            Version right = new Version(0, 9);

            // when
            bool result = left < right;

            // then
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void VersionEqual_ReturnsFalse_OnNonEqualVersion() {
            // given
            Version left = new Version(1, 0);
            Version right = new Version(0, 9);

            // when
            bool result = left == right;

            // then
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void VersionEqual_ReturnsTrue_OnEqualVersion() {
            // given
            Version left = new Version(1, 0);
            Version right = new Version(1, 0);

            // when
            bool result = left == right;

            // then
            Assert.That(result, Is.EqualTo(true));
        }
    }
}