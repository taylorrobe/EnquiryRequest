using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnquiryRequest3.Models;

namespace EnquiryRequest3.Controllers
{
    public class EnquiriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Enquiries
        public ActionResult Index()
        {
            var enquiries = db.Enquiries.Include(e => e.Invoice);
            return View(enquiries.ToList());
        }

        // GET: Enquiries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enquiry enquiry = db.Enquiries.Find(id);
            if (enquiry == null)
            {
                return HttpNotFound();
            }
            return View(enquiry);
        }

        // GET: Enquiries/Create
        public ActionResult Create()
        {
            ViewBag.InvoiceId = new SelectList(db.Invoices, "InvoiceId", "Code");
            return View();
        }

        // POST: Enquiries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EnquiryId,Code,Name,InvoiceEmail,SearchArea,NoOfYears,JobNumber,Agency,AgencyContact,DataUsedFor,Citations,GisKml,Express,EnquiryDate,Comment,AddedToRersDate,DataCleanedDate,ReporCompleteDate,DocumentsCleanedDate,EnquiryDeliveredDate,AdminComment,InvoiceId")] Enquiry enquiry)
        {
            if (ModelState.IsValid)
            {
                db.Enquiries.Add(enquiry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.InvoiceId = new SelectList(db.Invoices, "InvoiceId", "Code", enquiry.InvoiceId);
            return View(enquiry);
        }

        // GET: Enquiries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enquiry enquiry = db.Enquiries.Find(id);
            if (enquiry == null)
            {
                return HttpNotFound();
            }
            ViewBag.InvoiceId = new SelectList(db.Invoices, "InvoiceId", "Code", enquiry.InvoiceId);
            return View(enquiry);
        }

        // POST: Enquiries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EnquiryId,Code,Name,InvoiceEmail,SearchArea,NoOfYears,JobNumber,Agency,AgencyContact,DataUsedFor,Citations,GisKml,Express,EnquiryDate,Comment,AddedToRersDate,DataCleanedDate,ReporCompleteDate,DocumentsCleanedDate,EnquiryDeliveredDate,AdminComment,InvoiceId")] Enquiry enquiry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enquiry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InvoiceId = new SelectList(db.Invoices, "InvoiceId", "Code", enquiry.InvoiceId);
            return View(enquiry);
        }

        // GET: Enquiries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enquiry enquiry = db.Enquiries.Find(id);
            if (enquiry == null)
            {
                return HttpNotFound();
            }
            return View(enquiry);
        }

        // POST: Enquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Enquiry enquiry = db.Enquiries.Find(id);
            db.Enquiries.Remove(enquiry);
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
