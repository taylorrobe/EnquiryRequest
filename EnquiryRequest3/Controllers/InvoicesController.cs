﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnquiryRequest3.Models;

namespace EnquiryRequest3.Controllers
{
    public class InvoicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invoices
        public ActionResult Index()
        {
            return View(db.Invoices.ToList());
        }

        // GET: Invoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // GET: Invoices/Create
        [Authorize(Roles = "Admin, EnquiryManager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, EnquiryManager")]
        public ActionResult Create([Bind(Include = "InvoiceId,Code,Amount,PONumber,InvoiceDate,PaidDate,PaymentMethodId,PaymentMethod,ChequeNumber,InSlipNumber,RemittanceReference")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Invoices.Add(invoice);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var result in ex.EntityValidationErrors)
                        foreach (var error in result.ValidationErrors)
                            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    return View(invoice);
                }
            }
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        [Authorize(Roles = "Admin, EnquiryManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, EnquiryManager")]
        public ActionResult Edit([Bind(Include = "InvoiceId,Code,Amount,PONumber,InvoiceDate,PaidDate,PaymentMethodId,PaymentMethod,ChequeNumber,InSlipNumber,RemittanceReference, RowVersion")] Invoice invoice)
        {
            if (!ModelState.IsValid) return View(invoice);

            try
            {

                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewBag.Message = "Sorry, couldn't update due to a concurrency issue <br />Please try again";
                return View(invoice);
            }
        }

        // GET: Invoices/Delete/5
        [Authorize(Roles = "Admin, EnquiryManager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, EnquiryManager")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Invoice invoice = db.Invoices.Find(id);
                db.Invoices.Remove(invoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewBag.Message = "Sorry, couldn't delete due to a concurrency issue <br />Please try again";
                return RedirectToAction("Delete");
            }

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
