using CRUD2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace CRUD2.Controllers
{
    public class HomeController : Controller
    {
        private ShraddhaEntities6 dbContext;
        public HomeController()
        {
            dbContext = new ShraddhaEntities6();
        }

        public ActionResult Index()
        {
            return RedirectToAction("AllUsers");
        }

        public ActionResult AllUsers()
        {
            var users = dbContext.users.ToList();
            dbContext.SaveChanges();
            return View(users);
        }

        public ActionResult AllUserProfile()
        {
            var users = dbContext.users.ToList();
            return View(users);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(user u, HttpPostedFileBase imageFile, HttpPostedFileBase resumeFile)
        {
            if (ModelState.IsValid)
            {
                if(imageFile != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    string extension = Path.GetExtension(imageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    u.profile_photo = "~/Content/img/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Content/img"), fileName);
                    imageFile.SaveAs(fileName);
                }
                else
                {
                    u.profile_photo = "~/Content/img/dummy_profile_img.png";
                }

                if (resumeFile != null && resumeFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(resumeFile.FileName);
                    string extension = Path.GetExtension(resumeFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    u.resume = "~/Content/TextFiles/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Content/TextFiles"), fileName);
                    resumeFile.SaveAs(fileName);
                }
                
                dbContext.users.Add(u);
                dbContext.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("AllUsers");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            if (ModelState.IsValid)
            {
                var rowData = dbContext.users.Where(x => x.userId == id).FirstOrDefault();
                if (rowData != null)
                {
                    TempData["UserId"] = id;
                    TempData.Keep();
                    return View(rowData);
                }
                return HttpNotFound();
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(user u, HttpPostedFileBase imageFile, HttpPostedFileBase resumeFile)
        {
            if (ModelState.IsValid)
            {
                var userId = (int)TempData["UserId"];
                var userObj = dbContext.users.FirstOrDefault(x => x.userId == userId);

                if (userObj != null)
                {
                    userObj.name = u.name;
                    userObj.dob = u.dob;
                    userObj.gender = u.gender;
                    userObj.email = u.email;
                    userObj.address = u.address;
                    userObj.city = u.city;
                    
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        DeleteImageFile(userId);

                         var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                        string extension = Path.GetExtension(imageFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        userObj.profile_photo = "~/Content/img/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Content/img"), fileName);
                        imageFile.SaveAs(fileName);
                    }

                    if (resumeFile != null && resumeFile.ContentLength > 0)
                    {
                        DeleteTextFile(userId);

                        var fileName = Path.GetFileNameWithoutExtension(resumeFile.FileName);
                        string extension = Path.GetExtension(resumeFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        userObj.resume = "~/Content/TextFiles/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Content/TextFiles"), fileName);
                        resumeFile.SaveAs(fileName);
                    }

                    dbContext.Entry(userObj).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
                return RedirectToAction("AllUsers");
            }
            else
            {
                return View(u);
            }
        }

        public ActionResult Delete(int id)
        {
            var user = dbContext.users.Find(id);

            if (user != null)
            { 
                DeleteImageFile(id);
                DeleteTextFile(id);
                dbContext.users.Remove(user);
                dbContext.SaveChanges();
            }
            return RedirectToAction("AllUsers");
        }

        //delete old image file from Conetent/img
        public void DeleteImageFile(int id)
        {
            var userObj = dbContext.users.FirstOrDefault(x => x.userId == id);

            var oldImageFilePath = userObj.profile_photo;
            oldImageFilePath = Server.MapPath(oldImageFilePath);
            System.IO.File.Delete(oldImageFilePath);
        }

        //delete old text file from Conetent/TextFiles
        public void DeleteTextFile(int id)
        {
            var userObj = dbContext.users.FirstOrDefault(x => x.userId == id);

            var oldTextFilePath = userObj.resume;
            oldTextFilePath = Server.MapPath(oldTextFilePath);
            System.IO.File.Delete(oldTextFilePath);
        }
    }
}