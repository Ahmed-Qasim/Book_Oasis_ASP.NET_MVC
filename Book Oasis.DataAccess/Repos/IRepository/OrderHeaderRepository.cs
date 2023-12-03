using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models.Models;

namespace Book_Oasis.DataAccess.Repos.IRepository
{
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext _context;
		public OrderHeaderRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}



		public void Update(OrderHeader orderHeader)
		{
			_context.OrderHeaders.Update(orderHeader);
		}
	}

}
