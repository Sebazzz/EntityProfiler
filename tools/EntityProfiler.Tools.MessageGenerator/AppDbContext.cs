using System.Data.Entity;
using System.Linq;

namespace EntityProfiler.Tools.MessageGenerator
{
    public sealed class AppDbContext : DbContext {

        public DbSet<Product> Products { get; set; } 
        public DbSet<Price> Prices { get; set; }

        /// <summary>
        /// Constructs a new context instance using conventions to create the name of the database to
        ///             which a connection will be made.  The by-convention name is the full name (namespace + class name)
        ///             of the derived context class.
        ///             See the class remarks for how this is used to create a connection.
        /// </summary>
        public AppDbContext() {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }

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
}