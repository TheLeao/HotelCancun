using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Implementation of generic repository methods. Should be inherited by the specific repository classes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly DatabaseContext databaseContext;

        public BaseRepository(DatabaseContext databaseContext)
            => this.databaseContext = databaseContext;

        /// <summary>
        /// Create new record of given entity
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<T> CreateAsync(T obj)
        {
            obj.CreatedAt = DateTime.Now;
            await databaseContext.AddAsync(obj);
            await databaseContext.SaveChangesAsync();
            return obj;
        }

        /// <summary>
        /// Deletes the entity from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(long id)
        {
            T? entity = await databaseContext.Set<T>().FindAsync(id);
            if (entity != null)
            {
                databaseContext.Set<T>().Remove(entity);
                await databaseContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Returns all entities of the specified set
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetAllAsync()
        {
            return await databaseContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            return await databaseContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id);
        }

        /// <summary>
        /// Update an existing entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedObj"></param>
        /// <returns></returns>
        public async Task<T> UpdateAsync(long id, T item)
        {
            item.ModifiedAt = DateTime.Now;
            await databaseContext.SaveChangesAsync();
            return item;
        }
    }
}
