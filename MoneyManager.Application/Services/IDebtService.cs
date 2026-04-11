using MoneyManager.Application.Commands;
using MoneyManager.Application.DTOs;

namespace MoneyManager.Application.Services;

public interface IDebtService
{
    Task<DebtDto> CreateAsync(CreateDebtCommand cmd, CancellationToken ct = default);
    Task<List<DebtDto>> GetAllAsync(Guid userId, CancellationToken ct = default);
    Task MarkAsPaidAsync(Guid debtId, CancellationToken ct = default);
}
