using System;

namespace LessonScheduleNew
{
    public class Lesson
    {
        public DateTime Date { get; set; }
        public string Audience { get; set; }
        public string Teacher { get; set; }
        public string Quality { get; set; }

        public Lesson(DateTime date, string audience, string teacher, string quality = "отлично")
        {
            if (string.IsNullOrWhiteSpace(audience))
                throw new ArgumentException("Аудитория не может быть пустой");

            if (string.IsNullOrWhiteSpace(teacher))
                throw new ArgumentException("Преподаватель не может быть пустым");

            if (string.IsNullOrWhiteSpace(quality))
                throw new ArgumentException("Качество не может быть пустым");

            Date = date;
            Audience = audience;
            Teacher = teacher;
            Quality = quality;
        }

        public override string ToString()
        {
            return $"{Date:yyyy.MM.dd};{Audience};{Teacher};{Quality}";
        }
    }
}