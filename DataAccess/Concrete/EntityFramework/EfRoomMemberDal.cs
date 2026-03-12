using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfRoomMemberDal : EfEntityRepositoryBase<RealTimeChatAppContext, RoomMember>, IRoomMemberDal
    {
        public EfRoomMemberDal(RealTimeChatAppContext context) : base(context)
        {
        }
    }
}
