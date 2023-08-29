using ASM2.Data;
using ASM2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Transactions;

namespace ASM2.Repositories
{
	public class CartRepository : ICartRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IHttpContextAccessor _contextAccessor;
		public CartRepository(ApplicationDbContext context,
			UserManager<IdentityUser> userManager,
			IHttpContextAccessor contextAccessor)
		{
			_context = context;
			_userManager = userManager;
			_contextAccessor = contextAccessor;
		}
		public async Task<int> AddItem(int ProductId, int quantity)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				if (string.IsNullOrEmpty(GetUserId()))
				{
					throw new Exception("user not found");
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
						cartItem.TotalPrice += (quantity * _context.Product.FirstOrDefault(x => x.Id == ProductId)!.Price);
					}
					else
					{
						cartItem = new CartItem()
						{
							CartId = cart.Id,
							ProductId = ProductId,
							ProductName = _context.Product.FirstOrDefault(x => x.Id == ProductId)!.Name,
							Quantity = quantity,
                            TotalPrice = quantity * _context.Product.FirstOrDefault(x => x.Id == ProductId)!.Price
						};
						_context.CartItem.Add(cartItem);
					}
					_context.SaveChanges();
					transaction.Commit();
				}
			}
			catch (Exception e)
			{
				throw;
			}
			var cartItemCount = await GetQuantity(GetUserId());
			return cartItemCount;
		}

		public async Task<int> RemoveItem(int ProductId)
		{
			try
			{
				if (string.IsNullOrEmpty(GetUserId()))
				{
					throw new Exception("user not found");
				}
				else
				{
					var cart = GetCart(GetUserId());
					if (cart == null)
					{
						throw new Exception("cart not found");
					}
					var cartItem = _context.CartItem.FirstOrDefault(x => x.CartId == cart.Id && x.ProductId == ProductId);
					if (cartItem == null)
					{
						throw new Exception("cart don't have item");
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
				}
			}
			catch (Exception e)
			{
				throw;
			}
			var cartItemCount = await GetQuantity(GetUserId());
			return cartItemCount;
		}

		public void DeleteItem(int ProductId)
		{
            try
            {
                if (string.IsNullOrEmpty(GetUserId()))
                {
                    throw new Exception("user not found");
                }
                else
                {
                    var cart = GetCart(GetUserId());
                    if (cart == null)
                    {
                        throw new Exception("cart not found");
                    }
                    var cartItem = _context.CartItem.FirstOrDefault(x => x.CartId == cart.Id && x.ProductId == ProductId);
                    if (cartItem == null)
                    {
                        throw new Exception("cart don't have item");
                    }
                    else
                    {
                        _context.CartItem.Remove(cartItem);
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

		public async Task<Cart> GetUserCart()
		{
			var userId = GetUserId();
			if (userId == null)
				throw new Exception("please login");
			var cart = await _context.Cart
									.Include(a => a.CartItem)
									.ThenInclude(a => a.Product)
									.ThenInclude(a => a.Category)
									.Where(a => a.UserId == userId && a.IsDeleted == false).FirstOrDefaultAsync();
			if (cart == null)
			{
				cart = new Cart()
				{
					UserId = GetUserId(),
				};
				_context.Cart.Add(cart);
			}
			_context.SaveChanges();
			return cart;
		}

		public async Task<int> GetQuantity(string userID = "")
		{
			if (!string.IsNullOrEmpty(userID))
			{
				userID = GetUserId();
			}
			var data = await (from Cart in _context.Cart.Where(x => x.IsDeleted == false)
							  join CartItem in _context.CartItem
							  on Cart.Id equals CartItem.CartId
							  select new { CartItem.Id }).ToListAsync();
			return data.Count();
		}

		public Cart GetCart(string userId)
		{
			var cart = _context.Cart.Where(x => x.UserId == userId).ToList().Where(x => x.IsDeleted == false).FirstOrDefault();
			return  cart;
		}

		public void DeleteCart()
		{
			var userId = GetUserId();
			var cart = GetCart(userId);
			cart.IsDeleted = true;
			_context.SaveChanges();
		}

		private string GetUserId()
		{
			var user = _contextAccessor.HttpContext.User;
			string userId = _userManager.GetUserId(user);
			return userId;
		}
	}
}
