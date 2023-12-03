using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models.Models;

namespace Book_Oasis.DataAccess.Repos.IRepository
{
	public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
	{
		private readonly ApplicationDbContext _context;
		public OrderDetailRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}



		public void Update(OrderDetail orderDetail)
		{
			_context.OrderDetails.Update(orderDetail);
		}
	}

}
