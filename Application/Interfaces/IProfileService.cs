namespace ICorteApi.Application.Interfaces;

public interface IProfileService : IService<Profile>
{
    Task<ProfileDtoResponse> CreateAsync(ProfileDtoCreate dto, int userId);
    Task<ProfileDtoResponse> GetByIdAsync(int id, int userId);
    Task<bool> UpdateAsync(ProfileDtoUpdate dto, int id, int userId);
}
