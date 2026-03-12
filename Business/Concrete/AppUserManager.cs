using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.IdentityModel.Tokens;

namespace Business.Concrete
{
    public class AppUserManager : IAppUserService
    {
        private readonly IAppUserDal appUserDal;

        public AppUserManager(IAppUserDal _appUserDal)
        {
            appUserDal = _appUserDal;
        }

        public async Task<AppUser?> GetUserByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User Id cannot be empty.");
            }

            var user = await appUserDal.GetAsync(u => u.Id == userId);
            return user;
        }

        public async Task<IReadOnlyCollection<AppUser>> GetUsersAsync()
        {
            var users = await appUserDal.GetAllAsync();

            return users;
        }
    }
}
