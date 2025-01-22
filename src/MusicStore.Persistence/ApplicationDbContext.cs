using Microsoft.EntityFrameworkCore;
using MusicStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    //Fluent API
    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Customizing the migration...
        modelBuilder.Entity<Genre>().Property(x => x.Name).HasMaxLength(50);
    }

    //Entities to tables
    public DbSet<Genre> Genres { get; set; }
}
