using Book_Oasis.DataAccess.Repos.Interfaces;
using Product = Book_Oasis.Models.Models.Product;

namespace Book_Oasis.DataAccess.Repos.IRepository
{
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		private readonly ApplicationDbContext _context;
		public ProductRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}



		public void Update(Product product)

		{
			var objFromDb = _context.Products.FirstOrDefault(u => u.Id == product.Id);
			if (objFromDb != null)
			{
				objFromDb.Title = product.Title;
				objFromDb.ISBN = product.ISBN;
				objFromDb.Price = product.Price;
				objFromDb.Price50 = product.Price50;
				objFromDb.ListPrice = product.ListPrice;
				objFromDb.Price100 = product.Price100;
				objFromDb.Description = product.Description;
				objFromDb.CategoryId = product.CategoryId;
				objFromDb.Author = product.Author;
				//if (product.ImgUrl != null)
				//{
				//    objFromDb.ImgUrl = product.ImgUrl;
				//}
			}
		}


		//_context.Products.Update(objFromDb);
	}
}


