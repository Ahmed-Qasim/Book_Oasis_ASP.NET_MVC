using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models.Models;
using Book_Oasis.Models.ViewModels;
using Book_Oasis.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Book_Oasis.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM shoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
				includeProp: "Product"),
				OrderHeader = new()

			};

			foreach (var cart in shoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

			}

			return View(shoppingCartVM);
		}

		public IActionResult Plus(int cartId)
		{
			var carFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.Id == cartId);
			carFromDb.Count += 1;
			_unitOfWork.ShoppingCartRepository.Update(carFromDb);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cartId)
		{
			var carFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.Id == cartId);
			if (carFromDb.Count <= 1)
			{
				_unitOfWork.ShoppingCartRepository.Delete(carFromDb);

			}
			else
			{
				carFromDb.Count -= 1;
				_unitOfWork.ShoppingCartRepository.Update(carFromDb);
			}

			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId)
		{
			var carFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.Id == cartId);
			_unitOfWork.ShoppingCartRepository.Delete(carFromDb);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Summary()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
				includeProp: "Product"),
				OrderHeader = new()

			};
			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);


			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;



			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

			}
			return View(ShoppingCartVM);
		}
		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
			   includeProp: "Product");

			ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
			ApplicationUser applicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
				ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;
			}
			else
			{
				ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
			}

			_unitOfWork.OrderHeaderRepository.Add(ShoppingCartVM.OrderHeader);
			_unitOfWork.Save();

			foreach (var item in ShoppingCartVM.ShoppingCartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = item.ProductId,
					OrderId = ShoppingCartVM.OrderHeader.Id,
					Count = item.Count,
					Price = item.Price,
				};
				_unitOfWork.OrderDetailRepository.Add(orderDetail);
				_unitOfWork.Save();
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				//stripe logic

				var domain = "https://localhost:7051/";
				var options = new SessionCreateOptions
				{
					SuccessUrl = domain + $"customer/cart/OrderConfirmation/{ShoppingCartVM.OrderHeader.Id}",
					CancelUrl = domain + "customer/cart/Index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};


				foreach (var item in ShoppingCartVM.ShoppingCartList)
				{
					var sessionItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(item.Price * 100),
							Currency = "usd",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Product.Title,

							}
						},
						Quantity = item.Count
					};
					options.LineItems.Add(sessionItem);
				}

				var service = new SessionService();
				Session session = service.Create(options);
				_unitOfWork.OrderHeaderRepository.UpdateStrripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
				_unitOfWork.Save();
				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);

			}

			return RedirectToAction(nameof(OrderConfirmation), new { ShoppingCartVM.OrderHeader.Id });
		}


		public IActionResult OrderConfirmation(int id)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == id, includeProp: "ApplicationUser");
			if (orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
			{
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);
				//check the stripe status
				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeaderRepository.UpdateStrripePaymentID(id, orderHeader.SessionId, session.PaymentIntentId);
					_unitOfWork.OrderHeaderRepository.UpdateStatus(id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}
			List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId ==
		  orderHeader.ApplicationUserId).ToList();
			HttpContext.Session.Clear();
			_unitOfWork.ShoppingCartRepository.DeleteAll(shoppingCarts);
			_unitOfWork.Save();
			return View(id);

		}




		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 50)
			{
				return shoppingCart.Product.Price;
			}
			else
			{
				if (shoppingCart.Count <= 100)
				{
					return shoppingCart.Product.Price50;
				}
				else
				{
					return shoppingCart.Product.Price100;
				}
			}
		}
	}
}
