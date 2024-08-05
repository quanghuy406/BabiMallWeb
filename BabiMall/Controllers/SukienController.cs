using BabiMall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabiMall.Controllers
{
    public class SukienController : Controller
    {
        // GET: Sukien
        BabiMallEntities database = new BabiMallEntities();
        private List<SUKIEN> LaySKMoi(int soluong)
        {
            // Sắp xếp sách theo ngày cập nhật giảm dần, lấy đúng số lượng sách cần
            // Chuyển qua dạng danh sách kết quả đạt được
            return database.SUKIENs.OrderByDescending(sk => sk.Ngaybatdau).Take(soluong).ToList();
        }
        // GET: BookStore
        public ActionResult Index()
        {
            
            var dsSKMoi = LaySKMoi(6);
            return View(dsSKMoi);
        }

        public ActionResult Details(int id)
        {
            var sk = database.SUKIENs.FirstOrDefault(a => a.MaSK == id);
            return View(sk);
        }
    }
}