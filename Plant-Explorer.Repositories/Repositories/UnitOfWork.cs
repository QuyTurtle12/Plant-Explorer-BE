using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Repositories.Base;

namespace Plant_Explorer.Repositories.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool disposed = false;
        private readonly PlantExplorerDbContext _dbContext;
        public UnitOfWork(PlantExplorerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            return new GenericRepository<T>(_dbContext);
        }
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void BeginTransaction()
        {
            _dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _dbContext.Database.CommitTransaction();
        }

        public void RollBack()
        {
            _dbContext.Database.RollbackTransaction();
        }
        public bool IsValid<T>(string id) where T : BaseEntity
        {
            var entity = GetRepository<T>().GetById(id);

            return (entity is not null && entity.DeletedBy is null);

        }
    }
}
