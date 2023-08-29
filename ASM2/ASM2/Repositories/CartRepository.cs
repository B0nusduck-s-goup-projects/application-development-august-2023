using ASM2.Data;
using ASM2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace ASM2.Repositories
{
	public class CartRepository : ICartRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IHttpContextAccessor _contextAccessor;
		CartRepository(ApplicationDbContext context,
			UserManager<IdentityUser> userManager,
			IHttpContextAccessor contextAccessor)
		{
			_context = context;
			_userManager = userManager;
			_contextAccessor = contextAccessor;
		}
		public bool AddItem(int ProductId, int quantity)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				if (string.IsNullOrEmpty(GetUserId()))
				{
					return false;
				}
				else
				{
					var cart = GetCart(GetUserId());
					if (cart == null)
					{
						cart = new Cart()
						{
							UserId = GetUserId(),
						};
						_context.Cart.Add(cart);
					}
					_context.SaveChanges();
					var cartItem = _context.CartItem.FirstOrDefault(x => x.CartId == cart.Id && x.ProductId == ProductId);
					if (cartItem != null)
					{
						cartItem.Quantity += quantity;
						cartItem.UnitPrice += (quantity * _context.Product.FirstOrDefault(x => x.Id == ProductId)!.Price);
					}
					else
					{
						cartItem = new CartItem()
						{
							CartId = cart.Id,
							ProductId = ProductId,
							ProductName = _context.Product.FirstOrDefault(x => x.Id == ProductId)!.Name,
							Quantity = quantity,
							UnitPrice = quantity * _context.Product.FirstOrDefault(x => x.Id == ProductId)!.Price
						};
						_context.CartItem.Add(cartItem);
					}
					_context.SaveChanges();
					transaction.Commit();
					return true;
				}
			}
			catch (Exception e)
			{
				return false;
			}
		}

		public bool RemoveItem(int ProductId)
		{
			try
			{
				if (string.IsNullOrEmpty(GetUserId()))
				{
					return false;
				}
				else
				{
					var cart = GetCart(GetUserId());
					if (cart == null)
					{
						return false;
					}
					var cartItem = _context.CartItem.FirstOrDefault(x => x.CartId == cart.Id && x.ProductId == ProductId);
					if (cartItem == null)
					{
						return false;
					}
					else if (cartItem.Quantity == 1)
					{
						_context.CartItem.Remove(cartItem);
					}
					else
					{
						cartItem.Quantity--;
					}
					_context.SaveChanges();
					return true;
				}
			}
			catch (Exception e)
			{
				return false;
			}
		}

		public async Task<IEnumerable<Cart>> GetUserCart()
		{
			var userId = GetUserId();
			if (userId == null)
				throw new Exception("please login");
			var cart = await _context.Cart.Include(a => a.CartItem)!
									.ThenInclude(a => a.Product)
									.ThenInclude(a => a.Category)
									.Where(a => a.UserId == userId).ToListAsync();
			return cart;
		}

		private Cart GetCart(string userId)
		{
			var cart = _context.Cart.Where(x => x.UserId == userId).ToList().Where(x => x.IsDeleted = false).FirstOrDefault();
			return  cart;
		}

		private string GetUserId()
		{
			var user = _contextAccessor.HttpContext.User;
			string userId = _userManager.GetUserId(user);
			return userId;
		}
	}
}
