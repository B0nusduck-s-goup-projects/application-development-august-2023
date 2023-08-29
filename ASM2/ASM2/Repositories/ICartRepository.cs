using ASM2.Models;
using ASM2.Models.ViewModel;

namespace ASM2.Repositories
{
	public interface ICartRepository
	{
		Task<int> AddItem(int ProductId, int quantity);
		Task<int> RemoveItem(int ProductId, int quantity);
		void DeleteItem(int ProductId);
        Task<CartViewModel> GetUserCart();
		Task<int> GetQuantity(string userID = "");
		Cart GetCart(string userId);
		void DeleteCart();

    }
}
