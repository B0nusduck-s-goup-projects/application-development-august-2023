using ASM2.Data;
using ASM2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASM2.Controllers
{
	[Authorize]
	public class CartController : Controller
	{
		private readonly ICartRepository _repository;
		private readonly ApplicationDbContext _context;
		public CartController(ICartRepository repository, ApplicationDbContext context)
        {
			_repository = repository;
			_context = context;
		}
		[Authorize]
		public async Task<IActionResult> Index()
		{
			var Data = await _repository.GetUserCart();
			return View(Data);
		}
		[Authorize]
        public async Task<IActionResult> AddItem(int productId,int quantity = 1, int redirect = 0)
		{
			var cartCount = await _repository.AddItem(productId, quantity);
			if (redirect == 0)
			{
				return Ok(cartCount);
			}
			return RedirectToAction("Index");
		}
		[Authorize]
		public async Task<IActionResult> RemoveItem(int productId)
		{
			var cartCcount = await _repository.RemoveItem(productId);
			return RedirectToAction("Index");
		}
		[Authorize]
		public IActionResult DeleteItem(int productId)
		{
			_repository.DeleteItem(productId);
            return RedirectToAction("Index");
        }
        //[Authorize]
        //public IActionResult GetUserCart()
        //{
        //	var cart = _repository.GetUserCart();
        //	return View(cart);
        //}
        [Authorize]
		public async Task<IActionResult> GetTotalCartItem()
		{
			int quantity = await _repository.GetQuantity();
			return Ok(quantity);
		}
		[Authorize]
		public IActionResult DeleteCart()
		{
			_repository.DeleteCart();
            return Redirect("/Home/index");
        }
	}
}
