using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfAppUserDal : EfEntityRepositoryBase<RealTimeChatAppContext, AppUser>, IAppUserDal
    {
        public EfAppUserDal(RealTimeChatAppContext _context) : base(_context)
        {
        }
    }
}
