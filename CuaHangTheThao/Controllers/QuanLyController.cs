using CuaHangTheThao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuaHangTheThao.Controllers
{
    public class QuanLyController : Controller
    {
        // GET: QuanLy
        csdl_cuahangthethaoDataContext data = new csdl_cuahangthethaoDataContext();
        public ActionResult Index()
        {
            List<DonHang> hd = data.DonHangs.ToList();
            return View(hd);
        }

        public ActionResult ThongKeHoaDon(string mhd)
        {
            List<ChiTietDonHang> dsct = data.ChiTietDonHangs.Where(ct => ct.don_hang_id.Equals(mhd)).ToList();
            ViewBag.thongke = "(SL " + dsct.Sum(ct => ct.so_luong) +
                "/" + string.Format("{0:#,##}", dsct.Sum(ct => ct.so_luong * ct.SanPham.gia) + ")");
            return PartialView();
        }

        [HttpPost]
        public ActionResult DuyetDonHang(string[] selectedOrders, string redirectUrl)
        {
            if (selectedOrders != null && selectedOrders.Length > 0)
            {
                foreach (var orderId in selectedOrders)
                {
                    var donHang = data.DonHangs.FirstOrDefault(dh => dh.don_hang_id == orderId);
                    if (donHang != null)
                    {
                        donHang.trang_thai = "Đã giao";
                    }
                }

                data.SubmitChanges();
            }

            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return Redirect(redirectUrl);
            }
            return RedirectToAction("Index");
        }



        public ActionResult XemChiTiet(string mhd)
        {
            List<ChiTietDonHang> dsct = data.ChiTietDonHangs.Where(ct => ct.don_hang_id.Equals(mhd)).ToList();

            return PartialView(dsct);
        }

        public ActionResult QLyDonHangChuaGiao()
        {
            List<DonHang> chuaGiao = data.DonHangs.Where(dh => dh.trang_thai == null).ToList();
            return View(chuaGiao);
        }

        public ActionResult QLyDonHangDaGiao()
        {
            List<DonHang> daGiao = data.DonHangs.Where(dh => dh.trang_thai == "Đã giao").ToList();
            return View(daGiao);
        }


        public ActionResult SanPham()
        {
            List<SanPham> sp = data.SanPhams.ToList();
            return View(sp);
        }
        public ActionResult ThemSanPham()
        {
            ViewBag.DanhMuc = data.DanhMucSanPhams.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult ThemSanPham(SanPham sp, HttpPostedFileBase hinh_anh)
        {
            if (ModelState.IsValid)
            {
                if (hinh_anh != null && hinh_anh.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(hinh_anh.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Images/"), fileName);
                    hinh_anh.SaveAs(path);
                    sp.hinh_anh_url = fileName;
                }

                sp.san_pham_id = Guid.NewGuid().ToString();
                sp.ngay_tao = DateTime.Now;

                data.SanPhams.InsertOnSubmit(sp);
                data.SubmitChanges();

                return RedirectToAction("SanPham");
            }

            ViewBag.DanhMuc = data.DanhMucSanPhams.ToList();
            return View(sp);
        }

        public ActionResult SuaSanPham(string id)
        {
            var sp = data.SanPhams.FirstOrDefault(s => s.san_pham_id == id);
            ViewBag.DanhMuc = data.DanhMucSanPhams.ToList();
            return View(sp);
        }

        [HttpPost]
        public ActionResult SuaSanPham(SanPham sp, HttpPostedFileBase hinh_anh)
        {
            if (ModelState.IsValid)
            {
                var spToUpdate = data.SanPhams.FirstOrDefault(s => s.san_pham_id == sp.san_pham_id);
                if (spToUpdate != null)
                {
                    spToUpdate.ten = sp.ten;
                    spToUpdate.mo_ta = sp.mo_ta;
                    spToUpdate.gia = sp.gia;
                    spToUpdate.danh_muc_id = sp.danh_muc_id;
                    spToUpdate.so_luong_ton = sp.so_luong_ton;

                    if (hinh_anh != null && hinh_anh.ContentLength > 0)
                    {
                        var fileName = System.IO.Path.GetFileName(hinh_anh.FileName);
                        var path = System.IO.Path.Combine(Server.MapPath("~/Images/"), fileName);
                        hinh_anh.SaveAs(path);
                        spToUpdate.hinh_anh_url = fileName;
                    }

                    data.SubmitChanges();
                    return RedirectToAction("SanPham");
                }
            }

            ViewBag.DanhMuc = data.DanhMucSanPhams.ToList();
            return View(sp);
        }

        [HttpPost]
        public ActionResult XoaSanPham(string id)
        {
            var sp = data.SanPhams.FirstOrDefault(s => s.san_pham_id == id);
            if (sp != null)
            {
                data.SanPhams.DeleteOnSubmit(sp);
                data.SubmitChanges();
            }
            return RedirectToAction("SanPham");
        }

        public ActionResult DanhMuc()
        {
            List<DanhMucSanPham> dm = data.DanhMucSanPhams.ToList();
            return View(dm);
        }

        public ActionResult ThemDanhMuc()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemDanhMuc(DanhMucSanPham dm)
        {
            if (ModelState.IsValid)
            {
                dm.danh_muc_id = Guid.NewGuid().ToString();

                data.DanhMucSanPhams.InsertOnSubmit(dm);
                data.SubmitChanges();

                return RedirectToAction("DanhMuc");
            }

            return View(dm);
        }

        public ActionResult SuaDanhMuc(string id)
        {
            var dm = data.DanhMucSanPhams.FirstOrDefault(d => d.danh_muc_id == id);
            return View(dm);
        }

        [HttpPost]
        public ActionResult SuaDanhMuc(DanhMucSanPham dm)
        {
            if (ModelState.IsValid)
            {
                var dmToUpdate = data.DanhMucSanPhams.FirstOrDefault(d => d.danh_muc_id == dm.danh_muc_id);
                if (dmToUpdate != null)
                {
                    dmToUpdate.ten = dm.ten;
                    dmToUpdate.mo_ta = dm.mo_ta;

                    data.SubmitChanges();
                    return RedirectToAction("DanhMuc");
                }
            }

            return View(dm);
        }

        [HttpPost]
        public ActionResult XoaDanhMuc(string id)
        {
            var dm = data.DanhMucSanPhams.FirstOrDefault(d => d.danh_muc_id.Equals(id));
            if (dm == null)
            {
                return HttpNotFound();
            }

            data.DanhMucSanPhams.DeleteOnSubmit(dm);
            data.SubmitChanges();
            return RedirectToAction("DanhMuc");
        }



        public ActionResult NguoiDungQL()
        {
            List<NguoiDung> users = data.NguoiDungs.ToList();
            return View(users);
        }

        public ActionResult ThemNguoiDung()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemNguoiDung(NguoiDung nd)
        {
            if (ModelState.IsValid)
            {
                nd.nguoi_dung_id = Guid.NewGuid().ToString();
                nd.ngay_dang_ky = DateTime.Now;
                nd.mat_khau = "1";

                data.NguoiDungs.InsertOnSubmit(nd);
                data.SubmitChanges();

                return RedirectToAction("NguoiDungQL");
            }

            return View(nd);
        }

        public ActionResult SuaNguoiDung(string id)
        {
            var nd = data.NguoiDungs.FirstOrDefault(n => n.nguoi_dung_id == id);
            return View(nd);
        }

        [HttpPost]
        public ActionResult SuaNguoiDung(NguoiDung nd)
        {
            if (ModelState.IsValid)
            {
                var ndToUpdate = data.NguoiDungs.FirstOrDefault(n => n.nguoi_dung_id == nd.nguoi_dung_id);
                if (ndToUpdate != null)
                {
                    ndToUpdate.ten_dang_nhap = nd.ten_dang_nhap;
                    ndToUpdate.email = nd.email;
                    ndToUpdate.ten = nd.ten;
                    ndToUpdate.ho = nd.ho;
                    ndToUpdate.so_dien_thoai = nd.so_dien_thoai;
                    ndToUpdate.dia_chi = nd.dia_chi;

                    data.SubmitChanges();
                    return RedirectToAction("NguoiDungQL");
                }
            }

            return View(nd);
        }

        public ActionResult ResetMatKhau(string id)
        {
            var nd = data.NguoiDungs.FirstOrDefault(n => n.nguoi_dung_id == id);
            if (nd != null)
            {
                nd.mat_khau = "1"; 
                data.SubmitChanges();
            }
            return RedirectToAction("NguoiDungQL");
        }

        [HttpPost]
        public ActionResult XoaNguoiDung(string id)
        {
            var nd = data.NguoiDungs.FirstOrDefault(n => n.nguoi_dung_id == id);
            if (nd != null)
            {
                data.NguoiDungs.DeleteOnSubmit(nd);
                data.SubmitChanges();
            }
            return RedirectToAction("NguoiDungQL");
        }


        public ActionResult Admin()
        {
            List<QuanTriVien> admins = data.QuanTriViens.ToList();
            return View(admins);
        }

        public ActionResult ThemQuanTriVien()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemQuanTriVien(QuanTriVien qtv)
        {
            if (ModelState.IsValid)
            {
                qtv.quan_tri_vien_id = Guid.NewGuid().ToString();
                qtv.ngay_tao = DateTime.Now;
                qtv.mat_khau = "default_password"; // Set a default password or use a method to generate one

                data.QuanTriViens.InsertOnSubmit(qtv);
                data.SubmitChanges();

                return RedirectToAction("Index");
            }

            return View(qtv);
        }

        public ActionResult SuaQuanTriVien(string id)
        {
            var qtv = data.QuanTriViens.FirstOrDefault(q => q.quan_tri_vien_id == id);
            return View(qtv);
        }

        [HttpPost]
        public ActionResult SuaQuanTriVien(QuanTriVien qtv)
        {
            if (ModelState.IsValid)
            {
                var qtvToUpdate = data.QuanTriViens.FirstOrDefault(q => q.quan_tri_vien_id == qtv.quan_tri_vien_id);
                if (qtvToUpdate != null)
                {
                    qtvToUpdate.ten_dang_nhap = qtv.ten_dang_nhap;
                    qtvToUpdate.email = qtv.email;
                    qtvToUpdate.vai_tro = qtv.vai_tro;

                    data.SubmitChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(qtv);
        }

        public ActionResult ResetMatKhauQTV(string id)
        {
            var qtv = data.QuanTriViens.FirstOrDefault(q => q.quan_tri_vien_id == id);
            if (qtv != null)
            {
                qtv.mat_khau = "123"; // Reset to a new default password or use a method to generate one
                data.SubmitChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult XoaQuanTriVien(string id)
        {
            var qtv = data.QuanTriViens.FirstOrDefault(q => q.quan_tri_vien_id == id);
            if (qtv != null)
            {
                data.QuanTriViens.DeleteOnSubmit(qtv);
                data.SubmitChanges();
            }
            return RedirectToAction("Index");
        }

    }
}