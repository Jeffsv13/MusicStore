using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MusicStore.Dto.Request;
using MusicStore.Entities;
using MusicStore.Entities.Info;
using MusicStore.Persistence;
using MusicStore.Repositories.Abstractions;
using MusicStore.Repositories.Utils;

namespace MusicStore.Repositories.Implementations;

public class ConcertRepository : RepositoryBase<Concert>, IConcertRepository
{
    private readonly IHttpContextAccessor httpContext;

    public ConcertRepository(ApplicationDbContext context, IHttpContextAccessor httpContext) : base(context)
    {
        this.httpContext = httpContext;
    }
    public override async Task<ICollection<Concert>> GetAsync()
    {
        //eager loading approach
        return await context.Set<Concert>()
            .Include(x=>x.Genre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Concert?> GetAsyncById(int id)
    {
        return await context.Set<Concert>()
            .Include(x => x.Genre)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<ICollection<ConcertInfo>> GetAsync(string? title, PaginationDto pagination)
    {
        //lazy loading approach
        var queryable = context.Set<Concert>()
            .Where(x => x.Title.Contains(title ?? string.Empty))
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Select(x => new ConcertInfo
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                ExtendedDescription = x.ExtendedDescription,
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
            .AsQueryable();

        await httpContext.HttpContext.InsertarPaginacionHeader(queryable);
        var response = await queryable.OrderBy(x => x.Id).Paginate(pagination).ToListAsync();
        return response;
    }

    public async Task FinalizeAsync(int id)
    {
        var entity = await GetAsync(id);
        if(entity is not null) {
            entity.Finalized = true;
            await UpdateAsync();
        }
    }
}
