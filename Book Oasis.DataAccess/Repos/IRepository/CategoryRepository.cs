using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models;

namespace Book_Oasis.DataAccess.Repos.IRepository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
	{
		private readonly ApplicationDbContext _context;
		public CategoryRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}



		public void Update(Category category)
		{
			_context.Categories.Update(category);
		}
	}

}
