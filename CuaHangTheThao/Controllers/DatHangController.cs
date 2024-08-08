using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CuaHangTheThao.Models;
namespace CuaHangTheThao.Controllers
{
    public class DatHangController : Controller
    {
        csdl_cuahangthethaoDataContext data = new csdl_cuahangthethaoDataContext();
        // GET: DatHang
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ThemMatHang(string msp, int iso_luong = 1)
        {
            GioHangmodel ghm = (GioHangmodel)Session["ghm"];
            if (ghm == null)
                ghm = new GioHangmodel();
            ghm.Them(msp, iso_luong);
            Session["ghm"] = ghm;


            return RedirectToAction("trangchu","Home");
        }


        public ActionResult XemGioHang()
        {
            GioHangmodel ghm = (GioHangmodel)Session["ghm"];
           

            return View(ghm);
        }

        public ActionResult SuaMatHang(string msp)
        {
            GioHangmodel ghm = (GioHangmodel)Session["ghm"];
            if (ghm == null)
                return RedirectToAction("XemGioHang");

            CartItem item = ghm.lst.FirstOrDefault(i => i.isan_pham_id == msp);
            if (item == null)
                return RedirectToAction("XemGioHang");

            return View(item);
        }
        [HttpPost]
        public ActionResult CapNhatSoLuong(string msp, int soLuong)
        {
            GioHangmodel ghm = (GioHangmodel)Session["ghm"];
            if (ghm != null)
            {
                var item = ghm.lst.FirstOrDefault(i => i.isan_pham_id == msp);
                if (item != null)
                {
                    item.iso_luong = soLuong;
                }
            }

            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult SuaGioHang(string msp, int quantityChange)
        {
            if (string.IsNullOrEmpty(msp))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid product ID");
            }

            GioHangmodel ghm = Session["ghm"] as GioHangmodel;
            if (ghm == null)
            {
                return RedirectToAction("XemGioHang");
            }

            CartItem item = ghm.lst.FirstOrDefault(i => i.isan_pham_id == msp);
            if (item == null)
            {
                return RedirectToAction("XemGioHang");
            }

            if (quantityChange < 0)
            {
                quantityChange = 0;
            }

            item.iso_luong = quantityChange;
            Session["ghm"] = ghm;

            return RedirectToAction("XemGioHang");
        }
        public ActionResult XoaMatHang(string msp)
        {
            GioHangmodel ghm = (GioHangmodel)Session["ghm"];
            if (ghm != null)
            {
                CartItem item = ghm.lst.FirstOrDefault(i => i.isan_pham_id == msp);
                if (item != null)
                {
                    ghm.lst.Remove(item);
                    Session["ghm"] = ghm;
                }
            }
            return RedirectToAction("XemGioHang");
        }
        public ActionResult XoaTatCaMatHang()
        {
            GioHangmodel ghm = (GioHangmodel)Session["ghm"];
            if (ghm != null)
            {
                // Xóa tất cả các mặt hàng trong giỏ hàng
                ghm.lst.Clear();
                Session["ghm"] = ghm;
            }

            // Chuyển hướng người dùng đến trang giỏ hàng
            return RedirectToAction("XemGioHang");
        }

        public ActionResult XacNhanDatHang()
        {
            if (Session["nd"] == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }  

            GioHangmodel ghm = Session["ghm"] as GioHangmodel;
            if (ghm == null || !ghm.lst.Any())
            {
                return RedirectToAction("XemGioHang");
            }

            NguoiDung nd = Session["kh"] as NguoiDung;
            ViewBag.n = nd;

            return View(ghm);
        }
        [HttpPost]
        public ActionResult XLDatHang(FormCollection c)
        {   
            NguoiDung khach = Session["nd"] as NguoiDung;

            string donHangId = Guid.NewGuid().ToString();
            DonHang dh = new DonHang
            {
                don_hang_id= donHangId,
                nguoi_dung_id = khach.nguoi_dung_id,
                ngay_dat_hang = DateTime.Now,
            };
             

            data.DonHangs.InsertOnSubmit(dh);
            data.SubmitChanges();

            GioHangmodel ghm = Session["ghm"] as GioHangmodel;

            foreach (CartItem item in ghm.lst)
            {
                // Tạo giá trị cho chi_tiet_gio_hang_id, có thể sử dụng GUID hoặc một phương pháp sinh ID khác
                string chiTietDonHangId = Guid.NewGuid().ToString();

                ChiTietDonHang ctgh = new ChiTietDonHang
                {
                    chi_tiet_don_hang_id = chiTietDonHangId,
                    don_hang_id = donHangId,
                    san_pham_id = item.isan_pham_id,
                    so_luong = item.iso_luong,
                };

                data.ChiTietDonHangs.InsertOnSubmit(ctgh);
            }

            data.SubmitChanges();

            Session["ghm"] = null;

            ViewBag.tb = 1;


            return View();
        }
    }
}