using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework.Contexts
{
    public class RealTimeChatAppContext : DbContext
    {
        public RealTimeChatAppContext(DbContextOptions<RealTimeChatAppContext> options) : base(options)
        {
        }

        // DbSet properties for your entities go here
        // Those are not ready yet, so they are commented out for now.
    }
}
