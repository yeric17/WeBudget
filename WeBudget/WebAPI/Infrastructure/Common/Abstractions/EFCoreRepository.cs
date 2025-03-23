namespace WebAPI.Infrastructure.Common.Abstractions
{
    public abstract class EFCoreRepository
    {
        protected readonly ApplicationDbContext _dbContext;
        protected EFCoreRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
