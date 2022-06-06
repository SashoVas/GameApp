using Microsoft.EntityFrameworkCore;

namespace GameApp.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        public Repository(GameAppDbContext context)
        {
            this.Context=context;
            this.DbSet=context.Set<TEntity>();
        }
        protected GameAppDbContext Context { get; set; }
        public DbSet<TEntity> DbSet { get; set; }
        public async Task AddAsync(TEntity entity)
        {
            await this.DbSet.AddAsync(entity).AsTask();
        }

        public IQueryable<TEntity> All()
        {
            return this.DbSet;
        }

        public void Delete(TEntity entity)
        {
            this.DbSet.Remove(entity);
        }

        public void Dispose()
        {
            this.Context.Dispose();
        }

        public async Task SaveChangesAsync()
        {
            await this.Context.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            this.DbSet.Update(entity);
        }
        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            this.DbSet.RemoveRange(entities);
        }
    }
}
