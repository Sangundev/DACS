using Food_Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Data.Entity;
using PagedList;

namespace Food_Web.Controllers
{
    public class StoresController : Controller
    {

        public ActionResult Index(string storeid)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var userList = userManager.Users.Where(u => u.IsApproved).ToList();
            return View(userList);
        }
        public static string getIdStore = "";

            public ActionResult StoreProducts(string id)
            {
                getIdStore = id;
                if (string.IsNullOrEmpty(id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                using (var db = new ApplicationDbContext())
                {
                    var products = db.Products.Where(p => p.Userid == id /*&& p.status == true*/).ToList();
                    ViewBag.userId = id;

                    return View(products);
                }
            }

           [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Create([Bind(Include = "content, created_at, Rating, storeId")] Comment comment , string storeId)
            {
                getIdStore= storeId;
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    using (var db = new FoodcontextDB())
                    {
                        // Retrieve the maximum comment_id from the database
                        int maxCommentId = db.Comments.Max(c => c.comment_id);

                        // Increment the comment_id by 1 to generate a new unique ID
                        comment.comment_id = maxCommentId + 1;

                        // Set the user_id and Store_id
                        comment.user_id = User.Identity.GetUserId();
                        comment.Store_id = storeId;
                        comment.created = DateTime.Now;

                        db.Comments.Add(comment);
                        await db.SaveChangesAsync();
                    }

                    return RedirectToAction("StoreProducts", new { id = storeId });
                }

                return View("StoreProducts");
            }

        //public ActionResult ShowComment(string storeId)
        //{
        //    storeId = getIdStore;
        //    using (var db = new FoodcontextDB())
        //    {
        //        var comments = db.Comments.Where(c => c.Store_id == storeId).ToList();
        //        return View(comments);
        //    }
        //}
        public ActionResult ShowComment(string storeId, int? page)
        {
            storeId = getIdStore;

            using (var db = new FoodcontextDB())
            {
                var comments = db.Comments.Where(c => c.Store_id == storeId).OrderBy(c => c.created).ToList();

                int pageSize = 2; // Number of comments per page
                int pageNumber = page ?? 1; // If no page is specified, default to page 1
                var pagedComments = comments.ToPagedList(pageNumber, pageSize);

                return View(pagedComments);
            }
        }



        public ActionResult HotProducts(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new ApplicationDbContext())
            {
                var products = db.Products.Where(p => p.Userid == id && p.is_hot == true).ToList();
                return View(products);
            }
        }

        public async Task<ActionResult> Sale1(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new ApplicationDbContext())
            {
                var products = db.Products.Where(p => p.Userid == id && p.DiscountPercent != null).ToList();
                return View(products);
            }
        }

        //}




    }
}