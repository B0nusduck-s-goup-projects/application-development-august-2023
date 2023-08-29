using ASM2.Data;
using ASM2.Models;
using ASM2.Models.ViewModel;
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

		public async Task<int> RemoveItem(int ProductId, int quantity)
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
						cartItem.Quantity-= quantity;
                        			cartItem.TotalPrice -= (quantity * _context.Product.FirstOrDefault(x => x.Id == ProductId)!.Price);
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

		public async Task<CartViewModel> GetUserCart()
		{
			var userId = GetUserId();
			if (userId == null)
				throw new Exception("please login");
			var cart = await _context.Cart
                                    /*i dont know why, but the damn produdct table wont merge with the other table so i have to 
									 do some terrible work around, it says the table is invalid inside an 'Include' operation, 
									since it does not represent a property access. anyway ive manage to work around this but the
									soluton is kinda terrible imo*/
                                    //.Include(a => a.CartItem)
                                    //.ThenInclude(a => a.Product)
                                    //.ThenInclude(a => a.Category)
                                    .Where(a => a.UserId == userId && a.IsDeleted == false).FirstOrDefaultAsync();
			

			if (cart == null)
			{
				cart = new Cart()
				{
					UserId = GetUserId(),
				};
                await _context.Cart.AddAsync(cart);
                await _context.SaveChangesAsync();
			}
			else
			{
				var items = await _context.CartItem.Where(x => x.CartId == cart.Id).ToListAsync();
				List<Product> products = new List<Product>();
											/*fix attemp #? tbh i dont remember what attemp this is, the other atemp is just micro adjustment
											 which is just too insignificant to keep around. anyway this is the atemp that almost help me 
											solve it, the only problem is that this querry cant return any value other than null, IDK why
											i just coppy it form some document or video, i dont remember*/
											//cart.CartItem.Where(cartItem => cartItem.Product != null)
											//.Select(cartItem => cartItem.Product)
											//.ToList();
				foreach (var item in items) 
				{
					var product = _context.Product.FirstOrDefault(x => x.Id == item.ProductId);
					products.Add(product!);
				}
				/*i was thinking about adding category for it's name but because i add cat name
					 to product earlier out of lazyness to avoid merging two table, so i just use
					the cat name from the product object to avoid more debug*/
                //var categories = products.Select(product => product.Category).ToList();

                var viewModel = new CartViewModel
                {
                    Cart = cart,
                    Products = products,
					//also a part of category i dont want to do more debug
                    //Categories = categories
                };

                return viewModel;
            }
			/*fix attemp #1-8 at this time i though the cause was because the cart item was nullable
			 so i try putting it some where it cant be null, turns out it was not the problem*/
			//if (cart.CartItem != null)
			//{
			//    foreach (var cartItem in cart.CartItem)
			//    {
			//        _context.Entry(cartItem)
			//            .Reference(ci => ci.Product)
			//            .Query()
			//            .Include(p => p.Category)
			//            .Load();
			//    }
			//}
			return new CartViewModel();
		}

		public async Task<int> GetQuantity(string userID = "")
		{
			if (string.IsNullOrEmpty(userID))
			{
				userID = GetUserId();
			}
			var data = await (from Cart in _context.Cart.Where(x => x.IsDeleted == false && x.UserId ==userID)
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
