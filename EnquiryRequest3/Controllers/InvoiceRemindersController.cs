using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EnquiryRequest3.Models;

namespace EnquiryRequest3.Controllers
{
    public class InvoiceRemindersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: InvoiceReminders
        public ActionResult Index()
        {
            var invoiceReminders = db.InvoiceReminders.Include(i => i.InvoiceReminderType);
            return View(invoiceReminders.ToList());
        }

        // GET: InvoiceReminders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceReminder invoiceReminder = db.InvoiceReminders.Find(id);
            if (invoiceReminder == null)
            {
                return HttpNotFound();
            }
            return View(invoiceReminder);
        }

        // GET: InvoiceReminders/Create
        public ActionResult Create()
        {
            ViewBag.InvoiceReminderTypeId = new SelectList(db.InvoiceReminderTypes, "InvoiceReminderTypeId", "ReminderSubject");
            return View();
        }

        // POST: InvoiceReminders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InvoiceReminderId,InvoiceReminderDate,InvoiceReminderTypeId")] InvoiceReminder invoiceReminder)
        {
            if (ModelState.IsValid)
            {
                db.InvoiceReminders.Add(invoiceReminder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.InvoiceReminderTypeId = new SelectList(db.InvoiceReminderTypes, "InvoiceReminderTypeId", "ReminderSubject", invoiceReminder.InvoiceReminderTypeId);
            return View(invoiceReminder);
        }

        // GET: InvoiceReminders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceReminder invoiceReminder = db.InvoiceReminders.Find(id);
            if (invoiceReminder == null)
            {
                return HttpNotFound();
            }
            ViewBag.InvoiceReminderTypeId = new SelectList(db.InvoiceReminderTypes, "InvoiceReminderTypeId", "ReminderSubject", invoiceReminder.InvoiceReminderTypeId);
            return View(invoiceReminder);
        }

        // POST: InvoiceReminders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InvoiceReminderId,InvoiceReminderDate,InvoiceReminderTypeId")] InvoiceReminder invoiceReminder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoiceReminder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InvoiceReminderTypeId = new SelectList(db.InvoiceReminderTypes, "InvoiceReminderTypeId", "ReminderSubject", invoiceReminder.InvoiceReminderTypeId);
            return View(invoiceReminder);
        }

        // GET: InvoiceReminders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceReminder invoiceReminder = db.InvoiceReminders.Find(id);
            if (invoiceReminder == null)
            {
                return HttpNotFound();
            }
            return View(invoiceReminder);
        }

        // POST: InvoiceReminders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvoiceReminder invoiceReminder = db.InvoiceReminders.Find(id);
            db.InvoiceReminders.Remove(invoiceReminder);
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
