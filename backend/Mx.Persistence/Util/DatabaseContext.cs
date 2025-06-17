using Mx.Persistence.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Mx.Persistence.Util;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public const string SchemaName = "Mx";
    
    public DbSet<Track> Tracks { get; set; }
    public DbSet<Motorcycle> Motorcycles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaName);

        ConfigureTrack(modelBuilder);
        ConfigureMotorcycle(modelBuilder);
        ConfigureUser(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Conventions.Remove<TableNameFromDbSetConvention>();
    }

    private static void ConfigureTrack(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<Track> track = modelBuilder.Entity<Track>();
        track.HasKey(t => t.Id);
        track.Property(t => t.Id).ValueGeneratedOnAdd();
        track.Property(t => t.Name).IsRequired();
        track.Property(t => t.LengthInKm).HasPrecision(10, 2);
        track.Property(t => t.Difficulty).IsRequired();
    }

    private static void ConfigureMotorcycle(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<Motorcycle> motorcycle = modelBuilder.Entity<Motorcycle>();
        motorcycle.HasKey(m => m.Id);
        motorcycle.Property(m => m.Id).ValueGeneratedOnAdd();
        motorcycle.Property(m => m.Model).IsRequired();
        motorcycle.Property(m => m.Number).IsRequired();
        motorcycle.Property(m => m.Horsepower).IsRequired();
        motorcycle.Property(m => m.IsRented).IsRequired();
        
        motorcycle.HasOne(m => m.Track)
                 .WithMany(t => t.AvailableMotorcycles)
                 .HasForeignKey(m => m.TrackId)
                 .OnDelete(DeleteBehavior.SetNull);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<User> user = modelBuilder.Entity<User>();
        user.HasKey(u => u.Id);
        user.Property(u => u.Id).ValueGeneratedOnAdd();
        user.Property(u => u.Name).IsRequired();
        user.Property(u => u.Age).IsRequired();
        user.Property(u => u.Weight).HasPrecision(10, 2).IsRequired();
        
        user.HasOne(u => u.RentedMotorcycle)
            .WithOne(m => m.RentedBy)
            .HasForeignKey<User>(u => u.RentedMotorcycleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
