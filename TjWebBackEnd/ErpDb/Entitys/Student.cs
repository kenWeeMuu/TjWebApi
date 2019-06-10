using System.Collections.Generic;

namespace ErpDb.Entitys
{
    public class Student
    {
        public Student()
        {
            Courses = new HashSet<Course>();
        }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        /// <summary>
        /// 一个学生也可以参加很多门课程
        /// </summary>
        public virtual ICollection<Course> Courses { get; set; }
    }
}