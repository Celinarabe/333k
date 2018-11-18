using System;
using Rabe_Celina_HW6.Models;
using Microsoft.EntityFrameworkCore;


namespace Rabe_Celina_HW6.DAL
{
    public class AppDbContext : DbContext
    {
        //constructor that invokes the base constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }



        //create the db set
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<SupplierDetail> SupplierDetails { get; set; }

    }
}

