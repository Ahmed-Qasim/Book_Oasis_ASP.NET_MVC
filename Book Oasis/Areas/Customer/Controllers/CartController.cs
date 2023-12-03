using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models.Models;
using Book_Oasis.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Book_Oasis.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
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

			};

			foreach (var cart in shoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				shoppingCartVM.OrderTotal += (cart.Price * cart.Count);

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

			return View();
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
