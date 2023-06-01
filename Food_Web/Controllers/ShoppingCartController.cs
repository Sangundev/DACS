using Food_Web.Models;
using Food_Web.Other;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Schema;



namespace Food_Web.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        FoodcontextDB db;
        public ShoppingCartController()
        {
            db = new FoodcontextDB();
        }
        
        public ActionResult Index(int? page)
        {
            //List<CartItem> ShoppingCart = GetCartItemsFromSession();
            var IDUser = User.Identity.GetUserId();
            var ShoppingCart = db.CartItems.Where(x => x.IdUser == IDUser).ToList();
            int pageSize = 2; // số sản phẩm hiển thị trên mỗi trang
            int pageIndex = page.HasValue ? page.Value : 1; // trang hiện tại, nếu không có thì mặc định là 1
            IPagedList<CartItem> cartItems = ShoppingCart.ToPagedList(pageIndex, pageSize);

            ViewBag.Tongsoluong = ShoppingCart.Sum(p => p.Quantity);
            ViewBag.Tongtien = ShoppingCart.Sum(p => p.Money);

            return View(cartItems);

        }
        [Authorize(Roles = "Admin")]
        public List<CartItem> GetCartItemsFromSession()
        {
            var lstShoppingCart = Session["ShoppingCart"] as List<CartItem>;
            if (lstShoppingCart == null)
            {
                lstShoppingCart = new List<CartItem>();
                Session["ShoppingCart"] = lstShoppingCart;

            }
            return lstShoppingCart;
        }
        //[Authorize]
        [HttpPost]
        public string AddToCart(int id)
        {
            try
            {

                var IDUser = User.Identity.GetUserId();
                var findCartItem = db.CartItems.FirstOrDefault(p => p.Productid == id && p.IdUser == IDUser);
                if (findCartItem == null)
                {
                    Product findsp = db.Products.First(m => m.Productid == id);

                    findCartItem = new CartItem();
                    findCartItem.IdUser = User.Identity.GetUserId();
                    findCartItem.Productid = findsp.Productid;
                    findCartItem.ProductName = findsp.discription;
                    findCartItem.Quantity = 1;
                    findCartItem.Image = findsp.image;
                    findCartItem.Price = findsp.price.Value;
                    db.CartItems.Add(findCartItem);
                }
                else
                {
                    findCartItem.Quantity++;
                }
                db.SaveChanges();
                return "Dat hang thanh cong!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        public RedirectToRouteResult UpdateCart(int id, int txtQuantity)
        {
            var itemFind = db.CartItems.FirstOrDefault(m => m.Id == id);
            if (itemFind != null)
            {
                itemFind.Quantity = txtQuantity;
            }
            return RedirectToAction("Index");
        }

      
        public ActionResult Delete()
        {
            using (var context = new FoodcontextDB())
            {
                var cartItems = context.CartItems.ToList();
                if (cartItems.Any())
                {
                    context.CartItems.RemoveRange(cartItems);
                    context.SaveChanges();
                }
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        
        public ActionResult RemoveCartItem(int Id)
        {
            using (var context = new FoodcontextDB())
            {
                // Lấy mục giỏ hàng cần xóa từ cơ sở dữ liệu
                var item = context.CartItems.FirstOrDefault(x => x.Id == Id);

                if (item != null)
                {
                    // Xóa mục giỏ hàng và lưu lại thay đổi vào cơ sở dữ liệu
                    context.CartItems.Remove(item);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public JsonResult updateQuantity(int ID, int num)
        {
            using (var context = new FoodcontextDB())
            {
                var food = context.CartItems.FirstOrDefault(x => x.Id == ID);
                if (food == null)
                {
                    return Json(new { success = false });
                }
                food.Quantity = num;
                context.SaveChanges();
                var total = food.Quantity * food.Price;
                var totalPrice = context.CartItems.Sum(x => x.Quantity * x.Price);
                var count = context.CartItems.Sum(x => x.Quantity);
                return Json(new { success = true, total = total, totalPrice = totalPrice, count = count });
            }

        }
      
        public ActionResult CartSummary()
        {
            int count = 0;
            var userId = User.Identity.GetUserId();


            var cartItems = db.CartItems.Where(h => h.IdUser == userId).ToList();
            if (cartItems != null)
            {
                count = cartItems.Count;
            }

            return Content(count.ToString());
        }

        //public ActionResult Orders(List<int> listID)
        //{
        //    string currentUserId = User.Identity.GetUserId();
        //    FoodcontextDB context = new FoodcontextDB();

        //    try
        //    {

        //        Order objOrder = new Order()
        //        {
        //            Od_name = currentUserId,
        //            Od_date = DateTime.Now,
        //            Od_note = null,
        //            Od_status = null,
        //            Od_address = null,
        //            idthanhtoan = 1

        //        };

        //        context.Orders.Add(objOrder);
        //        context.SaveChanges();

        //        int newOrderNo = objOrder.Od_id;

        //        List<listOrder> listOrders = getListOrder();

        //        foreach (var id in listID)
        //        {
        //            var cart = context.CartItems.SingleOrDefault(x => x.Id == id);
        //            if (cart != null)
        //            {
        //                Order_detail ctdh = new Order_detail()
        //                {
        //                    Od_id = newOrderNo,
        //                    Productid = cart.Product.Productid,
        //                    tt_money = (double?)(cart.Quantity * cart.Price),
        //                    num = cart.Quantity,
        //                    price = cart.Product.price,
        //                    Storeid = cart.Product.Userid
        //                };
        //                context.CartItems.Remove(cart);
        //                context.Order_detail.Add(ctdh);
        //                context.SaveChanges();
        //            }
                    
        //        }
        //    }
        //    catch (Exception ex){}
        //    return View();
        //}


        public ActionResult Order()
        {
           
            string currentUserId = User.Identity.GetUserId(); 
            FoodcontextDB context = new FoodcontextDB();

            try
            {

                Order objOrder = new Order()
                    {
                        Od_name = currentUserId,
                        Od_date = DateTime.Now,
                        Od_note = null,
                        Od_status = null,
                        Od_address = null,
                        idthanhtoan = 1
                        
                };

                    context.Orders.Add(objOrder);
                    context.SaveChanges();

                    int newOrderNo = objOrder.Od_id; // Giả sử bảng Order có cột "Id" đại diện cho số đơn hàng

                    List<listOrder> listOrders = getListOrder();

                    foreach (var order in listOrders)
                    {
                        var cart = context.CartItems.SingleOrDefault(x => x.Id == order.ID); // Sử dụng tham số 'id' truyền từ AJAX
                        if (cart != null)
                        {
                            Order_detail ctdh = new Order_detail()
                            {
                                Od_id = newOrderNo,
                                Productid = cart.Product.Productid,
                                tt_money = (double?)(cart.Quantity * cart.Price),
                                num = cart.Quantity,
                                price = cart.Product.price,
                                Storeid = cart.Product.Userid
                            };
                            context.CartItems.Remove(cart);
                            context.Order_detail.Add(ctdh);
                            context.SaveChanges();
                        }
                        //else
                        //{
                        //    transaction.Rollback();
                        //}
                    }
                    //transaction.Commit();
                    //RemoveProcessedItemsFromSession(listOrders);
                }catch (Exception ex)
                {
                    //transaction.Rollback();
                }
            getListOrder().Clear();
            return RedirectToAction("Index");
        }


        public ActionResult Payment()
        {
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "test";
            string returnUrl = "https://localhost:44346/ShoppingCart/ConfirmPaymentClient";
            string notifyurl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/Home/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            string amount = "1000";
            string orderid = DateTime.Now.Ticks.ToString(); //mã đơn hàng
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }


        public ActionResult ConfirmPaymentClient(Result result)
        {
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode;

            if (result != null && result.errorCode == "0")
            {
                string currentUserId = User.Identity.GetUserId(); // Lấy thông tin đăng nhập
                FoodcontextDB context = new FoodcontextDB();

                try
                {
                    Order objOrder = new Order()
                    {
                        Od_name = currentUserId,
                        Od_date = DateTime.Now,
                        Od_note = null,
                        Od_status = null,
                        Od_address = null,
                        idthanhtoan = 2
                       
                };
                    context.Orders.Add(objOrder);
                    context.SaveChanges();

                    int newOrderNo = objOrder.Od_id; // Giả sử bảng Order có cột "Id" đại diện cho số đơn hàng
                    List<listOrder> listOrders = getListOrder();

                    foreach (var item in listOrders)
                    {
                        var cart = context.CartItems.SingleOrDefault(x => x.Id == item.ID); // Sử dụng tham số 'id' truyền từ AJAX
                        if (cart != null)
                        {
                            Order_detail ctdh = new Order_detail()
                            {
                                Od_id = newOrderNo,
                                Productid = (int)cart.Productid,
                                tt_money = cart.Quantity * cart.Product.price,
                                price = cart.Product.price,
                                Storeid = cart.Product.Userid,
                                num = cart.Quantity
                            };

                            context.CartItems.Remove(cart);
                            context.Order_detail.Add(ctdh);
                            getListOrder().Clear();
                            context.SaveChanges();
                        }
                    }

                    //return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    //return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet); 
                }
            }

            return View();
        }

        public List<listOrder> getListOrder()
        {
            var listOrders = Session["listOrder"] as List<listOrder>;
            if (listOrders == null)
            {
                listOrders = new List<listOrder>();
                Session["listOrder"] = listOrders;
            }
            return listOrders;
        }
        public JsonResult ClearSession()
        {
            getListOrder().Clear();
            return Json(new { success = true });
        }
        public JsonResult CheckOrder(int id)
        {
            //, int selectedPaymentMethodID
            FoodcontextDB context = new FoodcontextDB();
            List<listOrder> listOrders = getListOrder();
            var cart = context.CartItems.SingleOrDefault(x => x.Id == id);
            listOrder item = new listOrder()
            {
                ID = cart.Id,
                Name = cart.ProductName,
                Pic = cart.Image,
                gia = cart.Price
            };
            listOrders.Add(item);



            return Json(new { success = true });
        }

        public ActionResult OrderItems()
        {
            List<listOrder> listOrders = getListOrder();
            //return View(listOrders);
            return PartialView("OrderItems", listOrders);

        }
        public void RemoveProcessedItemsFromSession(List<listOrder> processedItems)
        {
            List<listOrder> listOrders = Session["listOrder"] as List<listOrder>;
            if (listOrders != null)
            {
                foreach (var processedItem in processedItems)
                {
                    listOrders.Remove(processedItem);
                }
            }
        }




    }
}