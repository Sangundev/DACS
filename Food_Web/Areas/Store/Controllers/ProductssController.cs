using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Food_Web.Models;
using Microsoft.AspNet.Identity;
using System.IO;

namespace Food_Web.Areas.Store.Controllers
{
    public class ProductssController : Controller
    {
        private FoodcontextDB db = new FoodcontextDB();

        // GET: Store/Productss
        //public async Task<ActionResult> Index()
        //{
        //    var products = db.Products.Include(p => p.Category).Include(p => p.chef);
        //    return View(await products.ToListAsync());
        //}
        public ActionResult Index()
        {
            // Lấy thông tin người dùng đăng nhập
            var userId = User.Identity.GetUserId();

            // them 
            var totalProducts = CalculateTotalProductsForLoggedInUser();
            ViewBag.TotalStock = totalProducts;

            //thme
            var TotalProductsSold = CalculateTotalProductsSoldForLoggedInStore();
            ViewBag.TotalProductsSold = TotalProductsSold;

            //them 

          

                  var TotalMoney = CalculateTotamoneyForLoggedInUser();
            ViewBag.TotalMoney = TotalMoney;

            // Lấy danh sách sản phẩm thuộc người dùng đăng nhập
            var products = db.Products
                .Where(p => p.Userid == userId)
                .Include(p => p.Category)
                .Include(p => p.chef);

            return View(products.ToList());
        }
        // GET: Store/Productss/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Store/Productss/Create
        public ActionResult Create()
        {
            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Categoryname");
            ViewBag.chef_id = new SelectList(db.chefs, "chef_id", "image");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Productid,Productname,price,image,discription,Categoryid,sortdiscription,chef_id")] Product product)
        {
            if (ModelState.IsValid)
            {
                // Lấy id của người đăng nhập và gán cho trường Userid của đối tượng Product
                product.Userid = User.Identity.GetUserId();

                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Categoryname", product.Categoryid);
            ViewBag.chef_id = new SelectList(db.chefs, "chef_id", "image", product.chef_id);
            return View(product);
        }

        // GET: Store/Productss/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Categoryname", product.Categoryid);
            ViewBag.chef_id = new SelectList(db.chefs, "chef_id", "image", product.chef_id);
            return View(product);
        }

        // POST: Store/Productss/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Productid,Productname,price,image,discription,Categoryid,sortdiscription,chef_id,Userid")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Categoryname", product.Categoryid);
            ViewBag.chef_id = new SelectList(db.chefs, "chef_id", "image", product.chef_id);
            return View(product);
        }

        // GET: Store/Productss/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Store/Productss/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult UpdateStatus(int id, bool status)
        {
            // lấy sản phẩm từ database
            var product = db.Products.FirstOrDefault(p => p.Productid == id);
            if (product == null)
            {
                return Json(new { success = false });
            }
            // cập nhật trạng thái của sản phẩm
            product.status = status;
            db.SaveChanges();
            return Json(new { success = true });
        }



        public ActionResult hot()
        {
            // Lấy thông tin người dùng đăng nhập
            var userId = User.Identity.GetUserId();

            // Lấy danh sách sản phẩm thuộc người dùng đăng nhập và có thuộc tính is_hot bằng true
            var products = db.Products
                .Where(p => p.Userid == userId && p.is_hot == true)
                .Include(p => p.Category)
                .Include(p => p.chef);

            return View(products.ToList());
        }
        private List<SelectListItem> GetProductList(string userId)
        {
            var productList = db.Products
                .Where(p => p.Userid == userId)
                .Select(p => new SelectListItem
                {
                    Value = p.Productid.ToString(),
                    Text = p.Productname
                })
                .ToList();

            return productList;
        }

        public ActionResult SetHot()
        {
            // Lấy thông tin người dùng đăng nhập
            var userId = User.Identity.GetUserId();

            // Lấy danh sách sản phẩm thuộc người dùng đăng nhập
            var productList = GetProductList(userId);

            // Tạo đối tượng SetHotViewModel và gán danh sách sản phẩm vào
            var viewModel = new SetHotViewModel
            {
                Products = productList
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SetHot(SetHotViewModel model)
        {
            var product = db.Products.Find(model.SelectedProductId);
            if (product != null)
            {
                product.is_hot = true;
                db.SaveChanges();
            }

            // Lấy thông tin người dùng đăng nhập
            var userId = User.Identity.GetUserId();

            // Lấy danh sách sản phẩm thuộc người dùng đăng nhập
            var productList = GetProductList(userId);

            // Tạo đối tượng SetHotViewModel và gán danh sách sản phẩm vào
            var viewModel = new SetHotViewModel
            {
                Products = productList
            };

            return RedirectToAction("hot");
        }


        public async Task<ActionResult> Sale()
        {
            var userId = User.Identity.GetUserId();

            var products = await db.Products.Where(p => p.DiscountPercent > 0 && p.Userid == userId).ToListAsync();

            return View(products);
        }

        public async Task<ActionResult> CreateDiscount()
        {
            var userId = User.Identity.GetUserId();
            var products = await db.Products.Where(p => p.Userid == userId).ToListAsync();
            ViewBag.Products = new SelectList(products, "Productid", "ProductName");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateDiscount(int productId, int discountPercent)
        {
            var product = await db.Products.FindAsync(productId);

            if (product != null )
            {
                product.DiscountPercent = discountPercent;
                var discountedPrice = (int)(product.price * (100 - discountPercent) / 100);
                product.DiscountedPrice = discountedPrice;
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Sale");
        }


        public int CalculateTotalProductsSoldForLoggedInStore()
        {
            var userId = User.Identity.GetUserId();

            int totalProductsSold = db.Order_detail
                .Where(o => o.Storeid == userId)
                .Sum(o => (int)o.num);

            return totalProductsSold;
        }



        public int CalculateTotalProductsForLoggedInUser()
        {
            var userId = User.Identity.GetUserId();

            int totalProducts = db.Products
                .Count(p => p.Userid == userId);

            return totalProducts;
        }
        public int CalculateTotamoneyForLoggedInUser()
        {
            var userId = User.Identity.GetUserId();

            int totalProductsSold = db.Order_detail
                .Where(o => o.Storeid == userId)
                .Sum(o => (int)o.tt_money);

            return totalProductsSold;
        }


    }
}
