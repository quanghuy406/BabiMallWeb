using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BabiMall.Models;

namespace BabiMall.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        //Phần đăng nhập//
        BabiMallEntities database = new BabiMallEntities();
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        //Phần đăng nhập//
        [HttpPost]
        public ActionResult DangNhap(QUANLY admin)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(admin.Tendangnhap))
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống");
                if (string.IsNullOrEmpty(admin.Matkhau))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
                if (ModelState.IsValid)
                {
                    //Tìm khách hàng có tên đăng nhập và password hợp lệ trong CSDL
                    var quanly = database.QUANLies.FirstOrDefault(k => k.Tendangnhap ==
                   admin.Tendangnhap && k.Matkhau == admin.Matkhau);
                    if (quanly != null)
                    {
                        TempData["ThongBao"] = "Đăng nhập admin thành công";
                        //Lưu vào session
                        Session["TaiKhoan"] = quanly;
                    }
                    else
                        TempData["ThongBao"] = "Tài khoản hoặc mật khẩu không chính xác";
                }
            }

            return RedirectToAction("Index");
        }

        private List<KHACHHANG> Laykhachhang(int soluong)
        {
            // Sắp xếp sách theo ngày cập nhật giảm dần, lấy đúng số lượng sách cần
            // Chuyển qua dạng danh sách kết quả đạt được
            return database.KHACHHANGs.OrderBy(kh =>
           kh.MaKH).Take(soluong).ToList();
        }
        // GET: BookStore
        public ActionResult KhachHang()
        {
            // Giả sử cần lấy 5 quyển sách mới cập nhật
            var dsMBMoi = Laykhachhang(10);
            return View(dsMBMoi);
        }
        public ActionResult Index()
        {
            return View();
        }
        private List<MATBANG> LayMBMoi(int soluong)
        {
            // Sắp xếp sách theo ngày cập nhật giảm dần, lấy đúng số lượng sách cần
            // Chuyển qua dạng danh sách kết quả đạt được
            return database.MATBANGs.OrderByDescending(mb =>
           mb.Ngaycapnhat).Take(soluong).ToList();
        }
        // GET: BookStore
        public ActionResult MatBang()
        {
            // Giả sử cần lấy 5 quyển sách mới cập nhật
            var dsMBMoi = LayMBMoi(100);
            return View(dsMBMoi);
        }
        public ActionResult DetailsMatBang(int id)
        {
            var mb = database.MATBANGs.FirstOrDefault(a => a.Mamatbang == id);
            return View(mb);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaCD = new SelectList(database.CHUDEMATBANGs, "MaCD", "TenChuDe");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Mamatbang,Thongtinmatbang,Donvitinh,Dongia,Mota,Tenmatbang,Vitri,Dientich,Dienthoai,Lienhe,MaCD,Ngaycapnhat,Soluongban,solanxem,Trangthaidonhang")]
        MATBANG matbang, HttpPostedFileBase Hinhminhhoa)
        {
            if (ModelState.IsValid)
            {
                if (Hinhminhhoa != null)
                {
                    //Lấy tên file của hình đc up lên
                    var fileName = Path.GetFileName(Hinhminhhoa.FileName);
                    //Tạo đường dẫn
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    //Lưu tên
                    matbang.Hinhminhhoa = fileName;
                    //Save vào Images Folder
                    Hinhminhhoa.SaveAs(path);
                }

                database.MATBANGs.Add(matbang);
                database.SaveChanges();
                return RedirectToAction("MatBang");
            }

            ViewBag.MaCD = new SelectList(database.CHUDEMATBANGs, "MaCD", "TenChuDe", matbang.MaCD);
            return View(matbang);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MATBANG matbang = database.MATBANGs.Find(id);
            if (matbang == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaCD = new SelectList(database.CHUDEMATBANGs, "MaCD", "TenChuDe", matbang.MaCD);
            return View(matbang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Mamatbang,Thongtinmatbang,Donvitinh,Dongia,Mota,Tenmatbang,Vitri,Dientich,Dienthoai,Lienhe,MaCD,Ngaycapnhat,Soluongban,solanxem,Trangthaidonhang")]
        MATBANG matbang, HttpPostedFileBase Hinhminhhoa)
        {
            if (ModelState.IsValid)
            {
                var matbangdb = database.MATBANGs.FirstOrDefault(a => a.Mamatbang == matbang.Mamatbang);
                if (matbangdb != null)
                {
                    matbangdb.Thongtinmatbang = matbang.Thongtinmatbang;
                    matbangdb.Mota = matbang.Mota;
                    matbangdb.Dongia = matbang.Dongia;
                    if (Hinhminhhoa != null)
                    {
                        //Lấy tên file của hình đc up lên
                        var fileName = Path.GetFileName(Hinhminhhoa.FileName);
                        //Tạo đường dẫn
                        var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                        //Lưu tên
                        matbangdb.Hinhminhhoa = fileName;
                        //Save vào Images Folder
                        Hinhminhhoa.SaveAs(path);
                    }
                    matbangdb.MaCD = matbang.MaCD;
                }

                database.SaveChanges();
                return RedirectToAction("MatBang");
            }
            ViewBag.MaCD = new SelectList(database.CHUDEMATBANGs, "MaCD", "TenChuDe", matbang.MaCD);
            return View(matbang);
        }

         public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MATBANG matbang = database.MATBANGs.Find(id);
            if (matbang == null)
            {
                return HttpNotFound();
            }
            return View(matbang);
        }

        // POST: ACCs1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MATBANG matbang = database.MATBANGs.Find(id);
            database.MATBANGs.Remove(matbang);
            database.SaveChanges();
            return RedirectToAction("MatBang");
        }
        public ActionResult DoanhThu()
        {
            // Lấy danh sách đơn thuê mặt bằng từ cơ sở dữ liệu
            List<DONTHUEMATBANG> danhSachDonThue = database.DONTHUEMATBANGs.ToList();

            // Tính tổng doanh thu
            decimal tongDoanhThu = (decimal)danhSachDonThue.Sum(d => d.Trigia);

            // Truyền danh sách đơn thuê và tổng doanh thu qua ViewBag hoặc ViewData
            ViewBag.DanhSachDonThue = danhSachDonThue;
            ViewBag.TongDoanhThu = tongDoanhThu;

            return View();
        }
        public ActionResult DangXuat()
        {
            Session.Abandon();
            return RedirectToAction("DangNhap", "NguoiDung");
        }

        //SUKIENNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN
        [HttpGet]
        public ActionResult CreateSK()
        {
            ViewBag.MaCD = new SelectList(database.CHUDEMATBANGs, "MaCD", "TenChuDe");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSK([Bind(Include = "MaSK,TenSK,Ngaybatdau,Thongtinsukien,Motasukien")]
        SUKIEN sukien, HttpPostedFileBase Anhsukien)
        {
            if (ModelState.IsValid)
            {
                if (Anhsukien != null)
                {
                    //Lấy tên file của hình đc up lên
                    var fileName = Path.GetFileName(Anhsukien.FileName);
                    //Tạo đường dẫn
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    //Lưu tên
                    sukien.Anhsukien = fileName;
                    //Save vào Images Folder
                    Anhsukien.SaveAs(path);
                }

                database.SUKIENs.Add(sukien);
                database.SaveChanges();
                return RedirectToAction("SuKien");
            }

            
            return View(sukien);
        }
    }
}