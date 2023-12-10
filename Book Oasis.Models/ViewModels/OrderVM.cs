using Book_Oasis.Models.Models;

namespace Book_Oasis.Models.ViewModels
{
	public class OrderVM
	{
		public OrderHeader OrderHeader { get; set; }
		public IEnumerable<OrderDetail> orderDetails { get; set; }
	}
}
