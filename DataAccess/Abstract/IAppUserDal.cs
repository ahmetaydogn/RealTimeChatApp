using Core.DataAccess;
using Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IAppUserDal : IEntityRepository<AppUser>
    {
    }
}
