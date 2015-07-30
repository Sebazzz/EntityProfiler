using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace EntityProfiler.Tools.MessageGenerator
{
    public static class Repository1
    {
        // Moved to teste stack trace file location

        internal static void SelectCount()
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                Console.WriteLine(dbContext.Products.Count());
            }
        }
        
        internal static void SelectN1()
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                foreach (Product product in dbContext.Products.ToList())
                {
                    Console.Write("p");

                    foreach (Price price in product.Prices)
                    {
                        Console.Write(".");
                        Trace.Write(price.Value);
                    }
                }
            }

            Console.WriteLine();

        }

        internal static void Select()
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                foreach (Product product in dbContext.Products.Include(x => x.Prices))
                {
                    Console.Write("p");

                    foreach (Price price in product.Prices)
                    {
                        Console.Write(".");
                        Trace.Write(price.Value);
                    }
                }
            }

            Console.WriteLine();
        }
    }
}