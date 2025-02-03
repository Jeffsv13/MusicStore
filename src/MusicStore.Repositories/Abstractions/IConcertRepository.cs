using MusicStore.Entities;
using MusicStore.Entities.Info;

namespace MusicStore.Repositories.Abstractions;

public interface IConcertRepository : IRepositoryBase<Concert>
{
    Task<ICollection<ConcertInfo>> GetAsync(string? title);
    Task FinalizeAsync(int id);
    Task<Concert?> GetAsyncById(int id);
}
