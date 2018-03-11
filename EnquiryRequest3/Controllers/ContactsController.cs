using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using EnquiryRequest3.Models;
using Microsoft.AspNet.Identity;

namespace EnquiryRequest3.Controllers
{ 
    [Authorize]
    public class ContactsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contacts
        public ActionResult Index()
        {
            var contacts = db.Contacts.Include(c => c.Organisation);
            return View(contacts.ToList());
        }

        // GET: Contacts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // GET: Contacts/Create
        public ActionResult Create()
        {
            ViewBag.OrganisationId = new SelectList(db.Organisations, "OrganisationId", "Name");
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ContactId,Forename,Surname,Address1,Address2,Address3,PostCode,Email,PhoneNumber,OrganisationId,DefaultInvoicingEmail")] ContactForUserViewModel contactForUserViewModel)
        {
            if (ModelState.IsValid)
            {
                ViewBag.OrganisationId = new SelectList(db.Organisations, "OrganisationId", "Name", contactForUserViewModel.OrganisationId);
                //get the user id to use in the one-one relation between user and contact
                int userId = User.Identity.GetUserId<int>();
                //use extension class method to get the email address from user
                string userEmail = IdentityExtensions.GetUserEmailAdress(User.Identity);
                string defaultEmail = null;
                if(contactForUserViewModel.DefaultInvoicingEmail == null || contactForUserViewModel.DefaultInvoicingEmail == String.Empty)
                {
                    defaultEmail = userEmail;
                }
                var contact = new Contact()
                {
                    ContactId = userId,
                    Forename = contactForUserViewModel.Forename,
                    Surname = contactForUserViewModel.Surname,
                    Address1 = contactForUserViewModel.Address1,
                    Address2 = contactForUserViewModel.Address2,
                    Address3 = contactForUserViewModel.Address3,
                    PostCode = contactForUserViewModel.PostCode,
                    Email = userEmail,
                    PhoneNumber = contactForUserViewModel.PhoneNumber,
                    OrganisationId = contactForUserViewModel.OrganisationId,
                    DefaultInvoicingEmail = defaultEmail
                };
                db.Contacts.Add(contact);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch(DbEntityValidationException ex)
                {
                    foreach (var result in ex.EntityValidationErrors)
                        foreach (var error in result.ValidationErrors)
                            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    return View(contactForUserViewModel);
                }
            }
            return View(contactForUserViewModel);
        }

        // GET: Contacts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrganisationId = new SelectList(db.Organisations, "OrganisationId", "Name", contact.OrganisationId);
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ContactId,Forename,Surname,Address1,Address2,Address3,PostCode,Email,PhoneNumber,OrganisationId,DefaultInvoicingEmail")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrganisationId = new SelectList(db.Organisations, "OrganisationId", "Name", contact.OrganisationId);
            return View(contact);
        }

        // GET: Contacts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contact contact = db.Contacts.Find(id);
            db.Contacts.Remove(contact);
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
