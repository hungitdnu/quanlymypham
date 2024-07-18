using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SellingShop.App_Start;
using SellingShop.Models;

namespace SellingShop.Controllers
{
    public class AccountController : Controller
    {
        private QLMyPhamEntities qlbmp;
        public AccountController()
        {
            this.qlbmp = StaticObject.qlbmp;
        }
        public ActionResult Login()
        {
            if (StaticObject.username!="")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [FilterLogin]
        public ActionResult MyAccount()
        {
            return RedirectToAction("ViewProfile", "Account", new {profile = StaticObject.username});
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var account = StaticObject.qlbmp.accounts.FirstOrDefault(m => m.username.Equals(username));
            if (account == null) 
            { 

                ViewBag.TypeAlert = "danger";
                return View((object)"Tài khoản không tồn tại!");
            }
            var isRight = account.password.Trim().Equals(password);
            if (isRight == false)
            {
                ViewBag.TypeAlert = "danger";
                return View((object)"Mật khẩu không chính xác!");
            }
            HttpCookie cookie = new HttpCookie(StaticObject.cookieName);
            cookie.Value = (StaticObject.ToBase64(username));
            cookie.Expires = DateTime.Now.AddMinutes(30);
            Response.Cookies.Clear();
            Response.Cookies.Add(cookie);
            ViewBag.CurrentUser = username;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string fullname,string username,string email,string password, string repassword)
        {
            ViewBag.TypeAlert = "danger";
            string msg = "";
            if(password != repassword)
            {
                msg = "Mật khẩu không trùng khớp !";
                return View((object)msg);
            }
            if(this.qlbmp.accounts.FirstOrDefault(m => m.username.Equals(username,StringComparison.OrdinalIgnoreCase)) != null)
            {
                msg = "Tài khoản đã tồn tại !";
                return View((object)msg);
            }
            if(password.Length < 8)
            {
                msg = "Mật khẩu quá yếu !";
                return View((object)msg);
            }
            this.qlbmp.accounts.Add(StaticObject.createAccount(fullname,username,email,password));
            this.qlbmp.SaveChanges();
            ViewBag.TypeAlert = "success";
            msg = $"Tạo tài khoản {username} thành công !";
            return View((object)msg);
        }

        public ActionResult Logout()
        {
            // Xóa cookie để đăng xuất người dùng
            if (Request.Cookies[StaticObject.cookieName] != null)
            {
                HttpCookie cookie = new HttpCookie(StaticObject.cookieName);
                cookie.Expires = DateTime.Now.AddDays(-1); // Set thời gian hết hạn trong quá khứ để xóa cookie
                Response.Cookies.Add(cookie);
            }

            // Chuyển hướng đến trang đăng nhập sau khi đăng xuất
            return RedirectToAction("Login", "Account");
        }

        public ActionResult ViewProfile(string profile = null)
        {
            if(profile == null)
            {
                return View("Error404");
            }
            var user = this.qlbmp.accounts.FirstOrDefault(m => m.username.Equals(profile, StringComparison.OrdinalIgnoreCase));
            if(user == null)
            {
                return View("Error404");
            }
            return View(user);
        }

        public void UploadFile(account user, string username, string filename, HttpPostedFileBase file)
        {
            string pathServer = Server.MapPath("~/Content/images/avatars/" + username.Trim() + "_" + filename.Trim().Replace(" ", String.Empty));
            string pathUser = username.Trim() + "_" + filename.Trim().Replace(" ", String.Empty);
            try
            {
                file.SaveAs(pathServer);
                user.image = pathUser;
            }
            catch
            {

            }
        }

        [HttpGet]
        [FilterLogin]
        [FilterAuthorization]
        public ActionResult EditProfile(string profile)
        {

            if (profile == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            var user = this.qlbmp.accounts.FirstOrDefault(m => m.username.Equals(profile, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            return View(user);
        }

        [HttpPost]
        [FilterLogin]
        [FilterAuthorization]
        public ActionResult SubmitProfile(string profile, string website, string facebook, string fullname, string email, string phone, string address, DateTime? birth, string job, string instagram)
        {
            var user = this.qlbmp.accounts.FirstOrDefault(m => m.username.Equals(profile, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                return View("Error400");
            }
            HttpPostedFileBase file = Request.Files.Get(0);
            StaticObject.updateAccount(user, fullname, email, facebook, job, address, birth, instagram, website, phone);
            if (file != null)
                this.UploadFile(user, profile, file.FileName, file);
            this.qlbmp.SaveChanges();
            return RedirectToAction("ViewProfile", new { profile = profile });
        }


        [HttpGet]
        [FilterLogin]
        [FilterAuthorization]
        public ActionResult Cart()
        {
            var user = this.qlbmp.accounts.FirstOrDefault(m => m.username.Equals(StaticObject.username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                return View("Error404");
            }
            List<string> UserCart = new List<string>();
            foreach (var tmp in JsonConvert.DeserializeObject<object[]>(user.cart))
            {
                UserCart.Add(tmp.ToString());
            }
            return View(UserCart);
        }

        [HttpGet]
        [FilterLogin]
        [FilterAuthorization]
        public ActionResult AddProduct(int? id, int sl = 1)
        {
            var user = this.qlbmp.accounts.FirstOrDefault(m => m.username.Equals(StaticObject.username, StringComparison.OrdinalIgnoreCase));
            if (id == null)
            {
                return View("Error404");
            }
            bool isExist = false;
            List<string> UserCart = new List<string>();
            foreach (string tmp in JsonConvert.DeserializeObject<object[]>(user.cart))
            {
                int idCurrentProduct = int.Parse(tmp.Split(':')[0]);
                if(idCurrentProduct == id)
                {
                    isExist = true;
                    UserCart.Add($"{idCurrentProduct}:{int.Parse(tmp.Split(':')[1]) + sl}");
                }
                else
                    UserCart.Add(tmp.ToString());
            }
            if(!isExist)
                UserCart.Add($"{id}:{sl}");
            user.cart = JsonConvert.SerializeObject(UserCart.ToArray());
            this.qlbmp.SaveChanges();
            return RedirectToAction("Cart",UserCart);
        }

        [HttpGet]
        [FilterLogin]
        [FilterAuthorization]
        public ActionResult RemoveCart(int? id)
        {
            var user = this.qlbmp.accounts.FirstOrDefault(m => m.username.Equals(StaticObject.username, StringComparison.OrdinalIgnoreCase));
            if (id == null)
            {
                return View("Error404");
            }
            List<string> UserCart = new List<string>();
            foreach (var tmp in JsonConvert.DeserializeObject<object[]>(user.cart))
            {
                if (int.Parse(tmp.ToString().Split(':')[0]) != id)
                    UserCart.Add(tmp.ToString());
            }
            user.cart = JsonConvert.SerializeObject(UserCart.ToArray());
            this.qlbmp.SaveChanges();
            return View("Cart",UserCart);
        }

        [FilterLogin]
        [FilterAuthorization]
        public ActionResult Checkout()
        {
            var user = this.qlbmp.accounts.FirstOrDefault(m => m.username.Equals(StaticObject.username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                return View("Error400");
            }
            List<string> UserCart = new List<string>();
            foreach (var tmp in JsonConvert.DeserializeObject<object[]>(user.cart))
            {
                UserCart.Add(tmp.ToString());
            }
            return View(UserCart);
        }

        [HttpPost]
        [FilterLogin]
        public ActionResult SubmitOrder(string profile, [Bind(Prefix = "")] hoadon model)
        {
            var user = this.qlbmp.accounts.FirstOrDefault(m => m.username.Equals(profile, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                return View("Error400");
            }
            model.Custom = StaticObject.username;
            model.DateCreate = DateTime.Now;
            model.Status = "Đang chờ";
            this.qlbmp.hoadons.Add(model);
            user.cart = "[]";
            this.qlbmp.SaveChanges();
            return View("Complete");
        }

        [FilterLogin]
        public ActionResult Order()
        {
            List<hoadon> res = this.qlbmp.hoadons.Where(m => m.Custom.Equals(StaticObject.username)).OrderByDescending(m => m.ID).ToList();
            return View(res);
        }

        [FilterLogin]
        public ActionResult ViewOrder(int? id)
        {
            if (id == null)
            {
                return View("Error404");
            }
            var hd = this.qlbmp.hoadons.FirstOrDefault(m => m.ID == id);
            if(hd == null)
            {
                return View("Error404");
            }
            foreach (var tmp in JsonConvert.DeserializeObject<object[]>(hd.OrderCart))
            {
                hd.Orders.Add(tmp.ToString());
            }
            return View(hd);
        }

        [FilterLogin]
        public ActionResult CancelOrder(int? id)
        {
            if (id == null)
            {
                return View("Error404");
            }
            var hd = this.qlbmp.hoadons.FirstOrDefault(m => m.ID == id);
            if (hd == null)
            {
                return View("Error404");
            }
            hd.Status = "Đã huỷ";
            this.qlbmp.SaveChanges();
            return RedirectToAction("Order");
        }
    }
}