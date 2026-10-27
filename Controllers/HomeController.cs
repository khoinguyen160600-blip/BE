using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BE.Models;

namespace BE.Controllers
{
    public class HomeController : Controller
    {
        // Khởi tạo đối tượng kết nối CSDL
        MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index()
        {
            // Lấy toàn bộ danh sách sản phẩm từ bảng Product
            var products = db.Products.ToList();

            // Truyền danh sách này ra ngoài giao diện (View)
            return View(products);
        }

        // Hàm xem chi tiết sản phẩm
        public ActionResult Details(int id)
        {
            // Tìm sản phẩm trong CSDL dựa vào ID truyền vào
            var product = db.Products.Find(id);

            // Nếu không tìm thấy, báo lỗi 404
            if (product == null)
            {
                return HttpNotFound();
            }

            // Nếu tìm thấy, truyền dữ liệu sản phẩm đó ra View
            return View(product);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}