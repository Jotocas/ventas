namespace Ventas.Domain.Models
{
    using System.Data.Entity;
    using System.Linq;
    using Ventas.Common.Models;

    public class DataContext : DbContext
    {
        public DataContext() : base("DefaultConnection")
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}
