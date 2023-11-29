using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models;
using Book_Oasis.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Oasis.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var categoryList = _unitOfWork.CategoryRepository.GetAll().ToList();
            return View(categoryList);
        }
        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        public IActionResult Create(Category newCat)

        {

            if (newCat.Name == newCat.DisplayOrder.ToString())
            {

                ModelState.AddModelError("name", "The Display order can't match the name");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Add(newCat);
                _unitOfWork.Save();
                TempData["success"] = "Created Successfully";
                return RedirectToAction("Index");
            }
            return View();


        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var selectedCategory = _unitOfWork.CategoryRepository.Get(x => x.Id == id);


            if (selectedCategory == null)
            {
                return NotFound();
            }
            return View(selectedCategory);

        }
        [HttpPost]
        public IActionResult Edit(Category newCat)

        {

            if (newCat.Name == newCat.DisplayOrder.ToString())
            {

                ModelState.AddModelError("name", "The Display order can't match the name");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Update(newCat);
                _unitOfWork.Save();
                TempData["success"] = "Edited Successfully";
                return RedirectToAction("Index");
            }
            return View();


        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var selectedCategory = _unitOfWork.CategoryRepository.Get(x => x.Id == id);


            if (selectedCategory == null)
            {
                return NotFound();
            }
            return View(selectedCategory);

        }
        [HttpPost]
        public IActionResult Delete(Category newCat)

        {
            var category = _unitOfWork.CategoryRepository.Get(x => x.Id == newCat.Id);

            if (category == null)
            {
                return NotFound();
            }
            _unitOfWork.CategoryRepository.Delete(category);
            _unitOfWork.Save();
            TempData["success"] = "deleted Successfully";
            return RedirectToAction("Index");



        }
    }
}
