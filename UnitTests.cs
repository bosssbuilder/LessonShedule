using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace LessonScheduleNew.Tests
{
    [TestFixture]
    public class LessonParserTests
    {
        [Test]
        public void Parse_ValidInput_ReturnsLesson()
        {
            var lesson = LessonParser.Parse("2024.09.01 A-101 Иванов И.И.");
            NUnit.Framework.Assert.That(lesson.Date, Is.EqualTo(new DateTime(2024, 9, 1)));
            NUnit.Framework.Assert.That(lesson.Audience, Is.EqualTo("A-101"));
            NUnit.Framework.Assert.That(lesson.Teacher, Is.EqualTo("Иванов И.И."));
        }

        [Test]
        public void Parse_ValidRoomWithoutDash_ReturnsLesson()
        {
            var lesson = LessonParser.Parse("2024.09.01 B202 Петрова А.С.");
            NUnit.Framework.Assert.That(lesson.Audience, Is.EqualTo("B202"));
        }

        [Test]
        public void Parse_ValidRoomWithDash_ReturnsLesson()
        {
            var lesson = LessonParser.Parse("2024.09.01 LAB-3 Сидоров С.С.");
            NUnit.Framework.Assert.That(lesson.Audience, Is.EqualTo("LAB-3"));
        }

        [Test]
        public void Parse_InvalidDate_ThrowsException()
        {
            NUnit.Framework.Assert.Throws<ArgumentException>(() => LessonParser.Parse("2024/09/01 A-101 Иванов"));
        }

        [Test]
        public void Parse_InsufficientData_ThrowsException()
        {
            NUnit.Framework.Assert.Throws<ArgumentException>(() => LessonParser.Parse("2024.09.01 A-101"));
        }

        [Test]
        public void Parse_InvalidAudience_ThrowsException()
        {
            NUnit.Framework.Assert.Throws<ArgumentException>(() => LessonParser.Parse("2024.09.01 room123 Иванов"));
        }

        [Test]
        public void Parse_InvalidTeacherName_ThrowsException()
        {
            NUnit.Framework.Assert.Throws<ArgumentException>(() => LessonParser.Parse("2024.09.01 A-101 @#$%^&*()"));
        }

        [Test]
        public void LoadFromFileWithLog_SkipsInvalidLines()
        {
            string tempFile = Path.GetTempFileName();
            File.WriteAllLines(tempFile, new[]
            {
                "2024.09.01 A-101 Иванов И.И.",
                "bad line",
                "2024.09.02 B-202 Петрова А.С."
            });

            var (lessons, errors) = LessonParser.LoadFromFileWithLog(tempFile);
            NUnit.Framework.Assert.That(lessons.Count, Is.EqualTo(2));
            NUnit.Framework.Assert.That(errors.Count, Is.EqualTo(1));

            File.Delete(tempFile);
        }
    }

    [TestFixture]
    public class LessonServiceTests
    {
        [Test]
        public void Add_IncreasesCount()
        {
            var service = new LessonService();
            service.Add(new Lesson(DateTime.Now, "A-101", "Иванов"));
            NUnit.Framework.Assert.That(service.GetAll().Count, Is.EqualTo(1));
        }

        [Test]
        public void RemoveAt_RemovesCorrectly()
        {
            var service = new LessonService();
            service.Add(new Lesson(DateTime.Now, "A-101", "Иванов"));
            service.Add(new Lesson(DateTime.Now, "B-202", "Петров"));

            bool result = service.RemoveAt(0);
            NUnit.Framework.Assert.That(result, Is.True);
            NUnit.Framework.Assert.That(service.GetAll().Count, Is.EqualTo(1));
        }

        [Test]
        public void RemoveAt_InvalidIndex_ReturnsFalse()
        {
            var service = new LessonService();
            bool result = service.RemoveAt(99);
            NUnit.Framework.Assert.That(result, Is.False);
        }
    }

    [TestFixture]
    public class CommandParserTests
    {
        [Test]
        public void AddCommand_ValidData_AddsLesson()
        {
            var service = new LessonService();
            var parser = new CommandParser(service);

            parser.ExecuteCommandsFromString("ADD 2024.09.25;D-505;Кузнецов Д.А.");

            NUnit.Framework.Assert.That(service.GetAll().Count, Is.EqualTo(1));
        }

        [Test]
        public void RemCommand_ByTeacher_RemovesCorrectly()
        {
            var service = new LessonService();
            service.Add(new Lesson(new DateTime(2024, 9, 1), "A-101", "Иванов И.И."));
            service.Add(new Lesson(new DateTime(2024, 9, 2), "B-202", "Петров П.П."));

            var parser = new CommandParser(service);
            parser.ExecuteCommandsFromString("REM teacher = \"Иванов И.И.\"");

            NUnit.Framework.Assert.That(service.GetAll().Count, Is.EqualTo(1));
            NUnit.Framework.Assert.That(service.GetAll()[0].Teacher, Is.EqualTo("Петров П.П."));
        }

        [Test]
        public void RemCommand_ByDateGreater_RemovesCorrectly()
        {
            var service = new LessonService();
            service.Add(new Lesson(new DateTime(2024, 9, 10), "A-101", "Иванов"));
            service.Add(new Lesson(new DateTime(2024, 9, 20), "B-202", "Петров"));

            var parser = new CommandParser(service);
            parser.ExecuteCommandsFromString("REM date > 2024.09.15");

            NUnit.Framework.Assert.That(service.GetAll().Count, Is.EqualTo(1));
            NUnit.Framework.Assert.That(service.GetAll()[0].Date, Is.EqualTo(new DateTime(2024, 9, 10)));
        }

        [Test]
        public void SaveCommand_CreatesFile()
        {
            var service = new LessonService();
            service.Add(new Lesson(new DateTime(2024, 9, 1), "A-101", "Иванов"));

            var parser = new CommandParser(service);
            string tempFile = Path.GetTempFileName();
            parser.ExecuteCommandsFromString($"SAVE {tempFile}");

            NUnit.Framework.Assert.That(File.Exists(tempFile), Is.True);
            File.Delete(tempFile);
        }

        [Test]
        public void UnknownCommand_LogsError()
        {
            var service = new LessonService();
            var parser = new CommandParser(service);
            var log = parser.ExecuteCommandsFromString("UNKNOWN command");

            NUnit.Framework.Assert.That(log.Any(l => l.Contains("✗")), Is.True);
        }
    }
}