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
		}

		public void Add(T entity)
		{
			DbSet.Add(entity);
		}

		public T Get(Expression<Func<T, bool>> filter, string? includeProp)
		{
			IQueryable<T> values = DbSet;
			values = values.Where(filter);
			if (!string.IsNullOrEmpty(includeProp))
			{
				foreach (var prop in includeProp
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					values.Include(prop);
				}

			}
			return values.FirstOrDefault();
		}

		public IEnumerable<T> GetAll(string? includeProp)
		{
			IQueryable<T> values = DbSet;

			return values.ToList();
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
