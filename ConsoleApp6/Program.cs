using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

public class Student
{
    public int StudentId { get; set; }
    public string Name { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public List<Lecture> Lectures { get; set; } = new List<Lecture>();
}

public class Lecture
{
    public int LectureId { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public int Capacity { get; set; }
    public List<Student> Students { get; set; } = new List<Student>();
}

public class AppDbContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Lecture> Lectures { get; set; }

    public string DbPath { get; }

    public AppDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "student.db");
    }



    protected override void OnConfiguring(DbContextOptionsBuilder options)
      =>  options.UseSqlite($"Data Source={DbPath}"); 
   
}


public class LectureRepository
{
    private readonly AppDbContext _context;

    public LectureRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddLecture(Lecture lecture)
    {
        _context.Lectures.Add(lecture);
        _context.SaveChanges();
    }

    public Lecture findLecture(int lectureId)
    {
        return _context.Lectures.Find(lectureId);
    }
}


class Program
{
    static void Main()
    {
        using (var context = new AppDbContext())
        {
            var lectureRepo = new LectureRepository(context);

            var lecture = new Lecture
            {
                Title = ".NET Technologies",
                Date = DateTime.Now,
                Capacity = 30
            };

            var student1 = new Student { Name = "Ani", EnrollmentDate = DateTime.Now };
            var student2 = new Student { Name = "Giorgi", EnrollmentDate = DateTime.Now };

            lecture.Students.Add(student1);
            lecture.Students.Add(student2);

            lectureRepo.AddLecture(lecture);

            var storedLecture = lectureRepo.findLecture(lecture.LectureId);
            if (storedLecture != null)
            {
                Console.WriteLine($"Lecture: {storedLecture.Title}, Date: {storedLecture.Date}");
                foreach (var student in storedLecture.Students)
                {
                    Console.WriteLine($"Student: {student.Name}");
                }
            }
            else
            {
                Console.WriteLine("No lecture found.");
            }
        }
    }
}
