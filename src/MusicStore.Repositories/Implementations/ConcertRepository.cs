﻿using Microsoft.EntityFrameworkCore;
using MusicStore.Entities;
using MusicStore.Entities.Info;
using MusicStore.Persistence;
using MusicStore.Repositories.Abstractions;

namespace MusicStore.Repositories.Implementations;

public class ConcertRepository : RepositoryBase<Concert>, IConcertRepository
{
    public ConcertRepository(ApplicationDbContext context) : base(context)
    {
    }
    public override async Task<ICollection<Concert>> GetAsync()
    {
        //eager loading approach
        return await context.Set<Concert>()
            .Include(x=>x.Genre)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<ICollection<ConcertInfo>> GetAsync(string? title)
    {
        //eager loading approach optimizado
        return await context.Set<Concert>()
            .Include(x => x.Genre)
            .Where(x => x.Title.Contains(title ?? string.Empty))
            .AsNoTracking()
            .Select(x => new ConcertInfo
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Place = x.Place,
                UnitPrice = x.UnitPrice,
                GenreId = x.GenreId,
                Genre = x.Genre.Name,
                DateEvent = x.DateEvent.ToShortDateString(),
                TimeEvent = x.DateEvent.ToShortTimeString(),
                ImageUrl = x.ImageUrl,
                TicketsQuantity = x.TicketsQuantity,
                Finalized = x.Finalized,
                Status = x.Status ? "Activo" : "Inactivo"
            })
            .ToListAsync();
    }
}
