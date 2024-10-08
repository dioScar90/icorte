namespace ICorteApi.Application.Interfaces;

public interface IReportService : IService<Report>
{
    Task<ReportDtoResponse> CreateAsync(ReportDtoCreate dto, int clientId, int barberShopId);
    Task<ReportDtoResponse> GetByIdAsync(int id, int clientId, int barberShopId);
    Task<PaginationResponse<ReportDtoResponse>> GetAllAsync(int? page, int? pageSize, int barberShopId);
    Task<bool> UpdateAsync(ReportDtoUpdate dto, int id, int clientId, int barberShopId);
    Task<bool> DeleteAsync(int id, int clientId, int barberShopId);
}
