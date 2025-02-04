using MusicStore.Dto.Response;
using MusicStore.Dto.Request;

namespace MusicStore.Services.Abstractions;

public interface ISaleService
{
    Task<BaseResponseGeneric<int>> AddAsync(string email, SaleRequestDto request);
    Task<BaseResponseGeneric<SaleResponseDto>> GetAsync(int id);
    Task<BaseResponseGeneric<ICollection<SaleResponseDto>>> GetAsync(SaleByDateSearchDto search, PaginationDto pagination);
    Task<BaseResponseGeneric<ICollection<SaleResponseDto>>> GetAsync(string email, string? title, PaginationDto pagination);
}
