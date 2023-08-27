using ASM2.Models;

namespace ASM2.Repositories
{
	public interface ICartRepository
	{
		Task<int> AddItem(int ProductId, int quantity);
		Task<int> RemoveItem(int ProductId);
		void DeleteItem(int ProductId);
        Task<Cart> GetUserCart();
		Task<int> GetQuantity(string userID = "");
		Cart GetCart(string userId);
		void DeleteCart();
	}
}
