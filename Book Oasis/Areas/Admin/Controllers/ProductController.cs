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
                prodcutVM.Product = _unitOfWork.ProductRepository.Get(u => u.Id == id);
            }

            return View(prodcutVM);

        }
        [HttpPost]
        public IActionResult Upsert(ProdcutVM productVM, IFormFile? img)

        {


            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (img != null)
                {
                    string FileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\Product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImgUrl))
                    {
                        //delete the old path
                        var oldImgPath = Path.Combine(wwwRootPath, (productVM.Product.ImgUrl.TrimStart('\\')));
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, FileName), FileMode.Create))
                    {
                        img.CopyTo(fileStream);
                    };

                    productVM.Product.ImgUrl = @"\images\Product\" + FileName;

                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.ProductRepository.Add(productVM.Product);
                    TempData["success"] = "Created Successfully";
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(productVM.Product);
                    TempData["success"] = "Updated Successfully";
                }
                _unitOfWork.Save();
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
        ////public IActionResult Edit(int? id)
        ////{
        ////	if (id == null || id == 0)
        ////	{
        ////		return NotFound();
        ////	}

        ////	var selectedproduct = _unitOfWork.ProductRepository.Get(x => x.Id == id);


        ////	if (selectedproduct == null)
        ////	{
        ////		return NotFound();
        ////	}
        ////	return View(selectedproduct);

        ////}
        ////[HttpPost]
        ////public IActionResult Edit(Product newProduct)

        ////{

        ////	if (ModelState.IsValid)
        ////	{
        ////		_unitOfWork.ProductRepository.Update(newProduct);
        ////		_unitOfWork.Save();
        ////		TempData["success"] = "Edited Successfully";
        ////		return RedirectToAction("Index");
        ////	}

        ////	return View();



        ////}
        //public IActionResult Delete(int? id)
        //{
        //	if (id == null || id == 0)
        //	{
        //		return NotFound();
        //	}

        //	var selectedproduct = _unitOfWork.ProductRepository.Get(x => x.Id == id);


        //	if (selectedproduct == null)
        //	{
        //		return NotFound();
        //	}
        //	return View(selectedproduct);

        //}
        //[HttpPost]
        //public IActionResult Delete(Product Product)

        //{
        //	var product = _unitOfWork.ProductRepository.Get(x => x.Id == Product.Id);

        //	if (product == null)
        //	{
        //		return NotFound();
        //	}
        //	_unitOfWork.ProductRepository.Delete(product);
        //	_unitOfWork.Save();
        //	TempData["success"] = "deleted Successfully";
        //	return RedirectToAction("Index");



        //	}


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
                return Json(new { success = false, message = "Error While Deleting" });
            }
            var oldImgPath = Path.Combine(_webHostEnvironment.WebRootPath, (productToBeDeleted.ImgUrl.TrimStart('\\')));
            if (System.IO.File.Exists(oldImgPath))
            {
                System.IO.File.Delete(oldImgPath);
            }
            _unitOfWork.ProductRepository.Delete(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted Successfully" });

        }

        #endregion
    }
}
