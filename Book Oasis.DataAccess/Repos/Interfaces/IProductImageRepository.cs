using Book_Oasis.Models.Models;

namespace Book_Oasis.DataAccess.Repos.Interfaces
{
	public interface IProductImageRepository : IRepository<ProductImage>
	{

		void Update(ProductImage productImage);
	}
}
