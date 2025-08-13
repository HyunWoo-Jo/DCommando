using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public static class DictConverter
    {
        public static Dictionary<T, string> ToEnumKey<T>(this Dictionary<string, string> dict) where T : struct, Enum{
            var convertedMap = new Dictionary<T, string>();
            foreach (var kvp in dict) {
                if (System.Enum.TryParse<T>(kvp.Key, out var enumName)) {
                    convertedMap[enumName] = kvp.Value;
                }
            }
            return convertedMap;
        } 
    }
}
