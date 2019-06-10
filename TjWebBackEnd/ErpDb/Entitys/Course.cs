using System.Collections.Generic;

namespace ErpDb.Entitys
{

    /// <summary>
    /// 课程
    /// </summary>
    public class Course
    {
        public Course()
        {
            Students = new HashSet<Student>();
        }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        /// <summary>
        /// 一门课程可以有很多个学生参与
        /// </summary>
        public virtual ICollection<Student> Students { get; set; }
    }

 
}