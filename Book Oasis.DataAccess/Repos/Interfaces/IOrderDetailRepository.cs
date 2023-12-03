using Book_Oasis.Models.Models;

namespace Book_Oasis.DataAccess.Repos.Interfaces
{
	public interface IOrderDetailRepository : IRepository<OrderDetail>
	{

		void Update(OrderDetail orderDetail);
	}
}
