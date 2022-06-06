namespace GameApp.Data.Repositories
{
    public interface IRepository<TEntity>:IDisposable
        where TEntity : class
    {
        IQueryable<TEntity> All();
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task AddAsync(TEntity entity);
        Task SaveChangesAsync();
        void DeleteRange(IEnumerable<TEntity> entities);
    }
}
