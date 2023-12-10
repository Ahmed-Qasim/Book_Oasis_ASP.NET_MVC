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

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if (orderFromDb != null)
			{
				orderFromDb.OrderStatus = orderStatus;
				if (!string.IsNullOrEmpty(paymentStatus))
				{
					orderFromDb.PaymentStatus = paymentStatus;
				}

			}

		}

		public void UpdateStrripePaymentID(int id, string sessionId, string paymentIntenId)
		{
			var orderFromDb = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if (!string.IsNullOrEmpty(sessionId))
			{
				orderFromDb.SessionId = sessionId;
				if (!string.IsNullOrEmpty(paymentIntenId))
				{
					orderFromDb.PaymentIntenId = paymentIntenId;
					orderFromDb.PaymentDate = DateTime.Now;
				}

			}

		}
	}

}
