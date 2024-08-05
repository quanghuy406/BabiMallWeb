using BabiMall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BabiMall.Controllers
{
    public class GioHangController : Controller
    {
        

        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }

        public List<MatBangThue> LayGioHang()
        {
            List<MatBangThue> gioHang = Session["GioHang"] as List<MatBangThue>;
            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào Session
            if (gioHang == null)
            {
                gioHang = new List<MatBangThue>();
                Session["GioHang"] = gioHang;
            }
            return gioHang;
        }
        public RedirectToRouteResult ThemSanPhamVaoGio(int Mamatbang)
        {


            MATBANG matBang = database.MATBANGs.FirstOrDefault(m => m.Mamatbang == Mamatbang); // Hàm này cần được cài đặt phù hợp với logic của bạn

            if (matBang != null && matBang.Trangthaidonhang == "Sold out")
            {
                TempData["IsSoldOut"] = true;
                return RedirectToAction("Details","MatBang", new { id = Mamatbang });
            }
            else
            {
                //Lấy giỏ hàng hiện tại
                List<MatBangThue> gioHang = LayGioHang();
                //Kiểm tra xem có tồn tại mặt hàng trong giỏ hay chưa
                //Nếu có thì tăng số lượng lên 1, ngược lại thêm vào giỏ
                MatBangThue sanPham = gioHang.FirstOrDefault(s => s.MaMatBang == Mamatbang);
                if (sanPham == null) //Sản phẩm chưa có trong giỏ
                {
                    sanPham = new MatBangThue(Mamatbang);
                    gioHang.Add(sanPham);
                }
                else
                {
                    sanPham.DaTonTaiTrongGio = true; //Sản phẩm đã có trong giỏ thì tăng số lượng lên 1
                }

                if (sanPham.DaTonTaiTrongGio)
                {
                    // Hiển thị thông báo mặt bằng đã tồn tại trong giỏ
                    Console.WriteLine("Mặt bằng đã tồn tại trong giỏ hàng.");
                }
                else
                {
                    // Thêm sản phẩm vào giỏ thành công, có thể thực hiện các hành động khác
                    Console.WriteLine("Thêm sản phẩm vào giỏ hàng thành công.");
                }
            }

            return RedirectToAction("Index", "MatBang");
        }
        private int TinhTongSL()
        {
            int tongSL = 0;
            List<MatBangThue> gioHang = LayGioHang();
            if (gioHang != null)
                tongSL = (int)gioHang.Sum(sp => sp.DienTich);
            return tongSL;
        }
        private double TinhTongTien()
        {
            double TongTien = 0;
            List<MatBangThue> giohang = LayGioHang();
            if (giohang != null)
                TongTien = giohang.Sum(sp => sp.ThanhTien());
            return TongTien;
        }
        public ActionResult HienThiGioHang()
        {
            List<MatBangThue> giohang = LayGioHang();
            //Nếu giỏ hàng trống thì trả về trang ban đầu
            if (giohang == null || giohang.Count == 0)
            {
                return RedirectToAction("Index", "MatBang");
            }
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return View(giohang); //Trả về View hiển thị thông tin giỏ hàng
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return PartialView();
        }
        public ActionResult XoaMatHang(int Mamatbang)
        {
            List<MatBangThue> gioHang = LayGioHang();
            //Lấy sản phẩm trong giỏ hàng
            var sanpham = gioHang.FirstOrDefault(s => s.MaMatBang == Mamatbang);
            if (sanpham != null)
            {
                gioHang.RemoveAll(s => s.MaMatBang == Mamatbang);
                return RedirectToAction("HienThiGioHang"); //Quay về trang giỏ hàng
            }
            if (gioHang.Count == 0) //Quay về trang chủ nếu giỏ hàng không có gì
                return RedirectToAction("Index", "MatBang");
            return RedirectToAction("HienThiGioHang");
        }

        
        public ActionResult DatHang()
        {
            
            if (Session["TaiKhoan"] == null) //Chưa đăng nhập
                return RedirectToAction("DangNhap", "NguoiDung");
            List<MatBangThue> gioHang = LayGioHang();
            if (gioHang == null || gioHang.Count == 0) //Chưa có giỏ hàng hoặc chưa có sp
                return RedirectToAction("Index", "MatBang");
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return View(gioHang); //Trả về View hiển thị thông tin giỏ hàng
        }
        BabiMallEntities database = new BabiMallEntities();
        //Xác nhận đơn và lưu vào CSDL
       
        public ActionResult DongYDatHang()
        {
            // Tìm mặt bằng dựa trên id_mat_bang
            
            KHACHHANG khach = Session["TaiKhoan"] as KHACHHANG; //Khách
            List<MatBangThue> gioHang = LayGioHang(); //Giỏ hàng
            DONTHUEMATBANG DonHang = new DONTHUEMATBANG(); //Tạo mới đơn đặt hàng
            DonHang.MaKH = khach.MaKH;
            DonHang.Ngaythue = DateTime.Now;
            DonHang.Trigia = (decimal)TinhTongTien();
            
            DonHang.Tennguoithue = khach.HoTenKH;
            DonHang.Dienthoai = khach.DienthoaiKH;
            DonHang.HTThanhtoan = false;
           

            database.DONTHUEMATBANGs.Add(DonHang);
            database.SaveChanges();
            //Lần lượt thêm từng chi tiết cho đơn hàng trên
            foreach (var sanpham in gioHang)
            {
                CTTHUEMATBANG chitiet = new CTTHUEMATBANG();
                
                chitiet.Somatbang = DonHang.Somatbang;
                chitiet.Mamatbang = sanpham.MaMatBang;
                chitiet.Sodientichthue = (int?)sanpham.DienTich;
                chitiet.Dongia = (decimal)sanpham.DonGia;
                database.CTTHUEMATBANGs.Add(chitiet);
            }
            database.SaveChanges();
            //Xóa giỏ hàng
            Session["GioHang"] = null;
            return RedirectToAction("HoanThanhDonHang");
        }

        public ActionResult HoanThanhDonHang()
        {
            return View();
        }


        public ActionResult XemDonHang()
        {

            if (Session["TaiKhoan"] == null) //Chưa đăng nhập
                return RedirectToAction("DangNhap", "NguoiDung");
            // Lấy dữ liệu từ bảng DONTHUEMATBANG trong CSDL
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            var donHangs = database.CTTHUEMATBANGs.ToList();

            // Truyền dữ liệu vào View
            return View(donHangs);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaKH,HoTenKH,DienthoaiKH,Email,Diachi")]
        KHACHHANG kh)
        {

            if (ModelState.IsValid)
            {
                if (Session["TaiKhoan"] == null) //Chưa đăng nhập
                    return RedirectToAction("DangNhap", "NguoiDung");
                var khachhang = database.KHACHHANGs.FirstOrDefault(a => a.MaKH == kh.MaKH);
                if (khachhang != null)
                {
                    khachhang.HoTenKH = kh.HoTenKH;
                    khachhang.DienthoaiKH = kh.DienthoaiKH;
                    khachhang.Email = kh.Email;
                    khachhang.Diachi = kh.Diachi;
                }
                database.SaveChanges();
                return RedirectToAction("Index", "SuKien");
            }
            return View(kh);
        }
    }
}