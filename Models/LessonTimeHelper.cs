using System;
using System.Collections.Generic;

namespace UniversitySystem2.Models
{
    public static class LessonTimeHelper
    {
        // Словарь для хранения времени пар по номерам
        private static readonly Dictionary<int, string> LessonTimes = new Dictionary<int, string>
        {
            { 1, "08:00-09:30" },
            { 2, "09:45-11:15" },
            { 3, "11:30-13:00" },
            { 4, "13:30-15:00" },
            { 5, "15:15-16:45" },
            { 6, "17:00-18:30" },
            { 7, "18:45-20:15" }
        };

        // Получить время пары по номеру
        public static string GetLessonTime(int lessonNumber)
        {
            if (LessonTimes.TryGetValue(lessonNumber, out string time))
            {
                return time;
            }
            return "Время не определено";
        }

        // Получить все номера пар для выпадающего списка
        public static Dictionary<int, string> GetAllLessons()
        {
            return LessonTimes;
        }

        // Получить время начала пары (для сортировки)
        public static TimeSpan GetLessonStartTime(int lessonNumber)
        {
            if (LessonTimes.TryGetValue(lessonNumber, out string time))
            {
                string startTime = time.Split('-')[0];
                if (TimeSpan.TryParse(startTime, out TimeSpan result))
                {
                    return result;
                }
            }
            return TimeSpan.Zero;
        }
    }
}
