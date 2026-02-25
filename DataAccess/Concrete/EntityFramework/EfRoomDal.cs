using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfRoomDal : EfEntityRepositoryBase<RealTimeChatAppContext, Room>, IRoomDal
    {
        public EfRoomDal(RealTimeChatAppContext _context) : base(_context)
        {
        }
    }
}
