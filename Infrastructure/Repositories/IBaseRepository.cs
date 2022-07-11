namespace Infrastructure.Repositories
{
    /// <summary>
    /// Interface for the generic repository methods
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseRepository<T>
    {
        Task<T> CreateAsync(T obj);
        Task DeleteAsync(long id);
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(long id);
        Task<T> UpdateAsync(long id, T updatedObj);
    }
}