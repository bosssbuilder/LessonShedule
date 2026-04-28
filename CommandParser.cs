using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LessonScheduleNew
{
    public class CommandParser
    {
        private readonly LessonService _service;
        private readonly List<string> _log;

        public CommandParser(LessonService service)
        {
            _service = service;
            _log = new List<string>();
        }

        public List<string> ExecuteCommands(string filePath)
        {
            _log.Clear();
            _log.Add($"=== Выполнение команд из файла: {filePath} ===");

            if (!File.Exists(filePath))
            {
                _log.Add($"Ошибка: Файл {filePath} не найден!");
                return _log;
            }

            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                try
                {
                    ExecuteCommand(line);
                    _log.Add($"✓ [{i + 1}] {line}");
                }
                catch (Exception ex)
                {
                    _log.Add($"✗ [{i + 1}] {line} -> Ошибка: {ex.Message}");
                }
            }

            _log.Add("=== Выполнение команд завершено ===");
            return _log;
        }

        public List<string> ExecuteCommandsFromString(string commands)
        {
            _log.Clear();
            _log.Add("=== Выполнение команд из строки ===");

            string[] lines = commands.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                try
                {
                    ExecuteCommand(line);
                    _log.Add($"✓ [{i + 1}] {line}");
                }
                catch (Exception ex)
                {
                    _log.Add($"✗ [{i + 1}] {line} -> Ошибка: {ex.Message}");
                }
            }

            _log.Add("=== Выполнение команд завершено ===");
            return _log;
        }

        private void ExecuteCommand(string commandLine)
        {
            if (commandLine.StartsWith("ADD ", StringComparison.OrdinalIgnoreCase))
            {
                string data = commandLine.Substring(4).Trim();
                AddCommand(data);
            }
            else if (commandLine.StartsWith("REM ", StringComparison.OrdinalIgnoreCase))
            {
                string condition = commandLine.Substring(4).Trim();
                RemCommand(condition);
            }
            else if (commandLine.StartsWith("SAVE ", StringComparison.OrdinalIgnoreCase))
            {
                string filename = commandLine.Substring(5).Trim();
                SaveCommand(filename);
            }
            else
            {
                throw new ArgumentException($"Неизвестная команда: {commandLine}");
            }
        }

        private void AddCommand(string data)
        {
            string[] parts = data.Split(';');
            if (parts.Length < 3)
                throw new ArgumentException($"Неверный формат ADD: {data}. Ожидается: дата;аудитория;преподаватель");

            string dateStr = parts[0].Trim();
            string audience = parts[1].Trim();
            string teacher = parts[2].Trim();

            if (!DateTime.TryParseExact(dateStr, "yyyy.MM.dd", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                throw new ArgumentException($"Неверный формат даты: {dateStr}");

            if (!Regex.IsMatch(audience, @"^[A-Za-zА-Яа-я]+-?\d+$"))
                throw new ArgumentException($"Неверный формат аудитории: {audience}");

            if (!Regex.IsMatch(teacher, @"^[A-Za-zА-Яа-я\s\.\-]+$"))
                throw new ArgumentException($"Неверный формат ФИО преподавателя: {teacher}");

            var lesson = new Lesson(date, audience, teacher);
            _service.Add(lesson);
        }

        private void RemCommand(string condition)
        {
            var lessons = _service.GetAll().ToList();
            var toRemove = new List<Lesson>();

            foreach (var lesson in lessons)
            {
                if (EvaluateCondition(lesson, condition))
                {
                    toRemove.Add(lesson);
                }
            }

            foreach (var lesson in toRemove)
            {
                int index = _service.GetAll().ToList().FindIndex(l => l == lesson);
                if (index >= 0)
                    _service.RemoveAt(index);
            }
        }

        private bool EvaluateCondition(Lesson lesson, string condition)
        {
            condition = condition.Trim();

            var teacherMatch = Regex.Match(condition, @"teacher\s*=\s*""([^""]+)""");
            if (teacherMatch.Success)
            {
                string expectedTeacher = teacherMatch.Groups[1].Value;
                return lesson.Teacher == expectedTeacher;
            }

            var audienceMatch = Regex.Match(condition, @"audience\s*=\s*""([^""]+)""");
            if (audienceMatch.Success)
            {
                string expectedAudience = audienceMatch.Groups[1].Value;
                return lesson.Audience == expectedAudience;
            }

            var dateGreaterMatch = Regex.Match(condition, @"date\s*>\s*(\d{4}\.\d{2}\.\d{2})");
            if (dateGreaterMatch.Success)
            {
                DateTime threshold = DateTime.ParseExact(dateGreaterMatch.Groups[1].Value, "yyyy.MM.dd", null);
                return lesson.Date > threshold;
            }

            var dateLessMatch = Regex.Match(condition, @"date\s*<\s*(\d{4}\.\d{2}\.\d{2})");
            if (dateLessMatch.Success)
            {
                DateTime threshold = DateTime.ParseExact(dateLessMatch.Groups[1].Value, "yyyy.MM.dd", null);
                return lesson.Date < threshold;
            }

            throw new ArgumentException($"Неподдерживаемое условие: {condition}");
        }

        private void SaveCommand(string filename)
        {
            var lessons = _service.GetAll();
            var lines = new List<string>();

            foreach (var lesson in lessons)
            {
                lines.Add($"{lesson.Date:yyyy.MM.dd};{lesson.Audience};{lesson.Teacher}");
            }

            File.WriteAllLines(filename, lines);
        }
    }
}