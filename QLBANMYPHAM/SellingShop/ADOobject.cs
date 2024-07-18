using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SellingShop.Models;

namespace SellingShop
{
    public class ADOobject
    {
        public static QLMyPhamEntities qlbmp = new QLMyPhamEntities();
        public static string username = "";
        public static string cookieName = "myphamvip";
        public static string ToBase64(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }
        public static string FromBase64(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }

        public static account createAccount(string fullname, string username, string email, string password)
        {
            account act = new account();
            act.fullname = fullname;
            act.username = username;
            act.email = email;
            act.password = password;
            act.facebook = "";
            act.job = "";
            act.cart = "[]";
            act.address = "";
            act.birth = DateTime.Now;
            act.instagram = "";
            act.website = "";
            act.phone = "";
            act.image = "tmp_avatar.png";
            act.role = "user";
            return act;
        }

        public static void updateAccount(account act, string fullname, string email, string facebook,
           string job, string address, DateTime? birth, string instagram, string website, string phone)
        {
            act.fullname = fullname;
            act.email = email;
            act.facebook = facebook;
            act.job = job;
            act.address = address;
            if (birth != null)
                act.birth = birth;
            act.instagram = instagram;
            act.website = website;
            act.phone = phone;
        }


        public static void setMyPham(mypham mp, string tenMP, string moTa, int soLuong, int daBan, double giaGoc, double giaSale,  int? theLoai, string type,string imageLists ="", string imageTitle = "")
        {
            mp.TenMP=tenMP;
            mp.MoTa=moTa;
            mp.SoLuong=soLuong;
            mp.DaBan=daBan;
            mp.GiaGoc=giaGoc;
            mp.GiaSale=giaSale;
            if(imageLists!=null)
            {
                mp.ImageLists=imageLists;
                mp.ImageTitle=imageTitle;
            }    
            mp.TheLoai=theLoai;
            mp.type=type;
        }
    }
}