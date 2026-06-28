using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Infrastructure.Context
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
            { 
        }
       

        public DbSet<Item> Items { get; set; }
         public DbSet<Country> Countries { get; set; }
         public DbSet<Category> Categories { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DineinOrder> DineinOrders { get; set; }
        public DbSet<DeliveryOrder> DeliveryOrders { get; set; }
        public DbSet<TakeawayOrder> TakeawayOrders { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<PaymentRecord> PaymentRecords { get; set; }
        public DbSet<_DailyDeliverCounter> DailyDeliveryCounter { get; set; }
            public DbSet<_DailyDineinCounter> DailyDineinCounter { get; set; }

    }
}
