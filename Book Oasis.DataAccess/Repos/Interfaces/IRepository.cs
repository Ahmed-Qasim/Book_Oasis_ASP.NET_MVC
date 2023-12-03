using System.Linq.Expressions;

namespace Book_Oasis.DataAccess.Repos.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        T Get(Expression<Func<T, bool>> filter, string? includeProp = null, bool tracked = true);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProp = null);
        void Delete(T entity);
        void DeleteAll(IEnumerable<T> entites);
    }
}
