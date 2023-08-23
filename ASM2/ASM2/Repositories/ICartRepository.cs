using ASM2.Models;

namespace ASM2.Repositories
{
	public interface ICartRepository
	{
		bool AddItem(int ProductId, int quantity);
		bool RemoveItem(int ProductId);
		Task<IEnumerable<Cart>> GetUserCart();
	}
}
