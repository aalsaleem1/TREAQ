using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Traeq.Models;

namespace Traeq.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<OrderSupportThread> OrderSupportThreads { get; set; } = default!;
        public DbSet<OrderSupportMessage> OrderSupportMessages { get; set; } = default!;

        public DbSet<PharmacySupportThread> PharmacySupportThreads { get; set; } = default!;
        public DbSet<PharmacySupportMessage> PharmacySupportMessages { get; set; } = default!;
        public DbSet<PharmacyPromoCode> PharmacyPromoCodes { get; set; }

        public DbSet<Medicine> Medicines { get; set; } = default!;
        public DbSet<Cart> Carts { get; set; } = default!;
        public DbSet<PharmacyLegalInfo> PharmacyLegalInfos { get; set; } = default!;
        public DbSet<ContactUs> ContactUss { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!;
        public DbSet<UserAddress> UserAddresses { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasOne(u => u.EmployedAtPharmacy)
                .WithMany()
                .HasForeignKey(u => u.PharmacyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasOne(o => o.Pharmacy)
                .WithMany()
                .HasForeignKey(o => o.PharmacyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderSupportThread>()
                .HasOne(t => t.Order)
                .WithMany()
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderSupportThread>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrderSupportThread>()
                .HasOne(t => t.Admin)
                .WithMany()
                .HasForeignKey(t => t.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrderSupportThread>()
                .HasMany(t => t.Messages)
                .WithOne(m => m.Thread)
                .HasForeignKey(m => m.ThreadId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PharmacySupportThread>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PharmacySupportThread>()
                .HasOne(t => t.Pharmacy)
                .WithMany()
                .HasForeignKey(t => t.PharmacyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PharmacySupportThread>()
                .HasOne(t => t.Order)
                .WithMany()
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PharmacySupportThread>()
                .HasMany(t => t.Messages)
                .WithOne(m => m.Thread)
                .HasForeignKey(m => m.ThreadId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
