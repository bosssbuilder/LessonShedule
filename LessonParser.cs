using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace LessonScheduleNew
{
    public class ParseError
    {
        public int LineNumber { get; set; }
        public string Line { get; set; }
        public string ErrorMessage { get; set; }
    }

    public static class LessonParser
    {
        public static Lesson Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Пустая строка");

            string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 3)
                throw new ArgumentException($"Недостаточно данных: получено {parts.Length} частей");

            if (!DateTime.TryParseExact(parts[0], "yyyy.MM.dd", null, DateTimeStyles.None, out var date))
                throw new ArgumentException($"Неверный формат даты: {parts[0]}");

            string audience = parts[1];

            if (string.IsNullOrWhiteSpace(audience) ||
                !Regex.IsMatch(audience, @"^([A-Za-zА-Яа-я]{1,3}-\d+|[A-Za-zА-Яа-я]\d{3})$"))
            {
                throw new ArgumentException($"Неверный формат аудитории: {audience}");
            }

            string teacher = string.Join(" ", parts, 2, parts.Length - 2);

            if (!IsValidTeacherName(teacher))
                throw new ArgumentException($"Неверный формат ФИО преподавателя: {teacher}");

            return new Lesson(date, audience, teacher);
        }

        private static bool IsValidTeacherName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return Regex.IsMatch(name, @"^[A-Za-zА-Яа-я\s\.\-]+$");
        }

        public static (List<Lesson> Lessons, List<ParseError> Errors) LoadFromFileWithLog(string filePath)
        {
            var lessons = new List<Lesson>();
            var errors = new List<ParseError>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrEmpty(line))
                    continue;

                try
                {
                    lessons.Add(Parse(line));
                }
                catch (Exception ex)
                {
                    errors.Add(new ParseError
                    {
                        LineNumber = i + 1,
                        Line = line,
                        ErrorMessage = ex.Message
                    });
                }
            }

            return (lessons, errors);
        }
    }
}