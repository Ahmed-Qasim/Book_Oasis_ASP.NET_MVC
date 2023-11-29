using Book_Oasis.Models.Models;

namespace Book_Oasis.DataAccess.Repos.Interfaces
{
	public interface ICompanyRepository : IRepository<Company>
	{
		void Update(Company company);
	}
}
