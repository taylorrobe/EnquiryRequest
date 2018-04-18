using EnquiryRequest3.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace EnquiryRequest3.Controllers
{
    [Authorize]
    public class EnquiriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Enquiries
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId<int>();
            var enquiries = db.Enquiries.Include(e => e.Invoice)
                .Where(a => a.ApplicationUserId == userId);
                //.Where(a => a.ApplicationUser == User);
            return View(enquiries.ToList());
        }

        // GET: Enquiries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var currentUser = db.Users.Single(user => user.UserName == User.Identity.Name);
            Enquiry enquiry = db.Enquiries.Find(id);
            if (currentUser.Id == enquiry.ApplicationUserId)
            {
                //let user view the details
                
                if (enquiry == null)
                {
                    return HttpNotFound();
                }
                return View(enquiry);
            }
            else
            {
                // send him back to the post or do something else.
                return RedirectToAction("Index", "Enquiries");
            }

        }

        // GET: Enquiries/Create
        public ActionResult Create()
        {
            if (ViewBag.ValidationRepost == null)
            {
                ViewBag.ValidationRepost = "false";
            }

            ViewBag.DefaultInvoiceEmail = User.Identity.GetUserDefaultInvoicingEmail();
            ViewBag.SearchTypeId = new SelectList(db.SearchTypes, "SearchTypeId", "Name");
            return View();
        }

        // POST: Enquiries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EnquiryId,Name,InvoiceEmail,SearchAreaWkt,SearchTypeId,NoOfYears,JobNumber,Agency,AgencyContact,DataUsedFor,Citations,GisKml,Express,EnquiryDate,Comment")] UserCreateEditEnquiryViewModel model)
        {
            if (ModelState.IsValid)
            {
                DbGeometry geom = DbGeometry.FromText(model.SearchAreaWkt, 3857); //(EPSG:27700) is OSGB, (EPSG:3857) google maps geometric, wgs84 (EPSG:4326) google geographic
                //transform geom
                var defaultInvoiceEmail = User.Identity.GetUserDefaultInvoicingEmail();
                var user = User.Identity.GetAppUser();
                var userId = User.Identity.GetIntUserId();
                if (model.InvoiceEmail != null)
                {
                    defaultInvoiceEmail = model.InvoiceEmail;
                }
                Enquiry enquiry = new Enquiry
                {
                    ApplicationUserId = userId,
                    Name = model.Name,
                    InvoiceEmail = defaultInvoiceEmail,
                    SearchArea = geom,
                    SearchTypeId = model.SearchTypeId,
                    NoOfYears = model.NoOfYears,
                    JobNumber = model.JobNumber,
                    Agency = model.Agency,
                    AgencyContact = model.AgencyContact,
                    DataUsedFor = model.DataUsedFor,
                    Citations = model.Citations,
                    GisKml = model.GisKml,
                    Express = model.Express,
                    Comment = model.Comment,
                };
                
                db.Enquiries.Add(enquiry);

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
                    ViewBag.ValidationRepost = "true";
                    return View(model);
                }
            }

            ViewBag.DefaultInvoiceEmail = User.Identity.GetUserDefaultInvoicingEmail();
            ViewBag.SearchTypeId = new SelectList(db.SearchTypes, "SearchTypeId", "Name", model.SearchTypeId);
            ViewBag.ValidationRepost = "true";
            return View(model);
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
