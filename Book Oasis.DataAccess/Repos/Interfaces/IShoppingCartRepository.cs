using Book_Oasis.Models.Models;

namespace Book_Oasis.DataAccess.Repos.Interfaces
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {

        void Update(ShoppingCart shoppingCart);
    }
}
