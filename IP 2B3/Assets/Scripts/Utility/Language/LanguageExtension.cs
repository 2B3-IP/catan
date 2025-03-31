using System.Collections.Generic;
using UnityEngine;

namespace Utility.Language
{
    public static class LanguageExtension
    {
        private static readonly Dictionary<float, WaitForSeconds> waitMap = new();

        public static WaitForSeconds ToWait(this float seconds)
        {
            if (waitMap.TryGetValue(seconds, out var forSeconds))
                return forSeconds;
            
            var wait = new WaitForSeconds(seconds);
            waitMap.Add(seconds, wait);

            return wait;
        }
    }
}