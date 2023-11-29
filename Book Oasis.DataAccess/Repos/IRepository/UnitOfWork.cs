using Book_Oasis.DataAccess.Repos.Interfaces;

namespace Book_Oasis.DataAccess.Repos.IRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public ICompanyRepository CompanyRepository { get; set; }
        public IShoppingCartRepository ShoppingCartRepository { get; set; }
        public IApplicationUserRepository ApplicationUserRepository { get; set; }



        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            CategoryRepository = new CategoryRepository(_context);
            ProductRepository = new ProductRepository(_context);
            CompanyRepository = new CompanyRepository(_context);
            ShoppingCartRepository = new ShoppingCartRepository(_context);
            ApplicationUserRepository = new ApplicationUserRepository(_context);



        }


        public void Save()
        {
            _context.SaveChanges();
        }


    }
}
