using BabiMall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabiMall.Controllers
{
    public class MatBangController : Controller
    {
        // GET: MatBang
        BabiMallEntities database = new BabiMallEntities();
        private List<MATBANG> LayMBMoi(int soluong)
        {
            // Sắp xếp sách theo ngày cập nhật giảm dần, lấy đúng số lượng sách cần
            // Chuyển qua dạng danh sách kết quả đạt được
            return database.MATBANGs.OrderByDescending(mb =>
           mb.Ngaycapnhat).Take(soluong).ToList();
        }
        // GET: BookStore
        public ActionResult Index()
        {
            // Giả sử cần lấy 5 quyển sách mới cập nhật
            var dsMBMoi = LayMBMoi(5);
            return View(dsMBMoi);
        }
        public ActionResult LayChuDe()
        {
            var dsChuDe = database.CHUDEMATBANGs.ToList();
            return PartialView(dsChuDe);
        }
        public ActionResult SPTheoChuDe(int id)
        {
            var dsMbtheochude = database.MATBANGs.Where(mb => mb.MaCD == id).ToList();
            return View("Index", dsMbtheochude);

        }
        public ActionResult Details(int id)
        {
            var mb = database.MATBANGs.FirstOrDefault(a => a.Mamatbang== id);
            return View(mb);
        }
    }
}