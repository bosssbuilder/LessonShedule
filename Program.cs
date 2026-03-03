namespace LessonSchedule
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string test_data = "Lesson 2024-03-05 A101 Ivanov Ivan Ivanovich";

            Logic logic = new Logic();

            logic.PrintLesson(logic.ToLesson(test_data));
        }
    }
}