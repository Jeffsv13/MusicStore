using AutoMapper;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories.Abstractions;
using MusicStore.Services.Abstractions;

namespace MusicStore.Services.Implementations;

public class SaleService : ISaleService
{
    private readonly ISaleRepository repository;
    private readonly IConcertRepository concertRepository;
    private readonly ICustomerRepository customerRepository;
    private readonly IMapper mapper;
    private readonly ILogger<SaleService> logger;

    public SaleService(
        ISaleRepository repository,
        IConcertRepository concertRepository,
        ICustomerRepository customerRepository,
        IMapper mapper,
        ILogger<SaleService> logger)
    {
        this.repository = repository;
        this.concertRepository = concertRepository;
        this.customerRepository = customerRepository;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<BaseResponseGeneric<int>> AddAsync(string email, SaleRequestDto request)
    {
        var response = new BaseResponseGeneric<int>();
        
        try
        {
            await repository.CreateTransactionAsync();
            var entity = mapper.Map<Sale>(request);
            var customer = await customerRepository.GetByEmailAsync(email);
            if(customer == null)
            {
                throw new InvalidOperationException($"La cuenta {email} no esta registrada como cliente");
                //customer = new Customer
                //{
                //    Email = request.Email,
                //    FullName = request.FullName
                //};
                //customer.Id = await customerRepository.AddAsync(customer);
            }

            entity.CustomerId = customer.Id;

            var concert = await concertRepository.GetAsync(request.ConcertId);
            if (concert == null)
                throw new Exception($"El concierto con el Id {request.ConcertId} no existe.");
            
            if(DateTime.Today >= concert.DateEvent)
                throw new InvalidOperationException($"No se puede comprar tickets para un concierto {concert.Title} porque ya pasó.");
            
            if(concert.Finalized)
                throw new Exception($"El concierto con el id {request.ConcertId} ya finalizó.");

            entity.Total = entity.Quantity * (decimal)concert.UnitPrice;

            await repository.AddAsync(entity);
            await repository.UpdateAsync();

            response.Data = entity.Id;
            response.Success = true;

            logger.LogInformation("Se creó correctamente la venta para {email}",email);
        }
        catch (InvalidOperationException ex)
        {
            await repository.RollBackAsync();
            response.ErrorMessage = "Seleccionó datos invalidos para su compra";

            logger.LogError(ex, "{ErrorMessage} {Message}", response.ErrorMessage,ex.Message);
        }
        catch (Exception ex)
        {
            await repository.RollBackAsync();
            response.ErrorMessage = "Error al crear la venta.";
            logger.LogError(ex, "{ErrorMessage} {Message}", response.ErrorMessage,ex.Message);
        }

        return response;
    }

    public async Task<BaseResponseGeneric<SaleResponseDto>> GetAsync(int id)
    {
        var response = new BaseResponseGeneric<SaleResponseDto>();
        try
        {
            var sale = await repository.GetAsync(id);
            response.Data = mapper.Map<SaleResponseDto>(sale);
            response.Success = response.Data != null;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al seleccionar la venta.";
            logger.LogError(ex, "{ErrorMessage} {Message}", response.ErrorMessage, ex.Message);
        }
        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<SaleResponseDto>>> GetAsync(SaleByDateSearchDto search, PaginationDto pagination)
    {
        var response = new BaseResponseGeneric<ICollection<SaleResponseDto>>();
        try
        {
            var dateInit = Convert.ToDateTime(search.DateStart);
            var dateEnd = Convert.ToDateTime(search.DateEnd);

            var data = await repository.GetAsync(
                predicate: s => s.SaleDate >= dateInit && s.SaleDate <= dateEnd,
                orderBy: x => x.OperationNumber,
                pagination);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al filtrar las ventas por fechas.";
            logger.LogError(ex, "{ErrorMessage} {Message}", response.ErrorMessage, ex.Message);
        }
        return response;
    }
    public async Task<BaseResponseGeneric<ICollection<SaleResponseDto>>> GetAsync(string email, string? title, PaginationDto pagination)
    {
        var response = new BaseResponseGeneric<ICollection<SaleResponseDto>>();
        try
        {
            var data = await repository.GetAsync(
                predicate: s => s.Customer.Email == email && s.Concert.Title.Contains(title ?? string.Empty),
                orderBy: x => x.SaleDate,
                pagination);
            response.Data = mapper.Map<ICollection<SaleResponseDto>>(data);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al filtrar las ventas del usuario por titulo.";
            logger.LogError(ex, "{ErrorMessage} {Message}", response.ErrorMessage, ex.Message);
        }
        return response;
    }
}
