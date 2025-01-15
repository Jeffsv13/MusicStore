using MusicStore.Entities;

namespace MusicStore.Repositories;
public class GenreRepository
{
    private readonly List<Genre> genreList;

    public GenreRepository()
    {
        /*genreList = new List<Genre>
        {
            new Genre { Id = 1, Name = "Rock", Status = true },
            new Genre { Id = 2, Name = "Pop", Status = true },
            new Genre { Id = 3, Name = "Jazz", Status = true },
            new Genre { Id = 4, Name = "Metal", Status = true },
            new Genre { Id = 5, Name = "Blues", Status = true },
            new Genre { Id = 6, Name = "Country", Status = true },
            new Genre { Id = 7, Name = "Electronic", Status = true },
            new Genre { Id = 8, Name = "Hip Hop", Status = true },
            new Genre { Id = 9, Name = "Classical", Status = true },
            new Genre { Id = 10, Name = "Reggae", Status = true }
        };*/
        genreList = new();
        genreList.Add(new Genre() { Id = 1, Name = "Salsa" });
        genreList.Add(new Genre() { Id = 2, Name = "Merengue" });
        genreList.Add(new Genre() { Id = 3, Name = "Rock" });
    }

    public List<Genre> Get()
    {
        return genreList;
    }

    public Genre? Get(int id)
    {
        return genreList.FirstOrDefault(x => x.Id == id);
    }
    public void Add(Genre genre)
    {
        var lastItem = genreList.MaxBy(x => x.Id);
        genre.Id = lastItem is null ? 1 : lastItem.Id + 1;
        genreList.Add(genre);
    }
    public void Update(int id, Genre genre)
    {
        var item = Get(id);
        if (item is not null)
        {
            item.Name = genre.Name;
            item.Status = genre.Status;
        }
    }
    public void Delete(int id)
    {
        var item = Get(id);
        if (item is not null)
        {
            genreList.Remove(item);
        }
    }
}
