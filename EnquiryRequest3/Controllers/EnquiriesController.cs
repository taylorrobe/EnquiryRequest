using EnquiryRequest3.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EnquiryRequest3.Controllers.Utilities;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Diagnostics;

namespace EnquiryRequest3.Controllers
{
    [Authorize]
    public class EnquiriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        protected UserManager<ApplicationUser, int> userManager;

        public EnquiriesController()
        {
            userManager = new UserManager<ApplicationUser
                , int>(new UserStore<ApplicationUser
                , CustomRole
                , int
                , CustomUserLogin
                , CustomUserRole
                , CustomUserClaim>(this.db));
        }

        // GET: Enquiries
        public ActionResult Index(int? id, string searchString, string stageFilter)
        {
            var viewModel = new EnquiryIndexData();
            var enquiries = db.Enquiries.Include(e => e.Invoice)
                    .Include(q => q.Quotes);
            var SelectedEnquiryQuotes = db.Quotes.Include(q => q.Enquiry).Where(e => e.EnquiryId == id.Value);

            //filter by searchString
            if (!string.IsNullOrEmpty(searchString))
            {
                enquiries = enquiries.Where(a => a.Code == searchString || a.Name.Contains(searchString));
            }
            //filter by stageFilter
            if (!string.IsNullOrEmpty(stageFilter))
            {
                switch (stageFilter)
                {
                    case "All active":
                        enquiries = enquiries.Where(a => a.InvoiceId == null 
                            && a.AddedToRersDate == null
                            && a.DataCleanedDate == null
                            && a.DocumentsCleanedDate == null
                            && a.EnquiryDeliveredDate == null
                            && a.ReportCompleteDate == null
                            );
                        break;
                    case "Unquoted":
                        enquiries = enquiries.Where(a => a.InvoiceId == null
                            && a.AddedToRersDate == null
                            && a.DataCleanedDate == null
                            && a.DocumentsCleanedDate == null
                            && a.EnquiryDeliveredDate == null
                            && a.ReportCompleteDate == null
                            && a.Quotes.Count() < 1
                            );
                        break;
                    case "Quoted":
                        enquiries = enquiries.Where(a => a.InvoiceId == null
                            && a.AddedToRersDate == null
                            && a.DataCleanedDate == null
                            && a.DocumentsCleanedDate == null
                            && a.EnquiryDeliveredDate == null
                            && a.ReportCompleteDate == null
                            && a.Quotes.Count() > 0
                            && a.Quotes.Where(q=>q.AcceptedDate != null).Count() < 1
                            );
                        break;
                    case "Quote accepted":
                        enquiries = enquiries.Where(a => a.InvoiceId == null
                            && a.AddedToRersDate == null
                            && a.DataCleanedDate == null
                            && a.DocumentsCleanedDate == null
                            && a.EnquiryDeliveredDate == null
                            && a.ReportCompleteDate == null
                            && a.Quotes.Count() > 0
                            && a.Quotes.Where(q => q.AcceptedDate != null).Count() > 0
                            );
                        break;
                    case "All complete":
                        enquiries = enquiries.Where(a=>a.ReportCompleteDate != null
                            );
                        break;
                    case "Report complete":
                        enquiries = enquiries.Where(a => a.ReportCompleteDate != null
                            && a.InvoiceId == null
                            );
                        break;
                    case "Invoiced":
                        enquiries = enquiries.Where(a => a.ReportCompleteDate != null
                            && a.InvoiceId != null
                            );
                        break;
                    case "Paid":
                        enquiries = enquiries.Where(a => a.ReportCompleteDate != null
                            && a.InvoiceId != null
                            && a.Invoice.PaidDate != null
                            );
                        break;
                    default:
                        break;
                }
            }
            //filter enquiries by user if not admin or manager
            var userId = User.Identity.GetUserId<int>();
            var userName = User.Identity.Name;

            if (!userManager.IsInRole(userId, "Admin") && !userManager.IsInRole(userId, "EnquiryManager"))
            {
                enquiries = enquiries
                    .Where(a => a.ApplicationUserId == userId);
            }

            //order the results by desc enquiry date
            enquiries = enquiries.OrderByDescending(d => d.EnquiryDate);

            //attach to view model
            viewModel.Enquiries = enquiries;

            //attach quotes for selected enquiry
            if (id != null)
            {
                if (viewModel.Enquiries.Where(i => i.EnquiryId == id.Value).Count() > 0)
                {
                    var quotes = viewModel.Enquiries
                        .Where(i => i.EnquiryId == id.Value).SingleOrDefault().Quotes;
                    //set the view models quotes
                    
                    viewModel.Quotes = quotes;

                }
                ViewBag.EnquiryId = id;
            }

            ViewBag.stageFilter = stageFilter;
            ViewBag.filter = searchString;
            return View(viewModel);
        }


