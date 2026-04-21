using HotelBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Auth
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        // Hotel
        public DbSet<Hotel> Hotels => Set<Hotel>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<RoomCategory> RoomCategories => Set<RoomCategory>();
        public DbSet<Amenity> Amenities => Set<Amenity>();

        // Booking
        public DbSet<Booking> Bookings => Set<Booking>();

        // Promotions
        public DbSet<Promotion> Promotions => Set<Promotion>();
        public DbSet<DiscountCode> DiscountCodes => Set<DiscountCode>();
        public DbSet<LoyaltyReward> LoyaltyRewards => Set<LoyaltyReward>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role).HasDefaultValue("Customer");

            // Hotel <-> Amenity many-to-many
            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.Amenities)
                .WithMany(a => a.Hotels)
                .UsingEntity(j => j.ToTable("HotelAmenities"));

            // Room
            modelBuilder.Entity<Room>()
                .Property(r => r.PricePerNight).HasPrecision(18, 2);

            // Booking
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice).HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status).HasConversion<string>();

            // Promotion
            modelBuilder.Entity<Promotion>()
                .Property(p => p.DiscountPercentage).HasPrecision(5, 2);

            modelBuilder.Entity<DiscountCode>()
                .HasIndex(d => d.Code).IsUnique();
        }
    }
}
