namespace EntityProfiler.Common.Protocol.Serializer {
    using System;
    using System.Text;
    using Annotations;

    internal static class StringBuilderExtensions {
        public static bool EndsWith([NotNull] this StringBuilder sb, [NotNull] string text) {
            if (sb == null) {
                throw new ArgumentNullException("sb");
            }
            if (text == null) {
                throw new ArgumentNullException("text");
            }

            if (sb.Length < text.Length) {
                return false;
            }

            for (int sbIndex = sb.Length - text.Length, tIndex = 0; sbIndex < sb.Length; sbIndex++) {
                char sbChar = sb[sbIndex];
                char tChar = text[tIndex];
                if (sbChar != tChar) {
                    return false;
                }

                tIndex++;
            }

            return true;
        }
    }
}