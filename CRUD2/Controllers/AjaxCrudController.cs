using CRUD2.ActionFilters;
using CRUD2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CRUD2.Controllers
{
    [LogActionFilter]
    public class AjaxCrudController : Controller
    {
        private ShraddhaEntities7 _dbContext;

        public AjaxCrudController()
        {
            _dbContext = new ShraddhaEntities7();
        }
        public ActionResult Index()
        {
            var users = _dbContext.users.ToList();
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
                u.profile_photo = SaveUploadedFile(imageFile, "~/Content/img/");
                if(u.profile_photo == null)
                {
                    u.profile_photo = "~/Content/img/dummy_profile_img.png";
                }

                u.resume = SaveUploadedFile(resumeFile, "~/Content/TextFiles/");

                _dbContext.users.Add(u);
                _dbContext.SaveChanges();

                return Json(new { success = true, redirectToUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed", errors = errors }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Edit(int id)
        {
            if (ModelState.IsValid)
            {
                var rowData = _dbContext.users.Where(x => x.userId == id).FirstOrDefault();
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
                var userObj = _dbContext.users.FirstOrDefault(x => x.userId == userId);

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

                    _dbContext.Entry(userObj).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(u);
            }
        }

        public ActionResult Delete(int id)
        {
            var user = _dbContext.users.Find(id);

            if (user != null)
            {
                DeleteImageFile(id);
                DeleteTextFile(id);
                _dbContext.users.Remove(user);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public void DeleteImageFile(int id)
        {
            var userObj = _dbContext.users.FirstOrDefault(x => x.userId == id);

            var oldImageFilePath = userObj.profile_photo;
            oldImageFilePath = Server.MapPath(oldImageFilePath);
            System.IO.File.Delete(oldImageFilePath);
        }

        public void DeleteTextFile(int id)
        {
            var userObj = _dbContext.users.FirstOrDefault(x => x.userId == id);

            var oldTextFilePath = userObj.resume;
            oldTextFilePath = Server.MapPath(oldTextFilePath);
            System.IO.File.Delete(oldTextFilePath);
        }

        private string SaveUploadedFile(HttpPostedFileBase file, string folderPath)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                var filePath = Path.Combine(Server.MapPath(folderPath), fileName);
                file.SaveAs(filePath);
                return folderPath + fileName;
            }
            return null;
        }
    }
}