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
        private ShraddhaEntities1 dbContext;
        public HomeController()
        {
            dbContext = new ShraddhaEntities1();
        }

        public ActionResult Index()
        {
            return RedirectToAction("AllUsers");
        }

        public ActionResult AllUsers()
        {
            var users = dbContext.Users.ToList();
            dbContext.SaveChanges();
            return View(users);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(User u)
        {
            if (ModelState.IsValid)
            {
                var fileName = Path.GetFileNameWithoutExtension(u.imageFile.FileName);
                string extension = Path.GetExtension(u.imageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                u.profile_photo = "~/Content/img/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Content/img"), fileName);
                u.imageFile.SaveAs(fileName);
                dbContext.Users.Add(u);
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
                var rowData = dbContext.Users.Where(x => x.userId == id).FirstOrDefault();
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
        public ActionResult Edit(User u, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var userId = (int)TempData["UserId"];
                var userObj = dbContext.Users.FirstOrDefault(x => x.userId == userId);

                if (userObj != null)
                {
                    userObj.name = u.name;
                    userObj.dob = u.dob;
                    userObj.gender = u.gender;

                    // Check if a new file is uploaded
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                        string extension = Path.GetExtension(imageFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        u.profile_photo = "~/Content/img/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Content/img"), fileName);
                        imageFile.SaveAs(fileName);
                    }

                    dbContext.Entry(userObj).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
                return RedirectToAction("AllUsers");
            }
            else
            {
                // Handle invalid model state
                return View(u);
            }
        }

        public ActionResult Delete(int id)
        {
            var user = dbContext.Users.Find(id);
            if(user != null)
            {
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();
            }
            return RedirectToAction("AllUsers");
        }
    }
}