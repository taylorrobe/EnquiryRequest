using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnquiryRequest3.Models;

namespace EnquiryRequest3.Controllers
{
    public class SearchTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SearchTypes
        public ActionResult Index()
        {
            return View(db.SearchTypes.ToList());
        }

        // GET: SearchTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SearchType searchType = db.SearchTypes.Find(id);
            if (searchType == null)
            {
                return HttpNotFound();
            }
            return View(searchType);
        }

        // GET: SearchTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SearchTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SearchTypeId,Name,Description")] SearchType searchType)
        {
            if (ModelState.IsValid)
            {
                db.SearchTypes.Add(searchType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(searchType);
        }

        // GET: SearchTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SearchType searchType = db.SearchTypes.Find(id);
            if (searchType == null)
            {
                return HttpNotFound();
            }
            return View(searchType);
        }

        // POST: SearchTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SearchTypeId,Name,Description, RowVersion")] SearchType searchType)
        {
            if (!ModelState.IsValid) return View(searchType);
            try
            {
                db.Entry(searchType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewBag.Message = "Sorry, couldn't update due to a concurrency issue <br />Please try again";
                return View(searchType);
            }

        }

        // GET: SearchTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SearchType searchType = db.SearchTypes.Find(id);
            if (searchType == null)
            {
                return HttpNotFound();
            }
            return View(searchType);
        }

        // POST: SearchTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                SearchType searchType = db.SearchTypes.Find(id);
                db.SearchTypes.Remove(searchType);
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
