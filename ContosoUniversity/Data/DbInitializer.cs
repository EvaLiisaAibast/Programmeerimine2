using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContosoUniversity.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SchoolContext context)
        {
            // Tutorial 1 created the database with EnsureCreated:
            //     context.Database.EnsureCreated();
            // Tutorial 4 replaces that with EF Core migrations. Migrate() applies any
            // pending migrations - the programmatic equivalent of running
            // `dotnet ef database update` from the command line.
            context.Database.Migrate();

            // The demo dataset is deliberately huge (the lists are the show).
            // Skip when it is already in place so restarts never wipe edited data;
            // a database with fewer students than this is the old tutorial seed
            // (or an empty one), and gets rebuilt.
            if (context.Students.Count() >= 500)
            {
                return;
            }

            // Clear everything in FK-safe order so the seed can start from zero.
            context.Database.ExecuteSqlRaw(
                "DELETE FROM Enrollment;" +
                "DELETE FROM CourseAssignment;" +
                "DELETE FROM OfficeAssignment;" +
                "DELETE FROM Course;" +
                "DELETE FROM Department;" +
                "DELETE FROM Person;");

            // Fixed seed => the same 500 students on every rebuild.
            var rng = new Random(4242);

            // ------------------------------------------------------------------
            // Person names: first x last pairs, shuffled once and drawn from a
            // shared queue so no two people share the same full name.
            // ------------------------------------------------------------------
            string[] firstNames = {
                "Ava", "Liam", "Noah", "Emma", "Olivia", "Elijah", "James", "William",
                "Benjamin", "Lucas", "Henry", "Theodore", "Jack", "Levi", "Oliver", "Leo",
                "Hugo", "Felix", "Oscar", "Louis", "Mia", "Amelia", "Sophia", "Isabella",
                "Charlotte", "Luna", "Harper", "Evelyn", "Aria", "Chloe", "Nora", "Zoe",
                "Layla", "Ruby", "Iris", "Elsa", "Freya", "Ingrid", "Aino", "Matti",
                "Juho", "Liis", "Kadi", "Marek", "Tõnis", "Peeter", "Karl", "Annika",
                "Siim", "Maarja", "Dmitri", "Olga", "Ivan", "Yuki", "Hana", "Haruto",
                "Aarav", "Diya", "Rohan", "Priya", "Mateo", "Sofia", "Camila", "Diego",
                "Lucia", "Paulo", "Nadia", "Omar", "Karim", "Amina", "Chidi", "Amara",
                "Kwame", "Zainab", "Thabo", "Naledi", "Elif", "Deniz", "Sofia", "Marco"
            };
            string[] lastNames = {
                "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
                "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas",
                "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White",
                "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson", "Walker", "Young",
                "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
                "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell",
                "Carter", "Roberts", "Aibast", "Vahter", "Põldmäe", "Tammisaar", "Kivisild", "Ots",
                "Metsar", "Luik", "Veski", "Allik", "Karjus", "Kask", "Tamm", "Sepp",
                "Kuusk", "Ilves", "Pärn", "Varik", "Lepp", "Mägi", "Saar", "Rüütli"
            };

            var pairs = new List<(string First, string Last)>();
            foreach (var f in firstNames)
                foreach (var l in lastNames)
                    pairs.Add((f, l));

            // Fisher-Yates shuffle with the fixed seed.
            for (int i = pairs.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (pairs[i], pairs[j]) = (pairs[j], pairs[i]);
            }

            // Original tutorial names stay reserved so the classic sample rows survive.
            var reserved = new HashSet<string>
            {
                "Carson|Alexander", "Meredith|Alonso", "Arturo|Anand", "Gytis|Barzdukas",
                "Yan|Li", "Peggy|Justice", "Laura|Norman", "Nino|Olivetto",
                "Kim|Abercrombie", "Fadi|Fakhouri", "Roger|Harui", "Candace|Kapoor", "Roger|Zheng"
            };

            int pairCursor = 0;
            (string First, string Last) NextName()
            {
                while (pairCursor < pairs.Count)
                {
                    var p = pairs[pairCursor++];
                    if (!reserved.Contains(p.First + "|" + p.Last))
                        return p;
                }
                throw new InvalidOperationException("Name pool exhausted.");
            }

            // ------------------------------------------------------------------
            // Instructors: the original five plus 45 more => 50.
            // ------------------------------------------------------------------
            var instructors = new List<Instructor>
            {
                new Instructor { FirstMidName = "Kim",     LastName = "Abercrombie",
                    HireDate = DateTime.Parse("1995-03-11") },
                new Instructor { FirstMidName = "Fadi",    LastName = "Fakhouri",
                    HireDate = DateTime.Parse("2002-07-06") },
                new Instructor { FirstMidName = "Roger",   LastName = "Harui",
                    HireDate = DateTime.Parse("1998-07-01") },
                new Instructor { FirstMidName = "Candace", LastName = "Kapoor",
                    HireDate = DateTime.Parse("2001-01-15") },
                new Instructor { FirstMidName = "Roger",   LastName = "Zheng",
                    HireDate = DateTime.Parse("2004-02-12") }
            };

            for (int i = 0; i < 45; i++)
            {
                var n = NextName();
                instructors.Add(new Instructor
                {
                    FirstMidName = n.First,
                    LastName = n.Last,
                    HireDate = new DateTime(1985 + rng.Next(0, 41), rng.Next(1, 13), rng.Next(1, 28))
                });
            }

            context.Instructors.AddRange(instructors);
            context.SaveChanges();

            // ------------------------------------------------------------------
            // Departments: the tutorial's four, administered by the original staff.
            // ------------------------------------------------------------------
            var departments = new Department[]
            {
                new Department { Name = "English",     Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Abercrombie").ID },
                new Department { Name = "Mathematics", Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Fakhouri").ID },
                new Department { Name = "Engineering", Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Harui").ID },
                new Department { Name = "Economics",   Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Kapoor").ID }
            };

            context.Departments.AddRange(departments);
            context.SaveChanges();

            // ------------------------------------------------------------------
            // Courses: the original seven plus 33 more => 40.
            // CourseID is not auto-generated (DatabaseGenerated.None).
            // ------------------------------------------------------------------
            var courseDefs = new (int Id, string Title, int Credits, string Dept)[]
            {
                // the tutorial's original seven
                (1050, "Chemistry",             3, "Engineering"),
                (4022, "Microeconomics",        3, "Economics"),
                (4041, "Macroeconomics",        3, "Economics"),
                (1045, "Calculus",              4, "Mathematics"),
                (3141, "Trigonometry",          4, "Mathematics"),
                (2021, "Composition",           3, "English"),
                (2042, "Literature",            4, "English"),
                // Engineering
                (1055, "Physics",               4, "Engineering"),
                (1056, "Thermodynamics",        3, "Engineering"),
                (1057, "Structural Analysis",   4, "Engineering"),
                (1058, "Digital Circuits",      3, "Engineering"),
                (1059, "Materials Science",     3, "Engineering"),
                (1060, "Fluid Mechanics",       4, "Engineering"),
                (1061, "Organic Chemistry",     4, "Engineering"),
                (1062, "Programming Fundamentals", 3, "Engineering"),
                (1063, "Circuit Theory",        3, "Engineering"),
                // Mathematics
                (2055, "Linear Algebra",        4, "Mathematics"),
                (2056, "Differential Equations", 4, "Mathematics"),
                (2057, "Probability Theory",    3, "Mathematics"),
                (2058, "Discrete Mathematics",  3, "Mathematics"),
                (2059, "Number Theory",         3, "Mathematics"),
                (2060, "Statistics I",          3, "Mathematics"),
                (2061, "Real Analysis",         4, "Mathematics"),
                (2062, "Numerical Methods",     3, "Mathematics"),
                (2063, "Topology",              4, "Mathematics"),
                // Economics
                (3055, "Econometrics",          4, "Economics"),
                (3056, "Game Theory",           3, "Economics"),
                (3057, "Public Finance",        3, "Economics"),
                (3058, "International Trade",   3, "Economics"),
                (3059, "Money and Banking",     3, "Economics"),
                (3060, "Labor Economics",       3, "Economics"),
                (3061, "Development Economics", 3, "Economics"),
                // English
                (4055, "Creative Writing",      3, "English"),
                (4056, "Shakespeare Studies",   4, "English"),
                (4057, "Modern Poetry",         3, "English"),
                (4058, "Linguistics",           4, "English"),
                (4059, "World Fiction",         3, "English"),
                (4060, "Technical Writing",     3, "English"),
                (4061, "Journalism",            3, "English"),
                (4062, "Drama and Theatre",     3, "English")
            };

            var courses = courseDefs.Select(d => new Course
            {
                CourseID = d.Id,
                Title = d.Title,
                Credits = d.Credits,
                DepartmentID = departments.Single(s => s.Name == d.Dept).DepartmentID
            }).ToList();

            context.Courses.AddRange(courses);
            context.SaveChanges();

            // ------------------------------------------------------------------
            // Offices: everyone gets one (the original three keep theirs).
            // ------------------------------------------------------------------
            string[] buildings = { "Smith", "Gowan", "Thompson", "Newton", "Faraday",
                                   "Hawking", "Aristotle", "Curie", "Lovelace", "Turing" };
            var officeAssignments = new List<OfficeAssignment>
            {
                new OfficeAssignment { InstructorID = instructors.Single(i => i.LastName == "Fakhouri").ID,
                    Location = "Smith 17" },
                new OfficeAssignment { InstructorID = instructors.Single(i => i.LastName == "Harui").ID,
                    Location = "Gowan 27" },
                new OfficeAssignment { InstructorID = instructors.Single(i => i.LastName == "Kapoor").ID,
                    Location = "Thompson 304" }
            };
            foreach (var ins in instructors)
            {
                if (officeAssignments.Any(o => o.InstructorID == ins.ID))
                    continue;
                officeAssignments.Add(new OfficeAssignment
                {
                    InstructorID = ins.ID,
                    Location = buildings[rng.Next(buildings.Length)] + " " + rng.Next(101, 420)
                });
            }

            context.OfficeAssignments.AddRange(officeAssignments);
            context.SaveChanges();

            // ------------------------------------------------------------------
            // Teaching assignments: every course has a teacher, every teacher
            // has at least one course (up to four).
            // ------------------------------------------------------------------
            var assignmentKeys = new HashSet<(int CourseID, int InstructorID)>();
            foreach (var c in courses)
                assignmentKeys.Add((c.CourseID, instructors[rng.Next(instructors.Count)].ID));

            foreach (var ins in instructors)
            {
                int load = 1 + rng.Next(4);
                for (int k = 0; k < load; k++)
                {
                    var c = courses[rng.Next(courses.Count)];
                    assignmentKeys.Add((c.CourseID, ins.ID)); // HashSet drops duplicates
                }
            }

            context.CourseAssignments.AddRange(assignmentKeys.Select(a => new CourseAssignment
            {
                CourseID = a.CourseID,
                InstructorID = a.InstructorID
            }));
            context.SaveChanges();

            // ------------------------------------------------------------------
            // Students: the original eight plus 492 more => 500.
            // Enrollment dates are quarterly firsts between 2004 and 2026 so the
            // About page's GROUP BY has plenty to group.
            // ------------------------------------------------------------------
            var enrollmentDates = new List<DateTime>();
            for (int y = 2004; y <= 2026; y++)
                foreach (int m in new[] { 3, 6, 9, 12 })
                    enrollmentDates.Add(new DateTime(y, m, 1));

            var students = new List<Student>
            {
                new Student { FirstMidName = "Carson",   LastName = "Alexander",
                    EnrollmentDate = DateTime.Parse("2010-09-01") },
                new Student { FirstMidName = "Meredith", LastName = "Alonso",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { FirstMidName = "Arturo",   LastName = "Anand",
                    EnrollmentDate = DateTime.Parse("2013-09-01") },
                new Student { FirstMidName = "Gytis",    LastName = "Barzdukas",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { FirstMidName = "Yan",      LastName = "Li",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { FirstMidName = "Peggy",    LastName = "Justice",
                    EnrollmentDate = DateTime.Parse("2011-09-01") },
                new Student { FirstMidName = "Laura",    LastName = "Norman",
                    EnrollmentDate = DateTime.Parse("2013-09-01") },
                new Student { FirstMidName = "Nino",     LastName = "Olivetto",
                    EnrollmentDate = DateTime.Parse("2005-09-01") }
            };

            while (students.Count < 500)
            {
                var n = NextName();
                students.Add(new Student
                {
                    FirstMidName = n.First,
                    LastName = n.Last,
                    EnrollmentDate = enrollmentDates[rng.Next(enrollmentDates.Count)]
                });
            }

            context.Students.AddRange(students);
            context.SaveChanges();

            // ------------------------------------------------------------------
            // Enrollments: 2-6 courses per student (a few part-timers),
            // roughly one in four still ungraded.
            // ------------------------------------------------------------------
            Grade? PickGrade()
            {
                int roll = rng.Next(100);
                if (roll < 25) return null;      // still in progress
                if (roll < 43) return Grade.A;
                if (roll < 70) return Grade.B;
                if (roll < 95) return Grade.C;
                if (roll < 99) return Grade.D;
                return Grade.F;
            }

            var courseIds = courses.Select(c => c.CourseID).ToList();
            var taken = new HashSet<(int StudentID, int CourseID)>();
            var enrollments = new List<Enrollment>();

            foreach (var s in students)
            {
                int wanted = rng.Next(10) == 0 ? rng.Next(0, 2) : rng.Next(2, 7);
                int attempts = 0;
                while (wanted > 0 && attempts++ < 30)
                {
                    int cid = courseIds[rng.Next(courseIds.Count)];
                    if (!taken.Add((s.ID, cid)))
                        continue;
                    enrollments.Add(new Enrollment
                    {
                        StudentID = s.ID,
                        CourseID = cid,
                        Grade = PickGrade()
                    });
                    wanted--;
                }
            }

            context.Enrollments.AddRange(enrollments);
            context.SaveChanges();
        }
    }
}
