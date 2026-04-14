using System;

namespace LessonScheduleNew
{
    public class Lesson
    {
        public DateTime Date { get; private set; }
        public string Audience { get; private set; }
        public string Teacher { get; private set; }

        public Lesson(DateTime date, string audience, string teacher)
        {
            if (string.IsNullOrWhiteSpace(audience))
                throw new ArgumentException("Аудитория не может быть пустой");
            if (string.IsNullOrWhiteSpace(teacher))
                throw new ArgumentException("Преподаватель не может быть пустым");

            Date = date;
            Audience = audience;
            Teacher = teacher;
        }

        public override string ToString()
        {
            return $"{Date:yyyy.MM.dd};{Audience};{Teacher}";
        }
    }
}