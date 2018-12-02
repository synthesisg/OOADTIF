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
    public class usController : Controller
    {
        private usContext db = new usContext();

        // GET: us
        public ActionResult Index()
        {
            return View(db.db.ToList());
        }

        // GET: us/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            us us = db.db.Find(id);
            if (us == null)
            {
                return HttpNotFound();
            }
            return View(us);
        }

        // GET: us/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: us/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,user,pwd")] us us)
        {
            if (ModelState.IsValid)
            {
                db.db.Add(us);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(us);
        }

        // GET: us/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            us us = db.db.Find(id);
            if (us == null)
            {
                return HttpNotFound();
            }
            return View(us);
        }

        // POST: us/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,user,pwd")] us us)
        {
            if (ModelState.IsValid)
            {
                db.Entry(us).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(us);
        }

        // GET: us/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            us us = db.db.Find(id);
            if (us == null)
            {
                return HttpNotFound();
            }
            return View(us);
        }

        // POST: us/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            us us = db.db.Find(id);
            db.db.Remove(us);
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
