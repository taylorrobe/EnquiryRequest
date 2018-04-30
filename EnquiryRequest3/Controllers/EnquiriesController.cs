using EnquiryRequest3.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EnquiryRequest3.Controllers.Utilities;

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
                //get all boundaries for displaying on map
                SpatialHelper spatial = new SpatialHelper();
                ViewBag.Boundaries = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.DISPLAY);
                ViewBag.Coverage = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.COVERAGE);
                return View(enquiry);
            }
            else
            {
                // send user back to the index
                return RedirectToAction("Index", "Enquiries");
            }

        }

        // GET: Enquiries/Create
        public ActionResult Create()
        {
            ViewBag.DefaultInvoiceEmail = User.Identity.GetUserDefaultInvoicingEmail();
            ViewBag.SearchTypeId = new SelectList(db.SearchTypes, "SearchTypeId", "Name");
            //get all boundaries for displaying on map
            SpatialHelper spatial = new SpatialHelper();
            ViewBag.Boundaries = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.DISPLAY);
            ViewBag.Coverage = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.COVERAGE);
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
                    return View(model);
                }
            }

            ViewBag.DefaultInvoiceEmail = User.Identity.GetUserDefaultInvoicingEmail();
            ViewBag.SearchTypeId = new SelectList(db.SearchTypes, "SearchTypeId", "Name", model.SearchTypeId);
            //get all boundaries for displaying on map
            SpatialHelper spatial = new SpatialHelper();
            ViewBag.Boundaries = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.DISPLAY);
            ViewBag.Coverage = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.COVERAGE);
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
            var wkt = enquiry.SearchArea.WellKnownValue.WellKnownText;
            UserCreateEditEnquiryViewModel userCreateEditEnquiryViewModel = new UserCreateEditEnquiryViewModel
            {
                EnquiryId = enquiry.EnquiryId,
                Name = enquiry.Name,
                InvoiceEmail = enquiry.InvoiceEmail,
                SearchAreaWkt = wkt,
                SearchTypeId = enquiry.SearchTypeId,
                NoOfYears = enquiry.NoOfYears,
                JobNumber = enquiry.JobNumber,
                Agency = enquiry.Agency,
                AgencyContact = enquiry.AgencyContact,
                DataUsedFor = enquiry.DataUsedFor,
                Citations = enquiry.Citations,
                GisKml = enquiry.GisKml,
                Express = enquiry.Express,
                Comment = enquiry.Comment
            };
            ViewBag.SearchTypeId = new SelectList(db.SearchTypes, "SearchTypeId", "Name", userCreateEditEnquiryViewModel.SearchTypeId);
            //get all boundaries for displaying on map
            SpatialHelper spatial = new SpatialHelper();
            ViewBag.Boundaries = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.DISPLAY);
            ViewBag.Coverage = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.COVERAGE);
            return View(userCreateEditEnquiryViewModel);
        }

        // POST: Enquiries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EnquiryId,Name,InvoiceEmail,SearchAreaWkt,SearchTypeId,NoOfYears,JobNumber,Agency,AgencyContact,DataUsedFor,Citations,GisKml,Express,EnquiryDate,Comment")] UserCreateEditEnquiryViewModel model)
        {
            DbGeometry geom = DbGeometry.FromText(model.SearchAreaWkt, 3857);
            var user = User.Identity.GetAppUser();
            var userId = User.Identity.GetIntUserId();
            if (ModelState.IsValid)
            {
                Enquiry enquiry = new Enquiry
                {
                    EnquiryId = model.EnquiryId,
                    ApplicationUserId = userId,
                    Name = model.Name,
                    InvoiceEmail = model.InvoiceEmail,
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
                    Comment = model.Comment
                };
                db.Entry(enquiry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SearchTypeId = new SelectList(db.SearchTypes, "SearchTypeId", "Name", model.SearchTypeId);
            //get all boundaries for displaying on map
            SpatialHelper spatial = new SpatialHelper();
            ViewBag.Boundaries = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.DISPLAY);
            ViewBag.Coverage = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.COVERAGE);
            return View(model);
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
