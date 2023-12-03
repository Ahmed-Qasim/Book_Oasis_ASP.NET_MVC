using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models.Models;

namespace Book_Oasis.DataAccess.Repos.IRepository
{
	public class CompanyRepository : Repository<Company>, ICompanyRepository
	{
		private readonly ApplicationDbContext _context;
		public CompanyRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}



		public void Update(Company company)
		{
			_context.Companies.Update(company);
		}
	}

}
