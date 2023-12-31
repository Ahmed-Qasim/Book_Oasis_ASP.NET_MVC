﻿using Book_Oasis.DataAccess.Repos.Interfaces;

namespace Book_Oasis.DataAccess.Repos.IRepository
{
	public class UnitOfWork : IUnitOfWork
	{
		public ICategoryRepository CategoryRepository { get; private set; }
		public IProductRepository ProductRepository { get; private set; }
		public ICompanyRepository CompanyRepository { get; set; }
		public IShoppingCartRepository ShoppingCartRepository { get; set; }
		public IApplicationUserRepository ApplicationUserRepository { get; set; }
		public IOrderHeaderRepository OrderHeaderRepository { get; set; }
		public IOrderDetailRepository OrderDetailRepository { get; set; }
		public IProductImageRepository ProductImageRepository { get; set; }

		private readonly ApplicationDbContext _context;

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
			CategoryRepository = new CategoryRepository(_context);
			ProductRepository = new ProductRepository(_context);
			CompanyRepository = new CompanyRepository(_context);
			ShoppingCartRepository = new ShoppingCartRepository(_context);
			ApplicationUserRepository = new ApplicationUserRepository(_context);
			OrderHeaderRepository = new OrderHeaderRepository(_context);
			OrderDetailRepository = new OrderDetailRepository(_context);
			ProductImageRepository = new ProductImageRepository(_context);

		}


		public void Save()
		{
			_context.SaveChanges();
		}


	}
}
