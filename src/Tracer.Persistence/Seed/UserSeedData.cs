using Microsoft.EntityFrameworkCore;
using Tracer.Domain.Entities;
using Tracer.Persistence.Contexts;

namespace Tracer.Persistence.Seed;

/// <summary>Idempotent sample users (8) with varied roles for assignee/checkout flows.</summary>
public static class UserSeedData
{
    public static readonly Guid DefaultCompanyId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static readonly Guid ItAdminId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid AssetManagerId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid HelpDeskId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid EmployeeId = Guid.Parse("55555555-5555-5555-5555-555555555555");
    public static readonly Guid DeptManagerId = Guid.Parse("66666666-6666-6666-6666-666666666666");
    public static readonly Guid FinanceOfficerId = Guid.Parse("77777777-7777-7777-7777-777777777777");
    public static readonly Guid InventoryManagerId = Guid.Parse("88888888-8888-8888-8888-888888888888");
    public static readonly Guid SalesRepId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    /// <summary>BCrypt hash for password <c>User123!</c> ($2a$ compatible).</summary>
    private const string UserPasswordHash = "$2a$11$LQddNuaspTk1rDouBDPFeuYvIW2KhYiXNQSN53Np0DwSBMTCoIU5a";

    public static async Task EnsureSeededAsync(TracerDbContext db, CancellationToken cancellationToken = default)
    {
        var seeds = new[]
        {
            new ApplicationUser(ItAdminId)
            {
                FullName = "Priya Nair",
                Email = "priya.nair@tracer.io",
                PasswordHash = UserPasswordHash,
                CompanyId = DefaultCompanyId,
                RoleId = 3, // ITAdmin
                IsActive = true
            },
            new ApplicationUser(AssetManagerId)
            {
                FullName = "Marcus Chen",
                Email = "marcus.chen@tracer.io",
                PasswordHash = UserPasswordHash,
                CompanyId = DefaultCompanyId,
                RoleId = 4, // AssetManager
                IsActive = true
            },
            new ApplicationUser(HelpDeskId)
            {
                FullName = "Sofia Alvarez",
                Email = "sofia.alvarez@tracer.io",
                PasswordHash = UserPasswordHash,
                CompanyId = DefaultCompanyId,
                RoleId = 10, // HelpDesk
                IsActive = true
            },
            new ApplicationUser(EmployeeId)
            {
                FullName = "Jordan Blake",
                Email = "jordan.blake@tracer.io",
                PasswordHash = UserPasswordHash,
                CompanyId = DefaultCompanyId,
                RoleId = 11, // Employee
                IsActive = true
            },
            new ApplicationUser(DeptManagerId)
            {
                FullName = "Amelia Brooks",
                Email = "amelia.brooks@tracer.io",
                PasswordHash = UserPasswordHash,
                CompanyId = DefaultCompanyId,
                RoleId = 7, // DepartmentManager
                IsActive = true
            },
            new ApplicationUser(FinanceOfficerId)
            {
                FullName = "Noah Patel",
                Email = "noah.patel@tracer.io",
                PasswordHash = UserPasswordHash,
                CompanyId = DefaultCompanyId,
                RoleId = 8, // FinanceOfficer
                IsActive = true
            },
            new ApplicationUser(InventoryManagerId)
            {
                FullName = "Elena Volkov",
                Email = "elena.volkov@tracer.io",
                PasswordHash = UserPasswordHash,
                CompanyId = DefaultCompanyId,
                RoleId = 5, // InventoryManager
                IsActive = true
            },
            new ApplicationUser(SalesRepId)
            {
                FullName = "Chris Okonkwo",
                Email = "chris.okonkwo@tracer.io",
                PasswordHash = UserPasswordHash,
                CompanyId = DefaultCompanyId,
                RoleId = 11, // Employee (sales floor)
                IsActive = true
            },
        };

        foreach (var user in seeds)
        {
            var existing = await db.Users
                .FirstOrDefaultAsync(u => u.Id == user.Id || u.Email == user.Email, cancellationToken);

            if (existing is null)
            {
                // Migrate legacy seeded emails onto the same fixed IDs when present.
                var legacyEmail = LegacyEmailFor(user.Id);
                if (legacyEmail is not null)
                {
                    existing = await db.Users.FirstOrDefaultAsync(u => u.Id == user.Id || u.Email == legacyEmail, cancellationToken);
                }
            }

            if (existing is null)
            {
                db.Users.Add(user);
                continue;
            }

            // Keep fixed IDs stable; refresh display fields on re-run without duplicating.
            existing.FullName = user.FullName;
            existing.Email = user.Email;
            existing.RoleId = user.RoleId;
            existing.IsActive = true;
            if (string.IsNullOrEmpty(existing.PasswordHash))
                existing.PasswordHash = UserPasswordHash;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static string? LegacyEmailFor(Guid id) => id switch
    {
        _ when id == ItAdminId => "itadmin@tracer.io",
        _ when id == AssetManagerId => "assetmgr@tracer.io",
        _ when id == HelpDeskId => "helpdesk@tracer.io",
        _ when id == EmployeeId => "employee@tracer.io",
        _ => null
    };
}
