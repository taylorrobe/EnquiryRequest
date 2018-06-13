using EnquiryRequest3.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace EnquiryRequest3.Controllers
{
    public class QuotesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        protected UserManager<ApplicationUser, int> userManager;
    
        public QuotesController()
        {
            userManager = new UserManager<ApplicationUser
                , int>(new UserStore<ApplicationUser
                , CustomRole
                , int
                , CustomUserLogin
                , CustomUserRole
                , CustomUserClaim>(this.db));
        }

        // GET: Quotes
        public ActionResult Index()
        {
            var quotes = db.Quotes.Include(q => q.Enquiry);
            var userId = User.Identity.GetUserId<int>();
            if (!userManager.IsInRole(userId, "Admin") && !userManager.IsInRole(userId, "EnquiryManager"))
            {
                quotes = quotes
                    .Where(a => a.Enquiry.ApplicationUserId == userId);
            }
            return View(quotes.ToList());
        }

        // GET: Quotes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId<int>();
            Quote quote = db.Quotes.Find(id);
            if (quote == null)
            {
                return HttpNotFound();
            }
            if (userId == quote.Enquiry.ApplicationUserId || userManager.IsInRole(userId, "Admin") || userManager.IsInRole(userId, "EnquiryManager"))
            {            
                //let user view the details
                return View(quote);
            }
            else
            {
                // send user back to the index
                return RedirectToAction("Index", "Quotes");
            }

        }

        // GET: Quotes/Create
        public ActionResult Create(int? enquiryId)
        {
            if(enquiryId == null)
            {
                ViewBag.EnquiryList = new SelectList(db.Enquiries, "EnquiryId", "DisplayName");
            }
            else
            {
                Enquiry selectedEnquiry = db.Enquiries.SingleOrDefault(a => a.EnquiryId == enquiryId);
                ViewBag.Enquiry = selectedEnquiry;
            }
            return View();
        }

        // POST: Quotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Amount,QuotedDate,AcceptedDate,EnquiryId")] Quote quote)
        {
            if (ModelState.IsValid)
            {
                db.Quotes.Add(quote);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if(quote.EnquiryId > 0)
            {
                Enquiry selectedEnquiry = db.Enquiries.SingleOrDefault(a => a.EnquiryId == quote.EnquiryId);
                ViewBag.Enquiry = selectedEnquiry;
            }
            
            ViewBag.EnquiryList = new SelectList(db.Enquiries, "EnquiryId", "DisplayName", quote.EnquiryId);

            return View(quote);
        }

        // GET: Quotes/Edit/5
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId<int>();
            Quote quote = db.Quotes.Find(id);
            if (quote == null)
            {
                return HttpNotFound();
            }
            if (userId == quote.Enquiry.ApplicationUserId || userManager.IsInRole(userId, "Admin") || userManager.IsInRole(userId, "EnquiryManager"))
            {
                ViewBag.EnquiryId = new SelectList(db.Enquiries, "EnquiryId", "Code", quote.EnquiryId);
                return View(quote);
            }
            else
            {
                // send user back to the index
                return RedirectToAction("Index", "Quotes");
            }
        }

        // POST: Quotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, EnquiryManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QuoteId,Amount,QuotedDate,AcceptedDate,EnquiryId, RowVersion")] Quote quote)
        {
            ViewBag.EnquiryId = new SelectList(db.Enquiries, "EnquiryId", "Code", quote.EnquiryId);
            if (!ModelState.IsValid) return View(quote);
            try
            {
                db.Entry(quote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewBag.Message = "Sorry, couldn't update due to a concurrency issue <br />Please try again";
                return View(quote);
            }


        }

        // GET: Quotes/AcceptQuote/5
        public ActionResult AcceptQuote(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId<int>();
            Quote quote = db.Quotes.Find(id);
            if (quote == null)
            {
                return HttpNotFound();
            }
            if (userId == quote.Enquiry.ApplicationUserId || userManager.IsInRole(userId, "Admin") || userManager.IsInRole(userId, "EnquiryManager"))
            {
                ViewBag.EnquiryId = new SelectList(db.Enquiries, "EnquiryId", "Code", quote.EnquiryId);
                return View(quote);
            }
            else
            {
                // send user back to the index
                return RedirectToAction("Index", "Quotes");
            }
        }

        // POST: Quotes/AcceptQuote/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptQuote([Bind(Include = "QuoteId,Amount,QuotedDate,AcceptedDate,EnquiryId,RowVersion")] Quote quote)
        {
            ViewBag.EnquiryId = new SelectList(db.Enquiries, "EnquiryId", "Code", quote.EnquiryId);
            if (!ModelState.IsValid) return View(quote);
            try
            {
                var quoteDetails = db.Quotes.FirstOrDefault(q => q.QuoteId == quote.QuoteId);
                quoteDetails.AcceptedDate = DateTime.Now;
                db.Entry(quoteDetails).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewBag.Message = "Sorry, couldn't update due to a concurrency issue <br />Please try again";
                return View(quote);
            }
        }

        // GET: Quotes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId<int>();
            Quote quote = db.Quotes.Find(id);
            if (quote == null)
            {
                return HttpNotFound();
            }
            if (userId == quote.Enquiry.ApplicationUserId || userManager.IsInRole(userId, "Admin") || userManager.IsInRole(userId, "EnquiryManager"))
            {
                return View(quote);
            }
            else
            {
                // send user back to the index
                return RedirectToAction("Index", "Quotes");
            }
        }

        // POST: Quotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Quote quote = db.Quotes.Find(id);
                db.Quotes.Remove(quote);
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
