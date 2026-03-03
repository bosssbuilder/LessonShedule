using System;
using System.Collections.Generic;

namespace LessonSchedule
{
    internal class Program
    {
        static void Main(string[] args)
        {
<<<<<<< HEAD
=======
            string test_data = "Lesson 2024-03-05 A101 Ivanov Ivan Ivanovich Red";

>>>>>>> color
            Logic logic = new Logic();
            List<Lesson> lessons = new List<Lesson>();

            while (true)
            {
                Console.Clear();
                Console.WriteLine(" МЕНЮ УПРАВЛЕНИЯ РАСПИСАНИЕМ ");
                Console.WriteLine("1. Добавить занятие");
                Console.WriteLine("2. Показать все занятия");
                Console.WriteLine("3. Выход");
                Console.Write("Выберите пункт: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddLesson(logic, lessons);
                        break;

                    case "2":
                        ShowAllLessons(logic, lessons);
                        break;

                    case "3":
                        Console.WriteLine();
                        return;

                    default:
                        Console.WriteLine("Неверный выбор! Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void AddLesson(Logic logic, List<Lesson> lessons)
        {
            Console.Clear();
            Console.WriteLine(" ДОБАВЛЕНИЕ НОВОГО ЗАНЯТИЯ ");

            Console.Write("Введите дату (ГГГГ-ММ-ДД): ");
            string dateStr = Console.ReadLine();

            Console.Write("Введите аудиторию: ");
            string audience = Console.ReadLine();

            Console.Write("Введите ФИО преподавателя: ");
            string teacher = Console.ReadLine();

            Console.Write("Введите цвет (Red, Blue, Green и т.д.): ");
            string color = Console.ReadLine();

            string lessonStr = $"Lesson {dateStr} {audience} {teacher} {color}";

            try
            {
                Lesson lesson = logic.ToLesson(lessonStr);
                lessons.Add(lesson);
                Console.WriteLine("\n Занятие успешно добавлено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Ошибка при добавлении: {ex.Message}");
            }

            Console.WriteLine();
            Console.ReadKey();
        }

        static void ShowAllLessons(Logic logic, List<Lesson> lessons)
        {
            Console.Clear();
            Console.WriteLine(" СПИСОК ВСЕХ ЗАНЯТИЙ \n");

            if (lessons.Count == 0)
            {
                Console.WriteLine("Занятий пока нет.");
            }
            else
            {
                for (int i = 0; i < lessons.Count; i++)
                {
                    Console.Write($"{i + 1}. ");
                    logic.PrintLesson(lessons[i]);
                }
            }

            Console.WriteLine($"\nВсего занятий: {lessons.Count}");
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}