using Core.DataAccess;
using Core.Entities;

namespace DataAccess.Abstract
{
    public interface IRoomChatDal<T> : IEntityRepository<T>
        where T : class, IEntity, new()
    {
    }
}
