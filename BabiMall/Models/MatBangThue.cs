using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabiMall.Models
{
    public class MatBangThue
    {
        BabiMallEntities database = new BabiMallEntities();
        
        public int MaMatBang { get; set; }
        public string TenMatBang { get; set; }
        public string Hinhminhhoa { get; set; }
        public double DonGia { get; set; }
       
        public int SoLuong { get; set; }
        public double DienTich { get; set; }

        public bool DaTonTaiTrongGio { get; set; }
        //Tính thành tiền = DongGia * SoLuong
        public double ThanhTien()
        {

            return DienTich * DonGia;
        }
        public MatBangThue(int MaMatBang)
        {
            this.MaMatBang = MaMatBang;
            //Tìm mặt bằng trong CSDL có mã id cần và gán cho mặt hàng được mua
           
            var matbang = database.MATBANGs.Single(s => s.Mamatbang == this.MaMatBang);
            this.TenMatBang = matbang.Thongtinmatbang;
            this.Hinhminhhoa = matbang.Hinhminhhoa;
           
            this.DienTich = Convert.ToDouble(matbang.Dientich);
            this.DonGia = double.Parse(matbang.Dongia.ToString());
            this.SoLuong =1; //Số lượng mua ban đầu của một mặt hàng là 1 (cho lần click  đầu)
        }
    }
}