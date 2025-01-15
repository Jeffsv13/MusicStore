using Microsoft.AspNetCore.Mvc;
using MusicStore.Entities;
using MusicStore.Repositories;

namespace MusicStore.API.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenresController : ControllerBase
    {
        private readonly GenreRepository repository;

        public GenresController(GenreRepository repository)
        {
            this.repository = repository; 
        }

        [HttpGet]
        public ActionResult<List<Genre>> Get()
        {
            var data = repository.Get();
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Genre> Get(int id)
        {
            var item = repository.Get(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public ActionResult<Genre> Post(Genre genre)
        {
            repository.Add(genre);
            return Ok(genre);
        }
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Genre genre)
        {
            repository.Update(id, genre);
            return Ok();
        }
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            repository.Delete(id);
            return Ok();
        }
    }
}
