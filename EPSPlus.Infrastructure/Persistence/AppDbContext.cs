

using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EPSPlus.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }


    public DbSet<Member> Members { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Employer> Employers { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Contribution> Contributions { get; set; }
    public DbSet<BenefitEligibility> BenefitEligibilities { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(a => a.MemberDetails)
            .WithOne(m => m.User)
            .HasForeignKey<Member>(m => m.UserId);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(a => a.EmployerDetails)
            .WithOne(e => e.User)
            .HasForeignKey<Employer>(e => e.UserId);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(a => a.AdminDetails)
            .WithOne(e => e.User)
            .HasForeignKey<Admin>(e => e.UserId);

        modelBuilder.Entity<Employer>()
            .HasMany(e => e.Members)
            .WithOne(m => m.Employer)
            .HasForeignKey(m => m.EmployerId);

        // Contribution
        modelBuilder.Entity<Contribution>()
            .Property(c => c.Amount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Contribution>()
            .HasOne(c => c.Member)
            .WithMany(m => m.Contributions) 
            .HasForeignKey(c => c.MemberId);


        // BenefitEligibility
        modelBuilder.Entity<BenefitEligibility>()
            .HasOne(be => be.Member)
            .WithOne(m => m.BenefitEligibility)
            .HasForeignKey<BenefitEligibility>(be => be.MemberId);

        // Transaction
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Contribution)
            .WithMany(c => c.Transactions) // ✅ Corrected
            .HasForeignKey(t => t.ContributionId);

        // Seeding Application Users
        modelBuilder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = "user1", // Employer
                UserName = "techcorp_admin",
                IsActive = true,
                FullName = "techcorp_employer",
                PhoneNumber = "09023456789",
                NormalizedUserName = "TECHCORP_ADMIN",
                Email = "admin@techcorp.com",
                NormalizedEmail = "ADMIN@TECHCORP.COM",
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null, "Employer@123"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new ApplicationUser
            {
                Id = "user2", // Employer
                UserName = "innovate_admin",
                IsActive = true,
                FullName = "innovate_employer",
                PhoneNumber = "09023456789",
                NormalizedUserName = "INNOVATE_ADMIN",
                Email = "admin@innovate.com",
                NormalizedEmail = "ADMIN@INNOVATE.COM",
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null, "Employer@123"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new ApplicationUser
            {
                Id = "user3", // ✅ Member
                UserName = "member_one",
                IsActive = true,
                FullName = "member_one",
                PhoneNumber = "09023456789",
                NormalizedUserName = "MEMBER_ONE",
                Email = "member1@example.com",
                NormalizedEmail = "MEMBER1@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null, "Member@123"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new ApplicationUser
            {
                Id = "user4", // ✅ Member
                UserName = "member_two",
                FullName = "member_two",
                PhoneNumber = "08023456789",
                IsActive = true,
                NormalizedUserName = "MEMBER_TWO",
                Email = "member2@example.com",
                NormalizedEmail = "MEMBER2@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null, "Member@123"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );


        // Seeding Employers
        modelBuilder.Entity<Employer>().HasData(
            new Employer { Id = "1", UserId = "user1", CompanyName = "TechCorp", RegistrationNumber = "123456789", CreatedAt = DateTime.Now },
            new Employer { Id = "2", UserId = "user2", CompanyName = "Innovate Ltd", RegistrationNumber = "987654321", CreatedAt = DateTime.Now }
        );

        // Seeding Members
        modelBuilder.Entity<Member>().HasData(
            new Member { Id = "1", UserId = "user3", DateOfBirth = new DateTime(1990, 5, 20), Age = 34, CreatedAt = DateTime.Now, EmployerId = "1" },
            new Member { Id = "2", UserId = "user4", DateOfBirth = new DateTime(1985, 7, 15), Age = 39, CreatedAt = DateTime.Now, EmployerId = "2" }
        );

        // Seeding Contributions
        modelBuilder.Entity<Contribution>().HasData(
            new Contribution { Id = "1", MemberId = "1", ContributionType = ContributionStatus.Monthly, Amount = 100.00m, ContributionDate = DateTime.Now, Status = "Completed" },
            new Contribution { Id = "2", MemberId = "2", ContributionType = ContributionStatus.Voluntary, Amount = 200.00m, ContributionDate = DateTime.Now, Status = "Completed" }
        );

        // Seeding Transactions
        modelBuilder.Entity<Transaction>().HasData(
            new Transaction { Id = "1", ContributionId = "1", Status = TransactionStatus.Success, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            new Transaction { Id = "2", ContributionId = "2", Status = TransactionStatus.Pending, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
        );

        // Seeding BenefitEligibility
        modelBuilder.Entity<BenefitEligibility>().HasData(
            new BenefitEligibility { Id = "1", MemberId = "1", EligibleDate = DateTime.Now.AddYears(5), Status = true },
            new BenefitEligibility { Id = "2", MemberId = "2", EligibleDate = DateTime.Now.AddYears(3), Status = false }
        );


    }

}

