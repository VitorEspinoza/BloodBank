using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodBank.Infrastructure.Persistence;

public class BloodBankDbContext : DbContext
{
    
    public BloodBankDbContext(DbContextOptions<BloodBankDbContext> options) : base(options)
    {
    }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<BloodDonor> BloodDonors { get; set; }
    public DbSet<BloodStock> BloodStocks { get; set; }
    public DbSet<Donation> Donations { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<ProcessedEvent> ProcessedEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        
        builder.Entity<Donation>()
            .Property(e => e.Id)
            .UseHiLo("donation_hilo",  "dbo");
        
        builder
            .Entity<Address>(e =>
            {
                e.HasKey(a => a.Id);
        
                e.HasIndex(a => new { a.ZipCode, a.Number, a.Complement })
                    .IsUnique()
                    .HasFilter("([Complement] IS NOT NULL)");
            
                e.HasIndex(a => new { a.ZipCode, a.Number })
                    .IsUnique()
                    .HasFilter("([Complement] IS NULL)");
                
                e.Property(a => a.Complement).IsRequired(false);
            });
        
        
      builder
            .Entity<BloodDonor>(e =>
            {
                e.HasKey(bd => bd.Id);

                e.HasIndex(bd => bd.Email)
                    .IsUnique();
                
                e.HasIndex(bd => bd.AddressId)
                    .IsUnique(false);


                e.Property(bd => bd.BiologicalSex)
                    .HasConversion(
                        g => g.ToString(),
                        s => Enum.Parse<BiologicalSex>(s))
                    .HasMaxLength(6);
                
                e.OwnsOne(bd => bd.BloodType, ConfigureBloodType);
                
                e.HasMany(bd => bd.Donations)
                    .WithOne(d => d.BloodDonor)
                    .HasForeignKey(d => d.BloodDonorId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                
                e.HasOne<Address>(bd => bd.Address)  
                    .WithMany()     
                    .HasForeignKey(bd => bd.AddressId)
                    .IsRequired()  
                    .OnDelete(DeleteBehavior.Restrict);  
            });
      
      builder
          .Entity<BloodStock>(e =>
          {
              e.OwnsOne(bd => bd.BloodType, ConfigureBloodType);
          });
      
      builder.Entity<OutboxMessage>(e =>
      {
          e.HasKey(m => m.Id);
          e.Property(m => m.Status)
              .HasConversion(
                  s => s.ToString(),
                  s => Enum.Parse<OutboxMessageStatus>(s))
              .HasMaxLength(20);
      });
      
      builder.Entity<ProcessedEvent>(e =>
      {
          e.HasKey(pe => new { pe.EventId, pe.ConsumerName });
      
          e.Property(pe => pe.EventType)
              .IsRequired(false);
      });
      
      base.OnModelCreating(builder);
    }
    
    private static void ConfigureBloodType<TEntity>(OwnedNavigationBuilder<TEntity, BloodType> builder) 
        where TEntity : class
    {
        builder.Property(bt => bt.Group)
            .HasColumnName("BloodGroup")
            .HasConversion(
                g => g.ToString(),
                s => Enum.Parse<BloodTypeGroup>(s))
            .HasMaxLength(2)
            .IsRequired();
                
        builder.Property(bt => bt.Rh)
            .HasColumnName("RhFactor")
            .HasConversion(
                r => r.ToString(),
                s => Enum.Parse<RhFactor>(s))
            .HasMaxLength(8)
            .IsRequired();
    }
    
}
