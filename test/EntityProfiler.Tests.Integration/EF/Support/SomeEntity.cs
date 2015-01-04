namespace EntityProfiler.Tests.Integration.EF.Support {
    using System;

    internal class SomeEntity {
        public int Id { get; set; }

        public string Name { get; set; }


        public static SomeEntity CreateNew() {
            return new SomeEntity() {
                                        Name = Guid.NewGuid().ToString("P")
                                    };
        }
    }
}