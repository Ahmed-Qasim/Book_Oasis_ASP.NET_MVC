using Book_Oasis.Models.Models;

namespace Book_Oasis.DataAccess.Repos.Interfaces
{
	public interface IProductRepository : IRepository<Product>
	{
		void Update(Product product);
	}
}
