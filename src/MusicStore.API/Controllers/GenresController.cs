﻿using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories.Abstractions;
using System.Net;

namespace MusicStore.API.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreRepository repository;
        private readonly ILogger<GenresController> logger;

        public GenresController(
            IGenreRepository repository,
            ILogger<GenresController> logger)
        {
            this.repository = repository; 
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = new BaseResponseGeneric<List<GenreResponseDto>>();

            try
            {
                //mapping
                var genresDb = await repository.GetAsync();
                var genres = genresDb.Select(g => new GenreResponseDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Status = g.Status
                }).ToList();
                response.Data = genres;
                response.Success = true;
                logger.LogInformation("Obteniendo todos los generos musicales");
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Ocurrió un error al obtener la información";
                logger.LogError(ex, $"{response.ErrorMessage} {ex.Message}");
                return BadRequest(response);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = new BaseResponseGeneric<GenreResponseDto>();
            try
            {
                var genresDb = await repository.GetAsync(id);
                if(genresDb is null)
                {
                    logger.LogWarning($"No se encontró el genero musical con id {id}");
                    response.ErrorMessage = "No se encontró el registro";
                    return NotFound(response);
                }
                var genre = new GenreResponseDto
                {
                    Id = genresDb.Id,
                    Name = genresDb.Name,
                    Status = genresDb.Status
                };
                response.Data = genre;
                response.Success = true;
                logger.LogInformation($"Obteniendo el genero musical con id {id}");
                return response.Data == null ? NotFound(response) : Ok(response);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Ocurrió un error al obtener la información";
                logger.LogError(ex, $"{response.ErrorMessage} {ex.Message}");
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(GenreRequestDto genreRequestDto)
        {
            var response = new BaseResponseGeneric<int>();
            try
            {
                var genreDb =  new Genre()
                {
                    Name = genreRequestDto.Name,
                    Status = genreRequestDto.Status
                };
                var genreId = await repository.AddAsync(genreDb);
                response.Data = genreId;
                response.Success = true;
                logger.LogInformation($"Género músical insertado con id {genreId}");
                return StatusCode((int)HttpStatusCode.Created, response);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Ocurrió un error al insertar la información";
                logger.LogError(ex, $"{response.ErrorMessage} {ex.Message}");
                return BadRequest(response);
            }
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, GenreRequestDto genreRequestDto)
        {
            var response = new BaseResponse();
            try
            {
                var genreDb = await repository.GetAsync(id);
                if (genreDb is null)
                {
                    logger.LogWarning($"No se encontró el genero musical con id {id}");
                    response.ErrorMessage = "No se encontró el registro";
                    return NotFound(response);
                }
                genreDb.Name = genreRequestDto.Name;
                genreDb.Status = genreRequestDto.Status;
                await repository.UpdateAsync();
                response.Success = true;
                logger.LogInformation($"Género músical actualizado con id {id}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Ocurrió un error al actualizar la información";
                logger.LogError($"{response.ErrorMessage} {ex.Message}");
                return BadRequest(response);
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new BaseResponse();
            try
            {
                var genreDb = await repository.GetAsync(id);
                if (genreDb is null)
                {
                    logger.LogWarning($"No se encontró el genero musical con id {id}");
                    response.ErrorMessage = "No se encontró el registro";
                    return NotFound(response);
                }
                await repository.DeleteAsync(id);
                response.Success = true;
                logger.LogInformation($"Género músical eliminado con id {id}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Ocurrió un error al eliminar la información";
                logger.LogError($"{response.ErrorMessage} {ex.Message}");
                return BadRequest(response);
            }
        }
    }
}
