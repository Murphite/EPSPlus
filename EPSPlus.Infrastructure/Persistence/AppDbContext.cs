

using EPSPlus.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EPSPlus.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Member> Members { get; set; }
    public DbSet<Employer> Employers { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Contribution> Contributions { get; set; }
    public DbSet<BenefitEligibility> BenefitEligibilities { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(a => a.MemberDetails)
            .WithOne(m => m.User)
            .HasForeignKey<Member>(m => m.UserId);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(a => a.EmployerDetails)
            .WithOne(e => e.User)
            .HasForeignKey<Employer>(e => e.UserId);

        modelBuilder.Entity<Employer>()
            .HasMany(e => e.Members)
            .WithOne(m => m.Employer)
            .HasForeignKey(m => m.EmployerId);
    }
}
