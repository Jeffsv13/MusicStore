using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories.Abstractions;

namespace MusicStore.API.Controllers;
[ApiController]
[Route("api/concerts")]
public class ConcertsController : ControllerBase
{
    private readonly IConcertRepository repository;
    private readonly IGenreRepository genreRepository;
    private readonly ILogger<ConcertsController> logger;

    public ConcertsController(
        IConcertRepository repository,
        IGenreRepository genreRepository,
        ILogger<ConcertsController> logger)
    {
        this.repository = repository;
        this.genreRepository = genreRepository;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var concertsDb = await repository.GetAsync();
        return Ok(concertsDb);
    }
    [HttpGet("title")]
    public async Task<ActionResult> Get(string? title)
    {
        var response = new BaseResponseGeneric<ICollection<ConcertResponseDto>>();
        //try
        //{
        //    //Mapping
        //    var concertsDb = await repository.GetAsync(x => x.Title.Contains(title??string.Empty), x => x.DateEvent);

        //    var concertsDto = concertsDb.Select(x => new ConcertResponseDto
        //    {
        //        Id = x.Id,
        //        Title = x.Title,
        //        Description = x.Description,
        //        Place = x.Place,
        //        UnitPrice = x.UnitPrice,
        //        GenreId = x.GenreId,
        //        DateEvent = x.DateEvent,
        //        ImageUrl = x.ImageUrl,
        //        TicketsQuantity = x.TicketsQuantity,
        //        Finalized = x.Finalized
        //    }).ToList();

        //    response.Data = concertsDto;
        //    response.Success = true;
        //    logger.LogInformation("Obteniendo todos los conciertos");
        //    return Ok(response);
        //}
        //catch (Exception ex)
        //{
        //    response.ErrorMessage = "Ocurrió un error al obtener la información";
        //    logger.LogError(ex, $"{response.ErrorMessage} {ex.Message}");
        //    return BadRequest(response);
        //}
        var concerts = await repository.GetAsync(title);
        return Ok(concerts);
    }
    [HttpPost]
    public async Task<IActionResult> Post(ConcertRequestDto concertRequestDto)
    {
        var response = new BaseResponseGeneric<int>();
        try
        {
            //validating genre id
            var genre = await genreRepository.GetAsync(concertRequestDto.GenreId);
            if(genre is null)
            {
                response.ErrorMessage = $"El id del género {concertRequestDto.GenreId} es incorreecto";
                logger.LogWarning(response.ErrorMessage);
                return BadRequest(response);
            }
            //mapping
            var concertsDb = new Concert
            {
                Title = concertRequestDto.Title,
                Description = concertRequestDto.Description,
                Place = concertRequestDto.Place,
                UnitPrice = concertRequestDto.UnitPrice,
                GenreId = concertRequestDto.GenreId,
                DateEvent = concertRequestDto.DateEvent,
                ImageUrl = concertRequestDto.ImageUrl,
                TicketsQuantity = concertRequestDto.TicketsQuantity
            };
            response.Data = await repository.AddAsync(concertsDb);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Ocurrió un error al guardar la información";
            logger.LogError(ex, ex.Message);
        }
        return Ok(response);
    }
}
