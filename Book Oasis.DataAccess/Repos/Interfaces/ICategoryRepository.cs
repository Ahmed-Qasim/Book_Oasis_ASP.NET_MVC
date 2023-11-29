using Book_Oasis.Models;

namespace Book_Oasis.DataAccess.Repos.Interfaces
{
	public interface ICategoryRepository : IRepository<Category>
	{

		void Update(Category category);
	}
}