        // GET: Enquiries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId<int>();
            Enquiry enquiry = db.Enquiries.Find(id);
            if (enquiry == null)
            {
                return HttpNotFound();
            }
            if (userId == enquiry.ApplicationUserId || userManager.IsInRole(userId, "Admin") || userManager.IsInRole(userId, "EnquiryManager"))
            {
                //let user view the details


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
            UserCreateEditEnquiryViewModel newEnquiry = new UserCreateEditEnquiryViewModel();
            newEnquiry.InvoiceEmail = User.Identity.GetUserDefaultInvoicingEmail();
            newEnquiry.NoOfYears = 10;
            newEnquiry.DataUsedFor = "Ecological report";
            ViewBag.SearchTypeId = new SelectList(db.SearchTypes, "SearchTypeId", "Name");
            //get all boundaries for displaying on map
            SpatialHelper spatial = new SpatialHelper();
            ViewBag.Boundaries = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.DISPLAY);
            ViewBag.Coverage = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.COVERAGE);
            return View(newEnquiry);
            //test comment
        }

        // POST: Enquiries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code, EnquiryId,Name,InvoiceEmail,SearchAreaWkt,SearchTypeId,NoOfYears,EstimatedCost,JobNumber,Agency,AgencyContact,DataUsedFor,Citations,GisKml,Express,EnquiryDate,Comment")] UserCreateEditEnquiryViewModel model)
        {
            if (ModelState.IsValid)
            {
                DbGeometry geom = DbGeometry.FromText(model.SearchAreaWkt, 27700); //(EPSG:27700) is OSGB, (EPSG:3857) google maps geometric, wgs84 (EPSG:4326) google geographic
                var defaultInvoiceEmail = User.Identity.GetUserDefaultInvoicingEmail();
                var user = User.Identity.GetAppUser();
                var userId = User.Identity.GetIntUserId();
                if (model.InvoiceEmail != null)
                {
                    defaultInvoiceEmail = model.InvoiceEmail;
                }
                Enquiry enquiry = new Enquiry
                {
                    Code = model.Code,
                    ApplicationUserId = userId,
                    Name = model.Name,
                    InvoiceEmail = defaultInvoiceEmail,
                    SearchArea = geom,
                    SearchTypeId = model.SearchTypeId,
                    EstimatedCost = model.EstimatedCost,
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
                    FormatHelper formatHelper = new FormatHelper();
                    var enquiryCode = formatHelper.GetEnquiryCodePrefix(enquiry.EnquiryId);
                    enquiry.Code = enquiryCode;
                    db.Entry(enquiry).State = EntityState.Modified;
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
            var userId = User.Identity.GetUserId<int>();
            Enquiry enquiry = db.Enquiries.Include(e => e.Invoice)
                    .Include(q => q.Quotes).FirstOrDefault(e => e.EnquiryId == id);
            if (enquiry == null)
            {
                return HttpNotFound();
            }

            if (userId == enquiry.ApplicationUserId || userManager.IsInRole(userId, "Admin") || userManager.IsInRole(userId, "EnquiryManager"))
            {
                //do not allow editing of enquiry if there are quotes
                if (enquiry.Quotes.Count > 0)
                {
                    if (enquiry.Quotes.Where(q => q.AcceptedDate != null).Count() > 0)
                    {
                        TempData["ErrorMessage"] = "Cannot edit an enquiry that has an accepted quote, please contact us if there is an issue";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Cannot edit an enquiry that has been quoted, please delete all quotes for this enquiry and try again";
                    }

                    // send user back to the index
                    return RedirectToAction("Index", "Enquiries");
                }
                FormatHelper formatHelper = new FormatHelper();
                var wkt = enquiry.SearchArea.WellKnownValue.WellKnownText;
                UserCreateEditEnquiryViewModel userCreateEditEnquiryViewModel = new UserCreateEditEnquiryViewModel
                {
                    Code = enquiry.Code,
                    EnquiryId = enquiry.EnquiryId,
                    Name = enquiry.Name,
                    InvoiceEmail = enquiry.InvoiceEmail,
                    SearchAreaWkt = wkt,
                    SearchTypeId = enquiry.SearchTypeId,
                    NoOfYears = enquiry.NoOfYears,
                    EstimatedCost = enquiry.EstimatedCost,
                    JobNumber = enquiry.JobNumber,
                    Agency = enquiry.Agency,
                    AgencyContact = enquiry.AgencyContact,
                    DataUsedFor = enquiry.DataUsedFor,
                    Citations = enquiry.Citations,
                    GisKml = enquiry.GisKml,
                    Express = enquiry.Express,
                    Comment = enquiry.Comment,
                    AddedToRersDate = enquiry.AddedToRersDate,
                    DataCleanedDate = enquiry.DataCleanedDate,
                    ReportCompleteDate = enquiry.ReportCompleteDate,
                    DocumentsCleanedDate = enquiry.DocumentsCleanedDate,
                    EnquiryDeliveredDate = enquiry.EnquiryDeliveredDate,
                    AdminComment = enquiry.AdminComment,
                    RowVersion = enquiry.RowVersion
                };
                ViewBag.SearchTypeId = new SelectList(db.SearchTypes, "SearchTypeId", "Name", userCreateEditEnquiryViewModel.SearchTypeId);
                //get all boundaries for displaying on map
                SpatialHelper spatial = new SpatialHelper();
                ViewBag.Boundaries = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.DISPLAY);
                ViewBag.Coverage = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.COVERAGE);
                return View(userCreateEditEnquiryViewModel);
            }
            else
            {
                // send user back to the index
                return RedirectToAction("Index", "Enquiries");
            }
        }

        // POST: Enquiries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Code, EnquiryId,Name,InvoiceEmail,SearchAreaWkt,SearchTypeId" +
            ",NoOfYears,EstimatedCost,JobNumber,Agency,AgencyContact,DataUsedFor,Citations,GisKml,Express,EnquiryDate" +
            ",Comment,AddedToRersDate, DataCleanedDate, ReportCompleteDate, DocumentsCleanedDate, EnquiryDeliveredDate" +
            ", AdminComment, RowVersion")] UserCreateEditEnquiryViewModel model)
        {
            SpatialHelper spatial = new SpatialHelper();
            ViewBag.SearchTypeId = new SelectList(db.SearchTypes, "SearchTypeId", "Name", model.SearchTypeId);
            //get all boundaries for displaying on map
            ViewBag.Boundaries = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.DISPLAY);
            ViewBag.Coverage = spatial.GetGeoJsonCollectionFromBoundaryCollection(db.Boundaries.ToList(), SpatialHelper.BoundaryType.COVERAGE);

            if (ModelState.IsValid)
            {
                DbGeometry geom = DbGeometry.FromText(model.SearchAreaWkt, 27700);
                var user = User.Identity.GetAppUser();
                var userId = User.Identity.GetIntUserId();
                Enquiry enquiry = new Enquiry
                {
                    Code = model.Code,
                    EnquiryId = model.EnquiryId,
                    ApplicationUserId = userId,
                    Name = model.Name,
                    InvoiceEmail = model.InvoiceEmail,
                    SearchArea = geom,
                    SearchTypeId = model.SearchTypeId,
                    NoOfYears = model.NoOfYears,
                    EstimatedCost = model.EstimatedCost,
                    JobNumber = model.JobNumber,
                    Agency = model.Agency,
                    AgencyContact = model.AgencyContact,
                    DataUsedFor = model.DataUsedFor,
                    Citations = model.Citations,
                    GisKml = model.GisKml,
                    Express = model.Express,
                    Comment = model.Comment,
                    AddedToRersDate = model.AddedToRersDate,
                    DataCleanedDate = model.DataCleanedDate,
                    ReportCompleteDate = model.ReportCompleteDate,
                    DocumentsCleanedDate = model.DocumentsCleanedDate,
                    EnquiryDeliveredDate = model.EnquiryDeliveredDate,
                    AdminComment = model.AdminComment,
                    RowVersion = model.RowVersion
                };
                try
                {
                    db.Entry(enquiry).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ViewBag.Message = "Sorry, couldn't update due to a concurrency issue <br />Please try again";

                    return View(model);
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var result in ex.EntityValidationErrors)
                        foreach (var error in result.ValidationErrors)
                            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    return View(model);
                }
            }

            return View(model);
        }

        // GET: Enquiries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId<int>();
            Enquiry enquiry = db.Enquiries.Find(id);
            if (enquiry == null)
            {
                return HttpNotFound();
            }
            if (userId == enquiry.ApplicationUserId || userManager.IsInRole(userId, "Admin") || userManager.IsInRole(userId, "EnquiryManager"))
            {
                //do not allow deleting of enquiry if there is an accepted quote
                if (enquiry.Quotes.Where(q => q.AcceptedDate != null).Count() > 0)
                {
                    TempData["ErrorMessage"] = "Cannot delete an enquiry that has an accepted quote, please contact us if there is an issue";
                    // send user back to the index
                    return RedirectToAction("Index", "Enquiries");
                }

                //let user view the details
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

        // POST: Enquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Enquiry enquiry = db.Enquiries.Find(id);
                db.Enquiries.Remove(enquiry);
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
