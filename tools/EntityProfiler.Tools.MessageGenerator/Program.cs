namespace EntityProfiler.Tools.MessageGenerator {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;

    internal class Program {
        private static void Main() {
            Console.WriteLine("Initializing...");

            Database.SetInitializer(new AppDbContext.Initializer());

            using (new AppDbContext()) {
                // nop
            }

            Console.WriteLine("Initialized");
            Console.WriteLine();

            while (true) {
                Console.WriteLine("[S]elect / Select [N]+1 / [A]dd / [D]elete");

                var k = Console.ReadKey().KeyChar;
                switch (Char.ToLower(k)) {
                    case 's':
                        Select();
                        break;

                    case 'n':
                        SelectN1();
                        break;

                    case 'a':
                        Add();
                        break;

                    case 'd':
                        Delete();
                        break;
                }

                if (k == 'q') {
                    break;
                }
            }
        }

        private static void Delete() {
            using (AppDbContext dbContext = new AppDbContext()) {
                dbContext.Products.RemoveRange(dbContext.Products.Take(10).AsEnumerable());
            }
        }

        private static void Add() {
            using (AppDbContext dbContext = new AppDbContext()) {
                new AppDbContext.Initializer().AddItems(dbContext);
            }
        }

        private static void SelectN1() {
            using (AppDbContext dbContext = new AppDbContext()) {
                foreach (Product product in dbContext.Products) {
                    Console.Write("p");

                    foreach (Price price in product.Prices) {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine();

        }

        private static void Select() {
            using (AppDbContext dbContext = new AppDbContext()) {
                foreach (Product product in dbContext.Products.Include(x => x.Prices)) {
                    Console.Write("p");

                    foreach (Price price in product.Prices) {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine();
        }
    }


    internal sealed class AppDbContext : DbContext {

        public DbSet<Product> Products { get; set; } 
        public DbSet<Price> Prices { get; set; } 

        internal sealed class Initializer : DropCreateDatabaseAlways<AppDbContext> {
            /// <summary>
            /// A method that should be overridden to actually add data to the context for seeding.
            ///             The default implementation does nothing.
            /// </summary>
            /// <param name="context">The context to seed. </param>
            protected override void Seed(AppDbContext context) {
                int offset = context.Products.Count();
                for (int i = 0; i < 15; i++) {
                    Product p = new Product("Product #" + (offset + i));

                    for (int j = 0; j < 10; j++) {
                        p.Prices.Add(new Price(j * 423));
                    }

                    context.Products.Add(p);
                }

                context.SaveChanges();
            }

            public void AddItems(AppDbContext context) {
                this.Seed(context);
            }
        }
    }

    internal class Product {
        private ICollection<Price> _prices;

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Price> Prices {
            get { return this._prices ?? (this._prices = new Collection<Price>()); }
            set { this._prices = value; }
        }

        public Product() {}

        public Product(string name) {
            this.Name = name;
        }
    }

    internal class Price {

        public int Id { get; set; }

        [Required]
        public virtual Product Product { get; set; }


        public decimal Value { get; set; }

        public Price() {}

        public Price(decimal value) {
            this.Value = value;
        }
    } 
}