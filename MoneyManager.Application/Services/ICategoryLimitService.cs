using MoneyManager.Application.Commands;
using MoneyManager.Application.DTOs;

namespace MoneyManager.Application.Services;

public interface ICategoryLimitService
{
    Task<CategoryLimitStateDto> CreateAsync(CreateCategoryLimitCommand cmd, CancellationToken ct = default);
    Task<List<CategoryLimitStateDto>> GetActiveStatesAsync(Guid userId, CancellationToken ct = default);
    Task ArchiveAsync(Guid limitId, CancellationToken ct = default);
}
