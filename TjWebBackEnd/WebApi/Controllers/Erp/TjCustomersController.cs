using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using ErpDb.Entitys;

namespace WebApi.Controllers.Erp
{
    public class TjCustomersController : ApiController
    {
        private ErpDbContext db = new ErpDbContext();

        // GET: api/TjCustomers
        public IQueryable<TjCustomer> GetTjCustomers()
        {
            return db.TjCustomers;
        }

        // GET: api/TjCustomers/5
        [ResponseType(typeof(TjCustomer))]
        public IHttpActionResult GetTjCustomer(int id)
        {
            TjCustomer tjCustomer = db.TjCustomers.Find(id);
            if (tjCustomer == null)
            {
                return NotFound();
            }

            return Ok(tjCustomer);
        }

        // PUT: api/TjCustomers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTjCustomer(int id, TjCustomer tjCustomer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tjCustomer.Id)
            {
                return BadRequest();
            }

            db.Entry(tjCustomer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TjCustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TjCustomers
        [ResponseType(typeof(TjCustomer))]
        public IHttpActionResult PostTjCustomer(TjCustomer tjCustomer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TjCustomers.Add(tjCustomer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TjCustomerExists(tjCustomer.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = tjCustomer.Id }, tjCustomer);
        }

        // DELETE: api/TjCustomers/5
        [ResponseType(typeof(TjCustomer))]
        public IHttpActionResult DeleteTjCustomer(int id)
        {
            TjCustomer tjCustomer = db.TjCustomers.Find(id);
            if (tjCustomer == null)
            {
                return NotFound();
            }

            db.TjCustomers.Remove(tjCustomer);
            db.SaveChanges();

            return Ok(tjCustomer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TjCustomerExists(int id)
        {
            return db.TjCustomers.Count(e => e.Id == id) > 0;
        }
    }
}