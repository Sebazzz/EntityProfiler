namespace EntityProfiler.Common.Protocol.Serializer {
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Annotations;

    /// <summary>
    /// Serializer which serializes messages using <c>SimpleJson</c>
    /// </summary>
    /// <remarks>
    /// Serialization format is below, no line breaks or whitespace unless mentioned
    /// Header:                `BEGIN`
    /// Length of type string: 00000123 (padded as specified in <see cref="Format.FormatString"/>)
    /// Type string:           EntityProfiler.Common.Protocol.SomeMessage
    /// Length of JSON string: 00000321 (padded as specified in <see cref="Format.FormatString"/>)
    /// JSON serialized msg:   { { ... } ... }
    /// Footer:                `END`
    /// </remarks>
    internal class JsonMessageSerializer : StringMessageSerializer {
        private readonly IMessageTypeResolver _typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public JsonMessageSerializer([NotNull] IMessageTypeResolver typeResolver, TextWriter textWriter) : base(textWriter) {
            if (typeResolver == null) {
                throw new ArgumentNullException("typeResolver");
            }

            this._typeResolver = typeResolver;
        }

        protected override void SerializeMessage(Message message, TextWriter textWriter) {
            // header
            textWriter.Write(Format.Header);

            // body
            StringBuilder bodyBuilder = new StringBuilder();
            this.AppendTypeToBody(message, bodyBuilder);
            AppendMessageToBody(message, bodyBuilder);

            textWriter.Write(bodyBuilder);

            // footer
            textWriter.Write(Format.Footer);

            textWriter.Flush();
        }

        private void AppendTypeToBody(Message message, StringBuilder bodyBuilder) {
            Type type = message.GetType();
            string typeRef = this._typeResolver.CreateTypeRef(type);

            bodyBuilder.Append(IntegerToString(typeRef.Length));
            bodyBuilder.Append(typeRef);
        }

        private static void AppendMessageToBody(Message message, StringBuilder bodyBuilder) {
            string json = SimpleJson.SerializeObject(message);

            bodyBuilder.Append(IntegerToString(json.Length));
            bodyBuilder.Append(json);
        }

        private static string IntegerToString(int @int) {
            return @int.ToString(Format.FormatString, Format.FormatProvider);
        }

        internal static class Format {
            public const string Header = "`BEGIN`";
            public const string Footer = "`END`";
            public const string FormatString = "00000000000000000000000";
            public static readonly IFormatProvider FormatProvider = CultureInfo.InvariantCulture;
        }
    }
}