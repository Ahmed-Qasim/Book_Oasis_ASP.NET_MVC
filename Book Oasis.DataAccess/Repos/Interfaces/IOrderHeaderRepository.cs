using Book_Oasis.Models.Models;

namespace Book_Oasis.DataAccess.Repos.Interfaces
{
	public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{

		void Update(OrderHeader orderHeader);
		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
		void UpdateStrripePaymentID(int id, string sessionId, string paymentIntenId);
	}
}
