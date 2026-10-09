namespace ContosoUniversity.Models
{
    /// <summary>
    /// Tutorial 5: join entity for the many-to-many relationship
    /// between Instructor and Course. Its key is the composite
    /// CourseID + InstructorID, configured in SchoolContext.OnModelCreating.
    /// </summary>
    public class CourseAssignment
    {
        public int InstructorID { get; set; }
        public int CourseID { get; set; }

        public Instructor Instructor { get; set; }
        public Course Course { get; set; }
    }
}
