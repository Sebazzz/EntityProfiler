namespace EntityProfiler.Tests.Integration.EF.Support {
    using System.Data.Entity;

    internal class TestDbContext : DbContext {

        public DbSet<SomeEntity> TestEntities { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TestDbContext() {}


        public static TestDbContext CreateReset() {
            TestDbContext dbContext = new TestDbContext();

            dbContext.Database.Delete();

            dbContext.Database.CreateIfNotExists();

            dbContext.TestEntities.Add(SomeEntity.CreateNew());
            dbContext.TestEntities.Add(SomeEntity.CreateNew());
            dbContext.SaveChanges();

            return dbContext;
        }
    }
}