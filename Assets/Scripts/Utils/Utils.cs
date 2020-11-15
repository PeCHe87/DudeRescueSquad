using UnityEngine;

namespace RandomGames.Utils
{
    public class Utils
    {
        /// <summary>
        /// Given two values (min, max) it returns a value between both
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomNumberBetweenTwoNumbers(int min, int max)
        {
            return new System.Random().Next(min, max);
        }

        /// <summary>
        /// Given two values (min, max) it returns a float value between both
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float GetRandomNumberBetweenTwoNumbers(float min, float max)
        {
            var r = (float)(new System.Random().NextDouble());

            return min + r * (max - min);
        }
    }
}