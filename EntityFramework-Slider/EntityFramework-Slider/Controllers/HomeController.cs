using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace EntityFramework_Slider.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            //HttpContext.Session.SetString("name", "Pervin");

            //Response.Cookies.Append("surname", "Rehimli",new CookieOptions { MaxAge = TimeSpan.FromMinutes(30)});

            //Book book = new Book()
            //{
            //    Id = 1,
            //    Name = "Xosriv ve Shirin"
            //};



            //Response.Cookies.Append("book", JsonConvert.SerializeObject(book));


            List<Slider> sliders = await _context.Sliders.ToListAsync();

            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync();

            IEnumerable<Blog> blogs = await _context.Blogs.Where(m=>!m.SoftDelete).ToListAsync();

            IEnumerable<Category> categories = await _context.Categories.Where(m => !m.SoftDelete).ToListAsync();

            IEnumerable<Product> products = await _context.Products.Include(m=>m.Images).Where(m => !m.SoftDelete).ToListAsync();

            


            HomeVM model = new()
            {
                Sliders = sliders,
                SliderInfo = sliderInfo,
                Blogs = blogs,
                Categories = categories,
                Products = products,
                
            };

            return View(model);
        }


        



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id is null) return BadRequest();

            Product dbProduct = await _context.Products.FindAsync(id);

            if (dbProduct == null) return NotFound();

            List<BasketVM> basket;

            if(Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            }
            else
            {
                basket = new List<BasketVM>();
            }

            BasketVM existProduct = basket.FirstOrDefault(m=>m.Id == dbProduct.Id);


            if(existProduct == null)
            {
                basket?.Add(new BasketVM
                {
                    Id = dbProduct.Id,
                    Count = 1,
                    Price = (int)dbProduct.Price,
                    TotalPrice = (int)(dbProduct.Count * dbProduct.Price),
                    Image = dbProduct.Images.FirstOrDefault().Image

                }) ;
            }
            else
            {
                existProduct.Count++;
                existProduct.TotalPrice = (int) (existProduct.Price * existProduct.Count);
            }



            


            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));


            return RedirectToAction(nameof (Index));
        }














        //public IActionResult Test()
        //{
        //    var sessious = HttpContext.Session.GetString("name");
        //    var cookieData = Request.Cookies["surname"];

        //    var objectData = JsonConvert.DeserializeObject<Book>(Request.Cookies["book"]);

        //    return Json(sessious + "-" + cookieData);

                
        //}
    }





    //class Book
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}