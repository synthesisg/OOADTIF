using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ooadtest4_5.Models;

namespace ooadtest4_5.Controllers
{
    public class class1Controller : Controller
    {
        private class1DB db = new class1DB();

        // GET: class1
        public ActionResult Index()
        {
            return View(db.data.ToList());
        }

        // GET: class1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            class1 class1 = db.data.Find(id);
            if (class1 == null)
            {
                return HttpNotFound();
            }
            return View(class1);
        }

        // GET: class1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: class1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,course_id,name,site,time")] class1 class1)
        {
            if (ModelState.IsValid)
            {
                db.data.Add(class1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(class1);
        }

        // GET: class1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            class1 class1 = db.data.Find(id);
            if (class1 == null)
            {
                return HttpNotFound();
            }
            return View(class1);
        }

        // POST: class1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,course_id,name,site,time")] class1 class1)
        {
            if (ModelState.IsValid)
            {
                db.Entry(class1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(class1);
        }

        // GET: class1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            class1 class1 = db.data.Find(id);
            if (class1 == null)
            {
                return HttpNotFound();
            }
            return View(class1);
        }

        // POST: class1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            class1 class1 = db.data.Find(id);
            db.data.Remove(class1);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
