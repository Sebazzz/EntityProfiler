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

            using (AppDbContext dbContext = new AppDbContext()) {
                // Ef is lazy and really only initializes on first query
                if (!dbContext.Products.Any()) dbContext.SaveChanges();
            }

            Console.WriteLine("Initialized");
            Console.WriteLine();

            while (true) {
                Console.Write("[S]elect / Select [N]+1 / [C]ount / [A]dd / [D]elete: _\b");

                var k = Char.ToLower(Console.ReadKey().KeyChar);
                Console.WriteLine();

                switch (k) {
                    case 's':
                        Repository1.Select();
                        break;

                    case 'n':
                        Repository1.SelectN1();
                        break;

                    case 'c':
                        Repository1.SelectCount();
                        break;

                    case 'a':
                        Repository2.Add();
                        break;

                    case 'd':
                        Repository2.Delete();
                        break;
                }

                if (k == 'q') {
                    break;
                }

                Console.WriteLine();
            }
        }
    }


    public class Product {
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

    public class Price {

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