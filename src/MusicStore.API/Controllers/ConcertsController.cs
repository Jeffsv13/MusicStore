using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Entities;
using MusicStore.Services.Abstractions;

namespace MusicStore.API.Controllers;
[ApiController]
[Route("api/concerts")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleAdmin)]
public class ConcertsController : ControllerBase
{
    private readonly IConcertService service;

    public ConcertsController(
        IConcertService service)
    {
        this.service = service;
    }

    [HttpGet("title")]
    [AllowAnonymous]
    public async Task<ActionResult> Get(string? title, [FromQuery]PaginationDto pagination)
    {
        var response = await service.GetAsync(title, pagination);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult> Get(int id)
    {
        var response = await service.GetAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromForm]ConcertRequestDto request)
    {
        var response = await service.AddAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id,[FromForm] ConcertRequestDto request)
    {
        var response = await service.UpdateAsync(id, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var response = await service.DeleteAsync(id);
        return Ok(response);
    }
    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id)
    {
        return Ok(await service.FinalizeAsync(id));
    }
}
