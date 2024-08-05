using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BabiMall.Models;

namespace BabiMall.Controllers
{
    public class DanhmucController : Controller
    {
        BabiMallEntities database = new BabiMallEntities();
        // GET: Nhahang

        public ActionResult FilterResults(bool? Coffee, bool? nhahang,bool? thoitrang,bool? dichvu,int page = 1)
        {

            int itemsPerPage = 9; // Số danh mục trên mỗi trang

            List<DANHMUC> filteredCategories = GetFilteredDataFromDatabase(Coffee, nhahang, thoitrang, dichvu);

            // Tính tổng số trang dựa trên số danh mục và số danh mục trên mỗi trang
            int totalItems = filteredCategories.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

            // Lấy danh sách danh mục cho trang hiện tại
            List<DANHMUC> categoriesOnPage = filteredCategories.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(categoriesOnPage);
        }

        private List<DANHMUC> GetFilteredDataFromDatabase(bool? Coffee, bool? nhahang, bool? thoitrang, bool? dichvu)
        {
            using (var context = new BabiMallEntities())
            {
                IQueryable<DANHMUC> query = context.DANHMUCs; // Lấy DbSet từ DbContext

                if (Coffee == true)
                {
                    query = query.Where(n => n.Loaidoanhmuc == "Coffee & Tea");
                }

                if (nhahang == true)
                {
                    query = query.Where(n => n.Loaidoanhmuc == "Nhà hàng");
                }

                if (thoitrang == true)
                {
                    query = query.Where(n => n.Loaidoanhmuc == "Thời trang");
                }

                if (dichvu == true)
                {
                    query = query.Where(n => n.Loaidoanhmuc == "Giải trí - Dịch vụ");
                }

                List<DANHMUC> results = query.ToList();

                return results;
            }
        }

       
    }
}