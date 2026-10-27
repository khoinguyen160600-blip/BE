using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BE.Models;

namespace BE.Controllers
{
    public class CartController : Controller
    {
        MyStoreEntities db = new MyStoreEntities();

        // 1. Hàm lấy giỏ hàng hiện tại (nếu chưa có thì tạo mới)
        public List<CartItem> GetCart()
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // 2. Hàm thêm sản phẩm vào giỏ
        public ActionResult AddToCart(int id)
        {
            List<CartItem> cart = GetCart();
            CartItem item = cart.FirstOrDefault(p => p.ProductID == id);

            if (item != null)
            {
                item.Quantity++; // Đã có rồi thì tăng số lượng
            }
            else
            {
                Product prod = db.Products.Find(id); // Chưa có thì lấy từ DB thêm vào
                if (prod != null)
                {
                    cart.Add(new CartItem
                    {
                        ProductID = prod.ProductID,
                        ProductName = prod.ProductName,
                        ProductImage = prod.ProductImage,
                        UnitPrice = (decimal)prod.ProductPrice,
                        Quantity = 1
                    });
                }
            }
            return RedirectToAction("Index", "Cart"); // Chuyển sang xem giỏ hàng
        }

        // 3. Hàm hiển thị trang Giỏ hàng
        public ActionResult Index()
        {
            return View(GetCart());
        }

        // 4. Hàm xóa sản phẩm khỏi giỏ hàng
        public ActionResult RemoveFromCart(int id)
        {
            // Lấy giỏ hàng hiện tại
            List<CartItem> cart = GetCart();

            // Tìm sản phẩm cần xóa dựa vào ID
            CartItem item = cart.FirstOrDefault(p => p.ProductID == id);

            // Nếu thấy thì xóa khỏi danh sách
            if (item != null)
            {
                cart.Remove(item);
            }

            // Xóa xong thì load lại trang Giỏ hàng
            return RedirectToAction("Index", "Cart");


        }

        [HttpPost]
        public ActionResult Checkout(string customerName, string customerPhone, string customerAddress)
        {
            List<CartItem> cart = GetCart();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index");
            }

            // Tạo mới một hóa đơn (Order) khớp với các cột thực tế trong Order.cs
            Order order = new Order();
            order.CustomerID = 1; // Gán tạm mã khách hàng mặc định (hoặc lấy từ Session đăng nhập nếu có)
            order.OrderDate = System.DateTime.Now;
            order.TotalAmount = cart.Sum(x => x.TotalPrice);
            order.AddressDelivery = customerAddress; // Khớp với cột AddressDelivery trong Order.cs
            order.PaymentStatus = "Chưa thanh toán";

            db.Orders.Add(order);
            db.SaveChanges(); // Lưu để sinh ra OrderID mới nhất
            order.CustomerID = 1; // Xóa dòng này đi nếu bảng Order cho phép giá trị Null

            // Lưu từng sản phẩm trong giỏ hàng vào bảng Chi tiết đơn hàng (OrderDetail)
            foreach (var item in cart)
            {
                OrderDetail detail = new OrderDetail();
                detail.OrderID = order.OrderID;
                detail.ProductID = item.ProductID;
                detail.Quantity = item.Quantity;
                detail.UnitPrice = item.UnitPrice; // Khớp với cột UnitPrice trong OrderDetail.cs

                db.OrderDetails.Add(detail);
            }
            db.SaveChanges();

            // Thanh toán xong thì xóa sạch Session giỏ hàng
            Session["Cart"] = null;

            // Chuyển sang trang thông báo thành công
            return RedirectToAction("OrderSuccess");
        }

        // Trang thông báo đặt hàng thành công
        public ActionResult OrderSuccess()
        {
            return View();
        }

        public ActionResult Checkout()
        {
            if (Session["Cart"] == null || !((List<CartItem>)Session["Cart"]).Any())
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}