using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfMessageDal : EfEntityRepositoryBase<RealTimeChatAppContext, Message>, IMessageDal
    {
        public EfMessageDal(RealTimeChatAppContext _context) : base(_context)
        {
        }
    }
}
