using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangTheThao.Models
{
    public class GioHangmodel
    {
        public List<CartItem> lst { get; set; }

        // Constructor không tham số
        public GioHangmodel()
        {
            lst = new List<CartItem>();
        }

        // Constructor có tham số
        public GioHangmodel(List<CartItem> lstGH)
        {
            lst = lstGH;
        }

        // Phương thức tính số mặt hàng
        public int SoMatHang()
        {
            return lst.Count;
        }

        // Phương thức tính tổng số lượng hàng
        public int TongSLHang()
        {
            return lst.Sum(item => item.iso_luong);
        }

        // Phương thức tính tổng thành tiền
        public double TongThanhTien()
        {
            return lst.Sum(item => item.thanhtien);
        }

        // Phương thức thêm sản phẩm vào giỏ hàng
        public int Them(string msp, int isoluong)
        {
            CartItem sp = lst.Find(n => n.isan_pham_id == msp);
            if (sp == null)
            {
                CartItem spp = new CartItem(msp);
                if (spp == null) return -1;

                spp.iso_luong = isoluong > 0 ? isoluong : 1;

                lst.Add(spp);
            }
            else
            {
                sp.iso_luong += isoluong;
                if (
                    sp.iso_luong < 1)
                    sp.iso_luong = 1;
            }
            return 1;
        }

    }
}