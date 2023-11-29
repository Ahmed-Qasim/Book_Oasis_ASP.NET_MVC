using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models.Models;
using Book_Oasis.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Book_Oasis.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = StaticDetails.Role_Admin)]
	public class CompanyController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public CompanyController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			List<Company> objCompanyList = _unitOfWork.CompanyRepository.GetAll().ToList();
			return View(objCompanyList);
		}
		public IActionResult Upsert(int? id)

		{


			if (id == null || id == 0)
			{
				return View(new Company());
			}
			else
			{
				Company companyObj = _unitOfWork.CompanyRepository.Get(u => u.Id == id);
				return View(companyObj);
			}



		}
		[HttpPost]
		public IActionResult Upsert(Company companyObj)

		{


			if (ModelState.IsValid)
			{

				if (companyObj.Id == 0)
				{
					_unitOfWork.CompanyRepository.Add(companyObj);
					TempData["success"] = "Created Successfully";
				}
				else
				{
					_unitOfWork.CompanyRepository.Update(companyObj);
					TempData["success"] = "Updated Successfully";
				}
				_unitOfWork.Save();
				return RedirectToAction("Index");
			}
			else
			{

				return View(companyObj);
			}



		}


		#region API Calls
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Company> CompanyList = _unitOfWork.CompanyRepository.GetAll().ToList();
			return Json(new { data = CompanyList });

		}
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var CompanyToBeDeleted = _unitOfWork.CompanyRepository.Get(u => u.Id == id);
			if (CompanyToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error While Deleting" });
			}

			_unitOfWork.CompanyRepository.Delete(CompanyToBeDeleted);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Deleted Successfully" });

		}

		#endregion
	}
}
