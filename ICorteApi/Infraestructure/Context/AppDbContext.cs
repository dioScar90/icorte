using ICorteApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ICorteApi.Infraestructure.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    public DbSet<BarberShop> BarberShops { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<RecurringSchedule> RecurringSchedules { get; set; }
    public DbSet<SpecialSchedule> SpecialSchedules { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ServiceAppointment> ServiceAppointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public override int SaveChanges()
    {
        HandleSoftDelete();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleSoftDelete();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void HandleSoftDelete()
    {
        var deletedBarberShops = ChangeTracker.Entries<BarberShop>()
            .Where(e => e.State == EntityState.Modified && e.Entity.IsDeleted)
            .Select(e => e.Entity)
            .ToList();

        foreach (var barberShop in deletedBarberShops)
        {
            var recurringSchedules = RecurringSchedules
                .Where(os => os.BarberShopId == barberShop.Id)
                .ToList();

            RecurringSchedules.RemoveRange(recurringSchedules);
        }
    }
}
