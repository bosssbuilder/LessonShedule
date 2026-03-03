using System;
using System.Collections.Generic;
using System.Drawing;
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
                string color = finalValues.Last().Trim();
                string teacher = string.Join(" ", finalValues.Skip(2).Take(finalValues.Count - 3));

                var lesson = new Lesson(date, audience, teacher);
                lesson.Color = color;

                return lesson;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void PrintLesson(Lesson lesson)
        {
            Console.WriteLine($"Date: {lesson.Date:yyyy-MM-dd}; Audience: {lesson.Audience}; Teacher: {lesson.Teacher}; Color: {lesson.Color}");
        }
    }
}