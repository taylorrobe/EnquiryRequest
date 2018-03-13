using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnquiryRequest3.Models;

namespace EnquiryRequest3.Controllers
{
    public class InvoiceReminderTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: InvoiceReminderTypes
        public ActionResult Index()
        {
            return View(db.InvoiceReminderTypes.ToList());
        }

        // GET: InvoiceReminderTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceReminderType invoiceReminderType = db.InvoiceReminderTypes.Find(id);
            if (invoiceReminderType == null)
            {
                return HttpNotFound();
            }
            return View(invoiceReminderType);
        }

        // GET: InvoiceReminderTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InvoiceReminderTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InvoiceReminderTypeId,ReminderSubject,ReminderBody")] InvoiceReminderType invoiceReminderType)
        {
            if (ModelState.IsValid)
            {
                db.InvoiceReminderTypes.Add(invoiceReminderType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(invoiceReminderType);
        }

        // GET: InvoiceReminderTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceReminderType invoiceReminderType = db.InvoiceReminderTypes.Find(id);
            if (invoiceReminderType == null)
            {
                return HttpNotFound();
            }
            return View(invoiceReminderType);
        }

        // POST: InvoiceReminderTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InvoiceReminderTypeId,ReminderSubject,ReminderBody")] InvoiceReminderType invoiceReminderType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoiceReminderType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(invoiceReminderType);
        }

        // GET: InvoiceReminderTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceReminderType invoiceReminderType = db.InvoiceReminderTypes.Find(id);
            if (invoiceReminderType == null)
            {
                return HttpNotFound();
            }
            return View(invoiceReminderType);
        }

        // POST: InvoiceReminderTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvoiceReminderType invoiceReminderType = db.InvoiceReminderTypes.Find(id);
            db.InvoiceReminderTypes.Remove(invoiceReminderType);
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
