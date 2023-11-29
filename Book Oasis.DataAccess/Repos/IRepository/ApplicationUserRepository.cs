using Book_Oasis.DataAccess.Repos.Interfaces;
using Book_Oasis.Models.Models;

namespace Book_Oasis.DataAccess.Repos.IRepository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;
        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }




    }

}
