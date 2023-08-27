namespace ASM2.Models.ViewModel
{
    public class CartViewModel
    {
        public Cart Cart { get; set; }
        public List<Product> Products { get; set; }
        /*i was thinking about adding category for it's name but because i add cat name
        to product earlier out of lazyness to avoid merging two table, so now i just use
		the cat name from the product object to avoid more debug*/
        //public List<Category> Categories { get; set; }
    }
}