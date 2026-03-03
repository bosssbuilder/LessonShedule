using System;

namespace LessonSchedule
{
    public class Lesson
    {
        public DateTime Date { get; set; }
        public string Audience { get; set; }
        public string Teacher { get; set; }
        public string Color { get; set; }

        public Lesson(DateTime date, string audience, string teacher)
        {
            Date = date;
            Audience = audience;
            Teacher = teacher;
        }
    }
}