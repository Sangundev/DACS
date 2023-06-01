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
using System.Data.Entity.SqlServer;

namespace Food_Web.Areas.Store.Controllers
{
    public class Order_detailsController : Controller
    {
        private FoodcontextDB db = new FoodcontextDB();

        // GET: Store/Order_details
        public async Task<ActionResult> Index()
        {
            // Get the currently logged-in user
            string userId = User.Identity.GetUserId();

            // Retrieve order details for the logged-in user
            var order_detail = db.Order_detail.Include(o => o.Order).Include(o => o.Product)
                                   .Where(o => o.Storeid == userId);

            return View(await order_detail.ToListAsync());
        }


        // GET: Store/Order_details/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_detail order_detail = await db.Order_detail.FindAsync(id);
            if (order_detail == null)
            {
                return HttpNotFound();
            }
            return View(order_detail);
        }


        [HttpPost]
        public ActionResult UpStatus(int id, bool status)
        {
            try
            {
                var orderDetail = db.Order_detail.FirstOrDefault(o => o.Order.idthanhtoan == id);
                if (orderDetail == null)
                {
                    return Json(new { success = false, message = "Order not found." });
                }

                orderDetail.Order.Od_status = !status;
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public async Task<ActionResult> Orderday(DateTime? od_day)
        {
            var userId = User.Identity.GetUserId(); // Get the ID of the currently logged-in user
            var order_details = db.Order_detail.Include(o => o.Order).Include(o => o.Product);

            if (od_day.HasValue)
            {
                // Filter order details by od_day and user ID
                order_details = order_details.Where(o => DbFunctions.TruncateTime(o.Order.Od_date) == DbFunctions.TruncateTime(od_day.Value)
                    && o.Storeid == userId);
            }
            else
            {
                // Filter order details by user ID only
                order_details = order_details.Where(o => o.Storeid == userId);
            }

            return View(await order_details.ToListAsync());
        }


      

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
