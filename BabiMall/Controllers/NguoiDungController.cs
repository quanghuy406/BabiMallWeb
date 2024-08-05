using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BabiMall.Models;

namespace BabiMall.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        BabiMallEntities database = new BabiMallEntities();
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(KHACHHANG kh)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(kh.HoTenKH))
                    ModelState.AddModelError(string.Empty, "Họ tên không được để trống");
                if (string.IsNullOrEmpty(kh.TenDN))
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống");
                if (string.IsNullOrEmpty(kh.Matkhau))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được đểtrống");
                if (string.IsNullOrEmpty(kh.Email))
                    ModelState.AddModelError(string.Empty, "Email không được để trống");
                if (string.IsNullOrEmpty(kh.DienthoaiKH))
                    ModelState.AddModelError(string.Empty, "Điện thoại không được để trống");
                if (string.IsNullOrEmpty(kh.Diachi))
                    ModelState.AddModelError(string.Empty, "Địa chỉ không được để trống");
                //Kiểm tra xem có người nào đã đăng kí với tên đăng nhập này hay chưa
                var khachhang = database.KHACHHANGs.FirstOrDefault(k => k.TenDN == kh.TenDN);
                if (khachhang != null)
                    ModelState.AddModelError(string.Empty, "Đã có người đăng kí tên này");
                if (ModelState.IsValid)
                {
                    database.KHACHHANGs.Add(kh);
                    database.SaveChanges();
                }
                else
                {
                    return View();
                }
            }
            return RedirectToAction("DangNhap");
        }
        //Phần đăng nhập//
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        //Phần đăng nhập//
        [HttpPost]
        public ActionResult DangNhap(KHACHHANG kh)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(kh.TenDN))
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống");
            if (string.IsNullOrEmpty(kh.Matkhau))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
            if (ModelState.IsValid)
                {
                    //Tìm khách hàng có tên đăng nhập và password hợp lệ trong CSDL
                    var khach = database.KHACHHANGs.FirstOrDefault(k => k.TenDN ==
                   kh.TenDN && k.Matkhau == kh.Matkhau);
                    if (khach != null)
                    {
                        TempData["SuccessMessage"] = "Đăng nhập thành công";
                        //Lưu vào session
                        Session["TaiKhoan"] = khach;
                    }
                    else
                        TempData["SuccessMessage"] = "Tài khoản hoặc mật khẩu không chính xác";
                }
            }
            
            return View();
        }
        public ActionResult DangXuat()
        {
            Session.Abandon();
            return RedirectToAction("DangNhap", "NguoiDung");
        }

        
    }
}