using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuaHangTheThao.Models;

namespace CuaHangTheThao.Controllers
{
    public class HomeController : Controller
    {
        csdl_cuahangthethaoDataContext data = new csdl_cuahangthethaoDataContext();
        // GET: Home
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult trangchu(string danhMucId = null)
        {
            IQueryable<SanPham> spQuery = data.SanPhams;

            if (!string.IsNullOrEmpty(danhMucId))
            {
                spQuery = spQuery.Where(sp => sp.danh_muc_id == danhMucId);
            }

            List<SanPham> SP = spQuery.ToList();
            return View(SP);
        }
        public ActionResult DangNhap()
        {

            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection col)
        {
            var username = col["txtname"];
            var password = col["txtpass"];

            NguoiDung nd = data.NguoiDungs.FirstOrDefault(k => k.ten_dang_nhap == username && k.mat_khau == password);

            if (nd != null)
            {
                Session["nd"] = nd;
                return RedirectToAction("trangchu");
            }

            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View();
        }
        //Tạo nút thêm mới sản phẩm:
        //public ActionResult ThemMoi()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult ThemMoi(FormCollection collection ,SanPham sp)
        //{
        //    var ThemSanPham = collection["ten"];
        //    sp.ten = ThemSanPham;
        //    data.SanPhams.InsertOnSubmit(sp);
        //    data.SubmitChanges();
        //    return RedirectToAction("trangchu");
        //    return this.ThemMoi();

        //}
        public ActionResult DanhMucSanPham()
        {
            List<DanhMucSanPham> dmsp = data.DanhMucSanPhams.ToList();
            return PartialView(dmsp);
        }

        public ActionResult Register()
        {
            ViewBag.Message = null; // Khởi tạo ViewBag.Message nếu cần
            return View();
        }


        // Xử lý đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(DangKy model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Mã hóa mật khẩu trước khi lưu

                    string connectionString = ConfigurationManager.ConnectionStrings["CuaHangTheThaoConnectionString2"].ConnectionString;

                    using (var connection = new SqlConnection(connectionString))
                    {
                        string query = @"INSERT INTO NguoiDung (ten_dang_nhap, mat_khau, email, ten, ho, so_dien_thoai, dia_chi) 
                                         VALUES (@TenDangNhap, @MatKhau, @Email, @Ten, @Ho, @SoDienThoai, @DiaChi)";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@TenDangNhap", model.TenDangNhap);
                            command.Parameters.AddWithValue("@MatKhau", model.MatKhau);
                            command.Parameters.AddWithValue("@Email", model.Email);
                            command.Parameters.AddWithValue("@Ten", (object)model.Ten ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Ho", (object)model.Ho ?? DBNull.Value);
                            command.Parameters.AddWithValue("@SoDienThoai", (object)model.SoDienThoai ?? DBNull.Value);
                            command.Parameters.AddWithValue("@DiaChi", (object)model.DiaChi ?? DBNull.Value);

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }

                    TempData["Message"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay.";
                    return RedirectToAction("DangNhap", "Home");
                }
                catch (Exception ex)
                {
                    // Log exception details if possible
                    ViewBag.Message = "Có lỗi xảy ra: " + ex.Message;
                    return View(model);
                }
            }

            return View(model);
        }






        public ActionResult MenuCap1()
        {
            List<DanhMucSanPham> dsdm = data.DanhMucSanPhams.ToList();
            return PartialView(dsdm);
        }
        public ActionResult MenuCap2(string madm)
        {
            List<SanPham> dsloai = data.SanPhams.Where(l => l.danh_muc_id == madm).ToList();
            return PartialView(dsloai);
        }

        //public ActionResult DanhMucSanPham()
        //{
        //    List<DanhMucSanPham> dsCD = data.DanhMucSanPhams.Take(10).ToList();
        //    return PartialView(dsCD);
        //}
        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Search(string keyword, decimal? minPrice, decimal? maxPrice)
        {
            var products = data.SanPhams.AsQueryable(); // Use AsQueryable to build the query dynamically

            // Check keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                products = products.Where(p => p.ten.Contains(keyword));
            }

            // Check price range
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.gia >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.gia <= maxPrice.Value);
            }

            return View("trangchu", products.ToList());
        }

        public ActionResult DangXuat()
        {
            Session.Clear(); // Clear the session
            return RedirectToAction("DangNhap", "Home"); // Redirect to the login page
        }
        public ActionResult Details(string id)
        {
            // Lấy thông tin sản phẩm theo id
            var sanPham = data.SanPhams.FirstOrDefault(sp => sp.san_pham_id == id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            // Lấy danh mục sản phẩm cho sản phẩm này
            List<DanhMucSanPham> danhMuc = data.DanhMucSanPhams.ToList();

            // Truyền dữ liệu cho view
            ViewBag.DanhMuc = danhMuc;

            return View(sanPham);
        }


    }
}