using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonScheduleNew
{
    public class LessonService
    {
        private List<Lesson> _lessons = new List<Lesson>();
        public event EventHandler DataChanged;

        public IReadOnlyList<Lesson> GetAll() => _lessons.AsReadOnly();

        public void SetLessons(IEnumerable<Lesson> lessons)
        {
            if (lessons is null) throw new ArgumentNullException(nameof(lessons));
            _lessons = lessons.ToList();
            OnDataChanged();
        }

        public void Add(Lesson lesson)
        {
            if (lesson is null) throw new ArgumentNullException(nameof(lesson));
            _lessons.Add(lesson);
            OnDataChanged();
        }

        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= _lessons.Count)
                return false;

            _lessons.RemoveAt(index);
            OnDataChanged();
            return true;
        }

        protected virtual void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}