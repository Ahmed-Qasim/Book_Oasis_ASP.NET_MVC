using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models.Models;
using Book_Oasis.Models.ViewModels;
using Book_Oasis.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Book_Oasis.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = StaticDetails.Role_Admin)]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult Index()
		{
			var productList = _unitOfWork.ProductRepository.GetAll(includeProp: "Category").ToList();
			return View(productList);
		}
		public IActionResult Upsert(int? id)

		{


			IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(c =>
			new SelectListItem
			{
				Text = c.Name,
				Value = c.Id.ToString(),

			});

			ProdcutVM prodcutVM = new()
			{
				Product = new Product(),
				CategoryList = CategoryList,
			};


			if (id == null || id == 0)
			{
				return View(prodcutVM);
			}
			else
			{
				prodcutVM.Product = _unitOfWork.ProductRepository.Get(u => u.Id == id, includeProp: "ProductImages");
				//prodcutVM.Product.ProductImages = _unitOfWork.ProductImageRepository.GetAll(u => u.Id == id).ToList();
			}

			return View(prodcutVM);

		}
		[HttpPost]
		public IActionResult Upsert(ProdcutVM productVM, List<IFormFile?> files)

		{
			if (ModelState.IsValid)
			{
				if (productVM.Product.Id == 0)
				{
					_unitOfWork.ProductRepository.Add(productVM.Product);
				}
				else
				{
					_unitOfWork.ProductRepository.Update(productVM.Product);
				}
				_unitOfWork.Save();

				string wwwRootPath = _webHostEnvironment.WebRootPath;

				if (files != null)
				{
					foreach (var file in files)
					{
						string FileName = Guid.NewGuid().ToString() + Path.GetExtension(file?.FileName);
						string productPath = @"images\Products\product-" + productVM.Product.Id;
						string finalPath = Path.Combine(wwwRootPath, productPath);

						if (!Directory.Exists(finalPath))
						{
							Directory.CreateDirectory(finalPath);
						}

						using (var fileStream = new FileStream(Path.Combine(finalPath, FileName), FileMode.Create))
						{
							file.CopyTo(fileStream);
						};

						ProductImage productImage = new()
						{
							ImageUrl = @"\" + productPath + @"\" + FileName,
							ProductId = productVM.Product.Id,
						};

						if (productVM.Product.ProductImages == null)
						{
							productVM.Product.ProductImages = new List<ProductImage>();
						}
						productVM.Product.ProductImages.Add(productImage);


					}

					_unitOfWork.ProductRepository.Update(productVM.Product);
					_unitOfWork.Save();

				}
				TempData["success"] = "Product created/updated successfully";
				return RedirectToAction("Index");
			}
			else
			{
				productVM.CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(c =>
				new SelectListItem
				{
					Text = c.Name,
					Value = c.Id.ToString(),
				}
				);
				return View(productVM);
			}



		}

		public IActionResult DeleteImage(int imageId)
		{

			var imageToBeDeleted = _unitOfWork.ProductImageRepository.Get(u => u.Id == imageId);
			int productId = imageToBeDeleted.ProductId;
			if (imageToBeDeleted != null)
			{
				if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
				{
					var oldImagePath =
								   Path.Combine(_webHostEnvironment.WebRootPath,
								   imageToBeDeleted.ImageUrl.TrimStart('\\'));

					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				_unitOfWork.ProductImageRepository.Delete(imageToBeDeleted);
				_unitOfWork.Save();

				TempData["success"] = "Deleted successfully";
			}

			return RedirectToAction(nameof(Upsert), new { id = productId });
		}


		#region API Calls
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> ProductList = _unitOfWork.ProductRepository.GetAll(includeProp: "Category").ToList();
			return Json(new { data = ProductList });

		}
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _unitOfWork.ProductRepository.Get(u => u.Id == id);
			if (productToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			string productPath = @"images\products\product-" + id;
			string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

			if (Directory.Exists(finalPath))
			{
				string[] filePaths = Directory.GetFiles(finalPath);
				foreach (string filePath in filePaths)
				{
					System.IO.File.Delete(filePath);
				}

				Directory.Delete(finalPath);
			}


			_unitOfWork.ProductRepository.Delete(productToBeDeleted);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Delete Successful" });

		}

		#endregion
	}
}
