using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models.Models;
using Book_Oasis.Models.ViewModels;
using Book_Oasis.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace Book_Oasis.Areas.Admin.Controllers
{
	[Area("admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM OrderVM { get; set; }

		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
			OrderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == orderId, includeProp: "ApplicationUser"),
				orderDetails = _unitOfWork.OrderDetailRepository.GetAll(u => u.OrderId == orderId, includeProp: "Product")
			};
			return View(OrderVM);
		}
		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Empolyee)]
		public IActionResult UpdateOrderDetail()
		{
			var orderHeaderFromDB = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == OrderVM.OrderHeader.Id);
			orderHeaderFromDB.Name = OrderVM.OrderHeader.Name;
			orderHeaderFromDB.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
			orderHeaderFromDB.PostalCode = OrderVM.OrderHeader.PostalCode;
			orderHeaderFromDB.StreetAddress = OrderVM.OrderHeader.StreetAddress;
			orderHeaderFromDB.City = OrderVM.OrderHeader.City;
			orderHeaderFromDB.State = OrderVM.OrderHeader.State;
			if (string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
			{
				orderHeaderFromDB.Carrier = OrderVM.OrderHeader.Carrier;

			}
			if (string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
			{
				orderHeaderFromDB.Carrier = OrderVM.OrderHeader.TrackingNumber;

			}
			_unitOfWork.OrderHeaderRepository.Update(orderHeaderFromDB);
			_unitOfWork.Save();
			TempData["success"] = "Order Details Updated Successfully";


			return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDB.Id });
		}

		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Empolyee)]
		public IActionResult StartProcessing()
		{
			_unitOfWork.OrderHeaderRepository.UpdateStatus(OrderVM.OrderHeader.Id, StaticDetails.StatusInProcess);
			_unitOfWork.Save();

			TempData["success"] = "Order status Updated Successfully";


			return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

		}

		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Empolyee)]
		public IActionResult ShipOrder()
		{
			var orderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == OrderVM.OrderHeader.Id);
			orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
			orderHeader.OrderDate = DateTime.Now;
			orderHeader.OrderStatus = StaticDetails.StatusShipped;
			if (orderHeader.OrderStatus == StaticDetails.PaymentStatusDelayedPayment)
			{
				orderHeader.OrderDate = DateTime.Now.AddDays(30);

			}

			_unitOfWork.OrderHeaderRepository.Update(orderHeader);
			_unitOfWork.Save();

			TempData["success"] = "Order status Updated Successfully";


			return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

		}

		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Empolyee)]
		public IActionResult CancelOrder()
		{

			var orderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == OrderVM.OrderHeader.Id);
			if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntenId
				};

				var service = new RefundService();
				Refund refund = service.Create(options);

				_unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);
			}
			else
			{
				_unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusCancelled);
			}
			_unitOfWork.Save();

			TempData["Success"] = "Order Cancelled Successfully.";
			return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
		}



		#region API Calls
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders;


			if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Admin))
			{
				orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includeProp: "ApplicationUser").ToList();
			}
			else
			{
				var claimIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				orderHeaders = _unitOfWork.OrderHeaderRepository
					.GetAll(u => u.ApplicationUserId == userId, includeProp: "ApplicationUser").ToList();
			}

			switch (status)
			{
				case "pending":
					orderHeaders = orderHeaders.Where(u => u.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment);
					break;
				case "inprocess":
					orderHeaders = orderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusInProcess);
					break;
				case "completed":
					orderHeaders = orderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusShipped);
					break;
				case "approved":
					orderHeaders = orderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusApproved);
					break;
				default:
					break;
			}
			return Json(new { data = orderHeaders });

		}

		#endregion

	}






}

