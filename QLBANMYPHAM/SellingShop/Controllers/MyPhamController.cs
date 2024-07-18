using Newtonsoft.Json;
using SellingShop.App_Start;
using SellingShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SellingShop.Controllers
{
    public class MyPhamController : Controller
    {
        QLMyPhamEntities qlbmp;
        List<mypham> CurrentListMyPham = new List<mypham>();
        public MyPhamController()
        {
            this.qlbmp = StaticObject.qlbmp;
            ViewBag.categories = qlbmp.theloais.ToList();
            this.setNewList(qlbmp.myphams.ToList());
        }

        public void setNewList(List<mypham> newRes)
        {
            this.CurrentListMyPham = newRes;
            ViewBag.MaxPage = (int)(this.CurrentListMyPham.Count() / 9) + (this.CurrentListMyPham.Count()%9 == 0 ? 0 : 1);
        }
        // GET: Product
        public ActionResult Index(int page = 1)
        {
            if (page < 1 || page > ViewBag.MaxPage)
            {
                page = 1;
            }
            ViewBag.CurrentPage = page;
            var pagination = this.CurrentListMyPham.OrderByDescending(m => m.ID).Skip((page-1)*9).Take(9).ToList();
            return View(pagination);
        }

        public ActionResult FilterByCategory(int? isLowToHigh, int? category = 1)
        {
            ViewBag.CurrentPage = 1;
            if (category == 0)
            {
                this.setNewList(qlbmp.myphams.ToList());
                return View("Index", this.CurrentListMyPham.Take(9).ToList());
            }
            var result = qlbmp.myphams.Where(m => m.TheLoai == category).ToList();
            this.setNewList(result);
            return View("Index", result.Take(9).ToList());
        }

        public ActionResult ViewMyPham(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var mp = this.qlbmp.myphams.FirstOrDefault(m => m.ID == id);
            if (mp == null)
            {
                return View("Error404");
            }
            return View(mp);
        }

        [FilterLogin]
        [FilterAuthorization]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Error404");
            }
            var mypham = this.qlbmp.myphams.FirstOrDefault(m => m.ID == id);
            if (mypham == null)
            {
                return View("Error404");
            }
            return View(mypham);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [FilterLogin]
        [FilterAuthorization]
        public ActionResult Add(string TenSach, string GiaGoc, string GiaSale, string MoTa, int SoLuong)
        {
            string ImageTitle = "";
            string ImageList = "";
            mypham mp = new mypham();
            this.UploadFile(Request.Files, ref ImageTitle, ref ImageList, this.qlbmp.myphams.Count() + 1, mp);
            StaticObject.setMyPham(mp, TenSach, MoTa, SoLuong, 0, double.Parse(GiaGoc), double.Parse(GiaSale), 5, "", ImageList, ImageTitle);
            this.qlbmp.myphams.Add(mp);
            this.qlbmp.SaveChanges();
            return RedirectToAction("ViewMyPham", new { id = mp.ID });
        }

        public void UploadFile(HttpFileCollectionBase fileRequest, ref string ImageTitle, ref string ImageList, int? ID, mypham Editing)
        {
            if (fileRequest.Count > 0)
            {
                if (Editing.ImageTitle != "")
                    try
                    {
                        string ListOldImages = string.Join(",", new string[] { Editing.ImageLists, Editing.ImageTitle });
                        foreach (string image in ListOldImages.Split(','))
                        {
                            var path = Server.MapPath("~/Content/images/myphams/"+image);
                            System.IO.File.Delete(path);
                        }
                    }
                    catch
                    {

                    }
                for (int i = 0; i < fileRequest.Count; i++)
                {
                    string filename = fileRequest[i].FileName.Replace(' ', '_');
                    if (i == 0)
                    {
                        ImageTitle = ID + "_" + fileRequest.Get(i).FileName;
                    }

                    {
                        HttpPostedFileBase file = fileRequest.Get(i);
                        if (file.ContentLength > 0)
                        {

                            ImageList += ID + "_" + filename + ",";
                        }
                    }
                    fileRequest.Get(i).SaveAs(Server.MapPath("~/Content/images/myphams/" + ID + "_" + filename));
                }
            }
        }

        [HttpPost]
        public ActionResult Search(string keyword, int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.keyword = keyword;
            List<mypham> ListResult = this.qlbmp.myphams.Where(m => m.TenMP.ToLower().Contains(keyword.ToLower())).ToList();
            this.setNewList(ListResult);
            var pagination = this.CurrentListMyPham.OrderByDescending(m => m.ID).Skip((page-1)*9).Take(9).ToList();
            return View("Index", pagination);
        }

        [HttpPost]
        [FilterLogin]
        [FilterAuthorization]
        public ActionResult Edit(int ID, string TenMP, string MoTa, string GiaGoc, string GiaSale, int SoLuong)
        {
            string ImageTitle = "";
            string ImageList = "";
            var Editting = this.qlbmp.myphams.FirstOrDefault(m => m.ID == ID);


            this.UploadFile(Request.Files, ref ImageTitle, ref ImageList, ID, Editting);
            if (Editting != null)
            {
                try
                {
                    StaticObject.setMyPham(Editting, TenMP, MoTa, SoLuong, SoLuong, double.Parse(GiaGoc), double.Parse(GiaSale), Editting.TheLoai, "", ImageList == "" ? null : ImageList.Trim(','), ImageTitle == "" ? null : ImageTitle);
                }
                catch
                {
                    return View("Error403");
                }
            }
            else
            {
                return View("Error404");
            }
            try
            {
                StaticObject.qlbmp.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                string msg = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    msg += ($"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors:");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        msg += ($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                    }
                }
                Console.WriteLine(msg);
            }
            return RedirectToAction("ViewMyPham", new { id = Editting.ID });
        }



        [HttpGet]
        [FilterLogin]
        [FilterAuthorization]
        public ActionResult Remove(int? id)
        {
            if (id == null)
            {
                return View("Error404");
            }
            var bk = this.qlbmp.myphams.FirstOrDefault(m => m.ID == id);
            if (bk == null)
            {
                return View("Error403");
            }
            this.qlbmp.myphams.Remove(bk);
            this.qlbmp.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}