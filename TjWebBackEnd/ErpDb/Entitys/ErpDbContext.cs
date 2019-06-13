using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.ModelConfiguration.Conventions;
using ErpDb.Entitys.Auth;

namespace ErpDb.Entitys
{
    public class ErpDbContext : DbContext
    {
        public ErpDbContext(string conn) :base(conn){ }

        public ErpDbContext() : base(@"Data Source = 193.112.34.108,50077;Initial Catalog = ErpDB;User ID = tjapp;Password=123QWEasd!;Connect Timeout = 30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False") { 
       //     this.Configuration.ProxyCreationEnabled = false;
          DbInterception.Add(new NLogCommandInterceptor());

            //    this.Database.Log =  Console.WriteLine;

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }  
        public DbSet<Icon> Icons { get; set; }  
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Menu> Menus { get; set; }
        
        public   DbSet<TjSku> TjSkus { get; set; }
        public   DbSet<TjCustomer> TjCustomers { get; set; }
    }
}