using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models;
using Book_Oasis.Models.Models;
using Book_Oasis.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Book_Oasis.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _unitOfWork;
		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{

			IEnumerable<Product> productsList = _unitOfWork.ProductRepository.GetAll(includeProp: "Category,ProductImages");
			return View(productsList);
		}

		public IActionResult Details(int productId)
		{
			ShoppingCart shoppingCartItem = new()
			{
				Product = _unitOfWork.ProductRepository.Get(u => u.Id == productId, includeProp: "Category,ProductImages"),
				Count = 1,
				ProductId = productId
			};


			return View(shoppingCartItem);
		}

		[HttpPost]
		[Authorize]
		public IActionResult Details(ShoppingCart shoppingCart)
		{

			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			shoppingCart.ApplicationUserId = userId;

			ShoppingCart cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.ApplicationUserId == userId
			&& u.ProductId == shoppingCart.ProductId);

			if (cartFromDb != null)
			{
				cartFromDb.Count += shoppingCart.Count;
				_unitOfWork.ShoppingCartRepository.Update(cartFromDb);
				_unitOfWork.Save();

			}
			else
			{
				_unitOfWork.ShoppingCartRepository.Add(shoppingCart);
				_unitOfWork.Save();
				HttpContext.Session.SetInt32(StaticDetails.SessionCart,
			   _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId).Count());

			}

			TempData["success"] = "Updated Successfully";
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}