using System.Linq;

namespace EntityProfiler.Tools.MessageGenerator
{
    public static class Repository2
    {
        // Moved to teste stack trace file location

        internal static void Delete()
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                dbContext.Products.RemoveRange(dbContext.Products.Take(10).AsEnumerable());
                dbContext.SaveChanges();
            }
        }

        internal static void Add()
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                new AppDbContext.Initializer().AddItems(dbContext);
            }
        }
    }
}