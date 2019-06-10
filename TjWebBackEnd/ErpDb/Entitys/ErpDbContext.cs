using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ErpDb.Entitys.Auth;

namespace ErpDb.Entitys
{
    public class ErpDbContext : DbContext
    {
        public ErpDbContext(string conn) :base(conn){ }

        public ErpDbContext() : base(@"Data Source=SC-201810210901\SQLEXPRESS;Initial Catalog=ErpDb;Integrated Security=True")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }  
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Menu> Menus { get; set; }

        public   DbSet<TjCustomer> TjCustomers { get; set; }
    }
}