using Book_Oasis.Models.Models;

namespace Book_Oasis.DataAccess.Repos.Interfaces
{
	public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{

		void Update(OrderHeader orderHeader);
	}
}
