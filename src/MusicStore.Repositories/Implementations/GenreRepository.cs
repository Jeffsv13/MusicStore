using Microsoft.EntityFrameworkCore;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Persistence;
using MusicStore.Repositories.Abstractions;

namespace MusicStore.Repositories.Implementations;
public class GenreRepository : IGenreRepository
{
    private readonly ApplicationDbContext _context;

    public GenreRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GenreResponseDto>> GetAsync()
    {
        var items = await _context.Genres.ToListAsync();

        //Mapping
        var genresResponseDto = items.Select(x => new GenreResponseDto
        {
            Id = x.Id,
            Name = x.Name,
            Status = x.Status
        }).ToList();

        return genresResponseDto;
    }

    public async Task<GenreResponseDto?> GetAsync(int id)
    {
        var item = await _context.Genres.FirstOrDefaultAsync(x => x.Id == id);
        var genreResponseDto = new GenreResponseDto();
        if (item is not null)
        {
            //Mapping
            genreResponseDto.Id = item.Id;
            genreResponseDto.Name = item.Name;
            genreResponseDto.Status = item.Status;
        }
        else
        {
            throw new InvalidOperationException($"No se encontró el género con el id {id}");
        }

        return genreResponseDto;
    }
    public async Task<int> AddAsync(GenreRequestDto genreRequestDto)
    {
        //Mapping
        var genre = new Genre()
        {
            Name = genreRequestDto.Name,
            Status = genreRequestDto.Status
        };

        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        return genre.Id;
    }
    public async Task UpdateAsync(int id, GenreRequestDto genreRequestDto)
    {
        var item = await _context.Genres.FirstOrDefaultAsync(x => x.Id == id);
        if (item is not null)
        {
            item.Name = genreRequestDto.Name;
            item.Status = genreRequestDto.Status;
            _context.Genres.Update(item);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException($"No se encontró el género con el id {id}");
        }
    }
    public async Task DeleteAsync(int id)
    {
        var item = await _context.Genres.FirstOrDefaultAsync(x => x.Id == id);
        if (item is not null)
        {
            _context.Remove(item);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException($"No se encontró el género con el id {id}");
        }
    }
}
