﻿namespace Book_Oasis.DataAccess.Repos.Interfaces
{
	public interface IUnitOfWork
	{
		ICategoryRepository CategoryRepository { get; }
		IProductRepository ProductRepository { get; }
		ICompanyRepository CompanyRepository { get; }
		IShoppingCartRepository ShoppingCartRepository { get; }
		IApplicationUserRepository ApplicationUserRepository { get; }
		IOrderHeaderRepository OrderHeaderRepository { get; }
		IOrderDetailRepository OrderDetailRepository { get; }

		void Save();

	}
}
