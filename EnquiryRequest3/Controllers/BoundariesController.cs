using EnquiryRequest3.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;


namespace EnquiryRequest3.Controllers
{
    public class BoundariesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Boundaries
        public ActionResult Index()
        {
            return View(db.Boundaries.ToList());
        }

        // GET: Boundaries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boundary boundary = db.Boundaries.Find(id);
            if (boundary == null)
            {
                return HttpNotFound();
            }
            return View(boundary);
        }

        // GET: Boundaries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Boundaries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BoundaryId,Name,Area")] Boundary boundary)
        {
            if (ModelState.IsValid)
            {
                db.Boundaries.Add(boundary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(boundary);
        }

        // GET: Boundaries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boundary boundary = db.Boundaries.Find(id);
            if (boundary == null)
            {
                return HttpNotFound();
            }
            return View(boundary);
        }

        // POST: Boundaries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BoundaryId,Name,Area, RowVersion")] Boundary boundary)
        {
            if (!ModelState.IsValid) return View(boundary);

            try
            {
                db.Entry(boundary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewBag.Message = "Sorry, couldn't update due to a concurrency issue <br />Please try again";
                return View(boundary);
            }
        }

        // GET: Boundaries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boundary boundary = db.Boundaries.Find(id);
            if (boundary == null)
            {
                return HttpNotFound();
            }
            return View(boundary);
        }

        // POST: Boundaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Boundary boundary = db.Boundaries.Find(id);
                db.Boundaries.Remove(boundary);
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
