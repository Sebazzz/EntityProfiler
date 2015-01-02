namespace EntityProfiler.Tests.Unit.Common.Protocol.Serializer {
    using EntityProfiler.Common.Protocol;

    public class FakeMessage : Message {
        public string Prop1 { get; set; }

        public int Prop2 { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public FakeMessage() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public FakeMessage(string prop1, int prop2) {
            this.Prop1 = prop1;
            this.Prop2 = prop2;
        }
    }
}