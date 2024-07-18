using SellingShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SellingShop.Models;
using System.EnterpriseServices;

namespace SellingShop.Controllers
{
    public class HomeController : Controller
    {
        public  static mypham myphamClone(mypham mp)
        {
            mypham mpClone = new mypham();
            mpClone.ID=mp.ID;
            mpClone.TenMP=mp.TenMP;
            mpClone.MoTa=mp.MoTa;
            mpClone.SoLuong=mp.SoLuong;
            mpClone.DaBan=mp.DaBan;
            mpClone.GiaGoc=mp.GiaGoc;
            mpClone.GiaSale=mp.GiaSale;
            mpClone.ImageLists=mp.ImageLists;
            mpClone.ImageTitle=mp.ImageTitle;
            mpClone.type= mp.type;
            return mpClone;
        }
        QLMyPhamEntities qlbmp;
        public HomeController()
        {
            this.qlbmp = StaticObject.qlbmp;
        }
        public ActionResult Index()
        {
            var listMyPham = this.qlbmp.myphams.OrderByDescending(x => x.ID).ToList();
            return View(listMyPham.Take(6).ToList());
        }

        public ActionResult ContactUs()
        {
            return View();
        }

    }
}