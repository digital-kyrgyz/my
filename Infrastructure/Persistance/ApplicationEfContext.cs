using System.Reflection;
using System.Security.Claims;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Persistance;

public class ApplicationEfContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationEfContext(DbContextOptions<ApplicationEfContext> options) : base(options)
    {
        
    }
    public override Task<int> SaveChangesAsync(CancellationToken token = new())
    {
        Guid userId = Guid.Empty;
        var httpContextAccessor = this.GetService<IHttpContextAccessor>();
        string userIdVal = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userIdVal))
            userId = new Guid(userIdVal);

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.Created = DateTime.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastModified = DateTime.Now;
                    break;
            }
        }
            
        return base.SaveChangesAsync(token);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}