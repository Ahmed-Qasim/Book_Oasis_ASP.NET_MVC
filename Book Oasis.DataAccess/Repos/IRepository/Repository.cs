using Book_Oasis.DataAccess.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Book_Oasis.DataAccess.Repos.IRepository
{
    public class Repository<T> : IRepository<T> where T : class

    {

        private readonly ApplicationDbContext _context;
        internal DbSet<T> DbSet { get; set; }
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            this.DbSet = _context.Set<T>();
            _context.Products.Include(u => u.Category).ToList();

            _context.OrderHeaders.Include(u => u.ApplicationUser).ToList();

        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProp = null, bool tracked = true)
        {
            IQueryable<T> values;
            if (tracked)
            {
                values = DbSet;
            }
            else
            {
                values = DbSet.AsNoTracking();
            }
            values = values.Where(filter);
            if (!string.IsNullOrEmpty(includeProp))
            {
                foreach (var prop in includeProp
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    values.Include(prop).Load();
                }

            }
            return values.FirstOrDefault();

        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProp)
        {
            IQueryable<T> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProp))
            {
                foreach (var prop in includeProp
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop);
                }
            }
            return query.ToList();

        }
        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public void DeleteAll(IEnumerable<T> entites)
        {
            DbSet.RemoveRange(entites);
        }


    }
}
