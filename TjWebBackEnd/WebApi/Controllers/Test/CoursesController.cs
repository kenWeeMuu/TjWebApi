using System.Web.Http;

namespace WebApi.Controllers.Test
{
   // [ApiAuthorize]
    public class CoursesController : ApiController
    {
//        private TrainContext db = new TrainContext();
//
//        // GET: api/Courses
//        public IQueryable<Course> GetCourses()
//        {
//            return db.Courses;
//        }
//
//        // GET: api/Courses/5
//        [ResponseType(typeof(Course))]
//        public IHttpActionResult GetCourse(int id)
//        {
//            Course course = db.Courses.Find(id);
//            if (course == null)
//            {
//                return NotFound();
//            }
//
//            return Ok(course);
//        }
//
//        // PUT: api/Courses/5
//        [ResponseType(typeof(void))]
//        public IHttpActionResult PutCourse(int id, Course course)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }
//
//            if (id != course.CourseId)
//            {
//                return BadRequest();
//            }
//
//            db.Entry(course).State = EntityState.Modified;
//
//            try
//            {
//                db.SaveChanges();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!CourseExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }
//
//            return StatusCode(HttpStatusCode.NoContent);
//        }
//
//        // POST: api/Courses
//        [ResponseType(typeof(Course))]
//        public IHttpActionResult PostCourse(Course course)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }
//
//            db.Courses.Add(course);
//            db.SaveChanges();
//
//            return CreatedAtRoute("DefaultApi", new { id = course.CourseId }, course);
//        }
//
//        // DELETE: api/Courses/5
//        [ResponseType(typeof(Course))]
//        public IHttpActionResult DeleteCourse(int id)
//        {
//            Course course = db.Courses.Find(id);
//            if (course == null)
//            {
//                return NotFound();
//            }
//
//            db.Courses.Remove(course);
//            db.SaveChanges();
//
//            return Ok(course);
//        }
//
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//
//        private bool CourseExists(int id)
//        {
//            return db.Courses.Count(e => e.CourseId == id) > 0;
//        }
    }
}