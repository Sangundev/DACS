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

namespace Food_Web.Controllers
{
    public class Order_detailController : Controller
    {
        private FoodcontextDB db = new FoodcontextDB();

        // GET: Order_detail
        public async Task<ActionResult> Index(bool? statusFilter)
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                var loggedInUserId = User.Identity.GetUserId();
                var orderDetails = db.Order_detail.Include(o => o.Order).Include(o => o.Product).Where(i => i.Order.Od_name == loggedInUserId);

                if (statusFilter.HasValue)
                {
                    bool status = statusFilter.Value;
                    orderDetails = orderDetails.Where(o => o.Order.Od_status == status);
                }

                // Pass the filtered order details to the view
                return View(await orderDetails.ToListAsync());
            }
            else
            {
                // User is not authenticated, you can handle this accordingly (e.g., redirect to login page)
                return RedirectToAction("Login", "Account");
            }
        }

        public async Task<ActionResult> tt(int? paymentStatusFilter)
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                var loggedInUserId = User.Identity.GetUserId();
                var orderDetails = db.Order_detail.Include(o => o.Order).Include(o => o.Product).Where(i => i.Order.Od_name == loggedInUserId);

                if (paymentStatusFilter.HasValue)
                {
                    int idThanhToan = paymentStatusFilter.Value;
                    orderDetails = orderDetails.Where(o => o.Order.idthanhtoan == idThanhToan);
                }

                // Pass the filtered order details to the view
                return View("Index", await orderDetails.ToListAsync());

            }
            else
            {
                // User is not authenticated, you can handle this accordingly (e.g., redirect to login page)
                return RedirectToAction("Login", "Account");
            }
        }








    }
}
