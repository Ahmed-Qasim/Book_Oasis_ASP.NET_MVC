using Book_Oasis.DataAccess;
using Book_Oasis.Models.Models;
using Book_Oasis.Models.ViewModels;
using Book_Oasis.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace Book_Oasis.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = StaticDetails.Role_Admin)]
	public class UserController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly UserManager<IdentityUser> _userManager;

		public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
		{
			_db = db;
			_userManager = userManager;
		}
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult RoleManagement(string userId)
		{
			var roleId = _db.UserRoles.FirstOrDefault(x => x.UserId == userId).RoleId;
			RoleManagementVM RoleVM = new()
			{
				ApplicationUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(x => x.Id == userId),
				RoleList = _db.Roles.Select(i => new SelectListItem()
				{
					Text = i.Name,
					Value = i.Name
				}),
				CompanyList = _db.Companies.Select(i => new SelectListItem()
				{
					Text = i.Name,
					Value = i.Id.ToString()
				})


			};

			RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(i => i.Id == roleId).Name;

			return View(RoleVM);
		}
		[HttpPost]
		public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
		{
			var roleId = _db.UserRoles.FirstOrDefault(x => x.UserId == roleManagementVM.ApplicationUser.Id).RoleId;
			var oldRole = _db.Roles.FirstOrDefault(x => x.Id == roleId).Name;
			ApplicationUser applicationUser = _db.
					ApplicationUsers.FirstOrDefault(u => u.Id == roleManagementVM.ApplicationUser.Id);

			if (!(roleManagementVM.ApplicationUser.Role == oldRole))
			{


				if (roleManagementVM.ApplicationUser.Role == StaticDetails.Role_Company)
				{
					applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
				}
				if (oldRole == StaticDetails.Role_Company)
				{
					applicationUser.CompanyId = null;
				}
			}
			_db.SaveChanges();
			_userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
			_userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();



			return RedirectToAction(nameof(Index));
		}

		#region API Calls
		[HttpGet]
		public IActionResult GetAll()
		{
			List<ApplicationUser> userList = _db.ApplicationUsers.Include(u => u.Company).ToList();

			var UserRoles = _db.UserRoles.ToList();
			var roles = _db.Roles.ToList();

			foreach (var user in userList)
			{
				var roleId = UserRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
				user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
				if (user.Company == null)
				{
					user.Company = new()
					{
						Name = ""
					};
				}
			}
			return Json(new { data = userList });

		}
		[HttpPost]
		public IActionResult LockUnlock([FromBody] string? id)
		{
			var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
			if (objFromDb == null)
			{
				return Json(new { success = false, message = "error while locking/unlocking" });
			}
			if (objFromDb != null && objFromDb.LockoutEnd > DateTime.Now)
			{
				objFromDb.LockoutEnd = DateTime.Now;
			}
			else
			{
				objFromDb.LockoutEnd = DateTime.Now.AddYears(100);
			}
			_db.SaveChanges();
			return Json(new { success = true, message = "Operation Successfull" });

		}

		#endregion
	}
}
