using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonSchedule
{
    public class Logic
    {
        public Lesson ToLesson(string str)
        {
            try
            {
                str = str.Replace("Lesson", "");
                str = str.Replace("\"", "");

                List<string> finalValues = str
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                DateTime date = DateTime.Parse(finalValues[0].Trim());
                string audience = finalValues[1].Trim();
                string teacher = string.Join(" ", finalValues.Skip(2));

                return new Lesson(date, audience, teacher);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void PrintLesson(Lesson lesson)
        {
            Console.WriteLine($"Date: {lesson.Date:yyyy-MM-dd}; Audience: {lesson.Audience}; Teacher: {lesson.Teacher}");
        }
    }
}