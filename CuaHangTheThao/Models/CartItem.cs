using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CuaHangTheThao.Models;

namespace CuaHangTheThao.Models
{
    public class CartItem
    {
        public string isan_pham_id { get; set; }
        public string iten { get; set; }
        public string ihinh_anh_url { get; set; }
        public double igia { get; set; }
        public int iso_luong { get; set; }
        public double thanhtien
        {
            get { return iso_luong * igia; }
        }
        private csdl_cuahangthethaoDataContext data = new csdl_cuahangthethaoDataContext();
        public CartItem(string san_pham_id)
        {
            var sp = data.SanPhams.SingleOrDefault(n => n.san_pham_id == san_pham_id);
            if (sp != null)
            {
                isan_pham_id = san_pham_id;
                iten = sp.ten;
                ihinh_anh_url = sp.hinh_anh_url;
                igia = double.Parse(sp.gia.ToString()); // Truyền trực tiếp giá từ cơ sở dữ liệu

                iso_luong = 1;
            }
            

        }    
        
    }
}