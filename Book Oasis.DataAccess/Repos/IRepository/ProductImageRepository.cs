using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models.Models;

namespace Book_Oasis.DataAccess.Repos.IRepository
{
	public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
	{
		private readonly ApplicationDbContext _context;
		public ProductImageRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}



		public void Update(ProductImage productImage)
		{
			_context.ProductImages.Update(productImage);
		}
	}

}
