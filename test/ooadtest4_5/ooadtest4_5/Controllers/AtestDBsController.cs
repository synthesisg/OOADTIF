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
    public class AtestDBsController : Controller
    {
        private AtestDBContext db = new AtestDBContext();

        // GET: AtestDBs
        public ActionResult Index()
        {
            return View(db.Atest.ToList());
        }

        // GET: AtestDBs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtestDB atestDB = db.Atest.Find(id);
            if (atestDB == null)
            {
                return HttpNotFound();
            }
            return View(atestDB);
        }

        // GET: AtestDBs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AtestDBs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Director,Date")] AtestDB atestDB)
        {
            if (ModelState.IsValid)
            {
                db.Atest.Add(atestDB);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(atestDB);
        }

        // GET: AtestDBs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtestDB atestDB = db.Atest.Find(id);
            if (atestDB == null)
            {
                return HttpNotFound();
            }
            return View(atestDB);
        }

        // POST: AtestDBs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Director,Date")] AtestDB atestDB)
        {
            if (ModelState.IsValid)
            {
                db.Entry(atestDB).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(atestDB);
        }

        // GET: AtestDBs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtestDB atestDB = db.Atest.Find(id);
            if (atestDB == null)
            {
                return HttpNotFound();
            }
            return View(atestDB);
        }

        // POST: AtestDBs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AtestDB atestDB = db.Atest.Find(id);
            db.Atest.Remove(atestDB);
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
