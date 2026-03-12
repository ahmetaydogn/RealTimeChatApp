using Entities.Concrete;

namespace Business.Abstract
{
    public interface IAppUserService
    {
        Task<IReadOnlyCollection<AppUser>> GetUsersAsync();
        Task<AppUser?> GetUserByIdAsync(Guid userId);
    }


}
