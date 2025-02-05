using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicStore.Entities;
using MusicStore.Entities.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Persistence;

public class ApplicationDbContext : IdentityDbContext<MusicStoreUserIdentity>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    //Fluent API
    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Customizing the migration...
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Ignore<ConcertInfo>();
        //modelBuilder.Entity<ConcertInfo>().HasNoKey();

        modelBuilder.Entity<MusicStoreUserIdentity>(x => x.ToTable("User"));
        modelBuilder.Entity<IdentityRole>(x => x.ToTable("Role"));
        modelBuilder.Entity<IdentityUserRole<string>>(x => x.ToTable("UserRole"));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured)
            optionsBuilder.UseLazyLoadingProxies();
    }
    //Entities to tables
    //public DbSet<Genre> Genres { get; set; }
}
