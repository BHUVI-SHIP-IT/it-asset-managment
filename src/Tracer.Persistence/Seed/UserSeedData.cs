using Microsoft.EntityFrameworkCore;
using Tracer.Domain.Entities;
using Tracer.Persistence.Contexts;

namespace Tracer.Persistence.Seed;

/// <summary>Dev-only sample users so checkout/assignee flows have people to pick.</summary>
public static class UserSeedData
{
    public static readonly Guid DefaultCompanyId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static readonly Guid ItAdminId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid AssetManagerId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid HelpDeskId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid EmployeeId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    /// <summary>BCrypt hash for password <c>User123!</c> ($2a$ compatible).</summary>
    private const string UserPasswordHash = "$2a$11$LQddNuaspTk1rDouBDPFeuYvIW2KhYiXNQSN53Np0DwSBMTCoIU5a";

    public static async Task EnsureSeededAsync(TracerDbContext db, CancellationToken cancellationToken = default)
    {
        var seeds = new[]
        {
            new ApplicationUser(ItAdminId)
            {
                FullName = "IT Administrator",
                Email = "itadmin@tracer.io",
                PasswordHash = UserPasswordHash,
                CompanyId = DefaultCompanyId,
                RoleId = 3, // ITAdmin
                IsActive = true
            },
            new ApplicationUser(AssetManagerId)
            {
                FullName = "Asset Manager",
                Email = "assetmgr@tracer.io",
                PasswordHash = UserPasswordHash,
                CompanyId = DefaultCompanyId,
                RoleId = 4, // AssetManager
                IsActive = true
            },
            new ApplicationUser(HelpDeskId)
            {
                FullName = "Help Desk Agent",
                Email = "helpdesk@tracer.io",
                PasswordHash = UserPasswordHash,
                CompanyId = DefaultCompanyId,
                RoleId = 10, // HelpDesk
                IsActive = true
            },
            new ApplicationUser(EmployeeId)
            {
                FullName = "Jane Employee",
                Email = "employee@tracer.io",
                PasswordHash = UserPasswordHash,
                CompanyId = DefaultCompanyId,
                RoleId = 11, // Employee
                IsActive = true
            }
        };

        foreach (var user in seeds)
        {
            var exists = await db.Users.AnyAsync(u => u.Id == user.Id || u.Email == user.Email, cancellationToken);
            if (!exists)
                db.Users.Add(user);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
