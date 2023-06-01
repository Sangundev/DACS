using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Food_Web.Models;

namespace Food_Web.Controllers
{
    //[Authorize(Roles = "Member")]
    public class ProductController : Controller
    {
        private FoodcontextDB db = new FoodcontextDB();

        public ActionResult GetCategorys()
        {
            List<Category> listCategorys = db.Categories.OrderBy(p => p.Categoryid ).ToList();
            return PartialView(listCategorys);
        }

        public ActionResult GetProductByCategory(int categoryid)
        {
            List<Product> listProduct = db.Products.Where(p => p.Categoryid == categoryid && p.status ==true).ToList();
            return PartialView(listProduct);
        }
        public ActionResult Index()
        {
            var context = new FoodcontextDB(); 
            return View(context.Products.ToList());
        }


        public ActionResult ChangeLanguage(string lang)
        {
            CultureInfo.CurrentCulture = new CultureInfo(lang, false);
            CultureInfo.CurrentUICulture = new CultureInfo(lang, false);
            var context = new FoodcontextDB();
            return View("Index", context.Products.ToList());
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Search(String searchString)
        {
            var context = new FoodcontextDB();
            var results =
                (from m in context.Products
                 where
                 m.Productname.Contains(searchString)
                 || m.Category.Categoryname.Contains(searchString)
                 select m);
            if (results.Count() > 0)
            {
                //return View("Index", results.ToList().ToPagedList(1, 2));
                //    return RedirectToAction("Index?page=1");
                return PartialView(results);
            }
            return HttpNotFound("Thono tin chua co may cha oiw");
        }

        public ActionResult Menu()
        {
            var context = new FoodcontextDB();
            return View(context.Products.ToList());
        }
    }
}
