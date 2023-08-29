using ASM2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASM2.Controllers
{
	[Authorize]
	public class CartController : Controller
	{
		private readonly ICartRepository _repository;

		public CartController(ICartRepository repository)
        {
			_repository = repository;
		}
		[Authorize]
        public async Task<IActionResult> AddItem(int productId,int quantity = 1, int redirect = 0)
		{
			var cartCount = await _repository.AddItem(productId, quantity);
			if (redirect == 0)
			{
				return Ok(cartCount);
			}
			return RedirectToAction("GetUserCart");
		}
		[Authorize]
		public async Task<IActionResult> RemoveItem(int productId)
		{
			var cartCcount = await _repository.RemoveItem(productId);
			return RedirectToAction("GetUserCart");

		}
		[Authorize]
		public async Task<IActionResult> GetUserCart()
		{
			var cart = await _repository.GetUserCart();
			return View(cart);
		}
		[Authorize]
		public async Task<IActionResult> GetTotalCartItem()
		{
			int quantity = await _repository.GetQuantity();
			return Ok(quantity);
		}
	}
}
