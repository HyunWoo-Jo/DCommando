using UnityEngine;
using System.Collections.Generic;
using System;

namespace Game.Core {
    public static class CSVReader {
        /// <summary>
        /// CSV를 Key-Value Dictionary로 읽기 (첫 번째 열=Key, 두 번째 열=Value)
        /// </summary>
        public static Dictionary<string, string> ReadToDictionary(string fileName) {
            TextAsset csvFile = Resources.Load<TextAsset>(fileName);
            if (csvFile == null) {
                Debug.LogError($"Resources에서 CSV 파일을 찾을 수 없음: {fileName}");
                return new Dictionary<string, string>();
            }
            return ReadToDictionaryFromText(csvFile.text);
        }

        /// <summary>
        /// 텍스트에서 Key-Value Dictionary로 파싱
        /// </summary>
        public static Dictionary<string, string> ReadToDictionaryFromText(string csvText) {
            var dictionary = new Dictionary<string, string>();
            string[] lines = csvText.Split('\n');

            if (lines.Length <= 1) return dictionary;

            // 헤더 행 처리 (검증용)
            string[] headers = SplitCSVLine(lines[0]);

            if (headers.Length < 2) {
                Debug.LogError("CSV 파일에 최소 2개의 열이 필요합니다.");
                return dictionary;
            }

            // 데이터 행 처리
            for (int i = 1; i < lines.Length; i++) {
                string[] values = SplitCSVLine(lines[i]);

                if (values.Length < 2 || string.IsNullOrEmpty(values[0])) continue;

                string key = values[0].Trim();
                string value = values[1].Trim();

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value)) {
                    dictionary[key] = value;
                }
            }

            return dictionary;
        }

        /// <summary>
        /// CSV를 다중 컬럼 Dictionary로 읽기 (첫 번째 열=Key, 나머지 열들=List<string>)
        /// </summary>
        public static Dictionary<string, List<string>> ReadToMultiColumnDictionary(string fileName) {
            TextAsset csvFile = Resources.Load<TextAsset>(fileName);
            if (csvFile == null) {
                Debug.LogError($"Resources에서 CSV 파일을 찾을 수 없음: {fileName}");
                return new Dictionary<string, List<string>>();
            }
            return ReadToMultiColumnDictionaryFromText(csvFile.text);
        }

        /// <summary>
        /// 텍스트에서 다중 컬럼 Dictionary로 파싱 (첫 번째 열=Key, 나머지 열들=List<string>)
        /// </summary>
        public static Dictionary<string, List<string>> ReadToMultiColumnDictionaryFromText(string csvText) {
            var dictionary = new Dictionary<string, List<string>>();
            string[] lines = csvText.Split('\n');

            if (lines.Length <= 1) return dictionary;

            // 헤더 행 처리 (스킵하거나 검증용으로만 사용)
            string[] headers = SplitCSVLine(lines[0]);
            if (headers.Length < 2) {
                Debug.LogError("CSV 파일에 최소 2개의 열이 필요합니다.");
                return dictionary;
            }

            // 데이터 행 처리
            for (int i = 1; i < lines.Length; i++) {
                string[] values = SplitCSVLine(lines[i]);

                if (values.Length == 0 || string.IsNullOrEmpty(values[0])) continue;

                string key = values[0].Trim();
                if (string.IsNullOrEmpty(key)) continue;

                var rowValues = new List<string>();

                // 모든 컬럼 값을 List에 추가 (첫 번째 컬럼도 포함)
                for (int j = 0; j < values.Length; j++) {
                    rowValues.Add(values[j].Trim());
                }

                dictionary[key] = rowValues;
            }

            return dictionary;
        }

        /// <summary>
        /// 일반적인 List<Dictionary> 방식 (기존 호환성)
        /// </summary>
        public static List<Dictionary<string, T>> ReadFromResources<T>(string fileName) {
            TextAsset csvFile = Resources.Load<TextAsset>(fileName);
            if (csvFile == null) {
                Debug.LogError($"Resources에서 CSV 파일을 찾을 수 없음: {fileName}");
                return new List<Dictionary<string, T>>();
            }
            return ReadFromText<T>(csvFile.text);
        }

        public static List<Dictionary<string, T>> ReadFromText<T>(string csvText) {
            var list = new List<Dictionary<string, T>>();
            string[] lines = csvText.Split('\n');

            if (lines.Length <= 1) return list;

            // 헤더 행 처리
            string[] headers = SplitCSVLine(lines[0]);

            // 데이터 행 처리
            for (int i = 1; i < lines.Length; i++) {
                string[] values = SplitCSVLine(lines[i]);

                if (values.Length == 0 || string.IsNullOrEmpty(values[0])) continue;

                var entry = new Dictionary<string, T>();

                for (int j = 0; j < headers.Length && j < values.Length; j++) {
                    string value = values[j].Trim();
                    string key = headers[j].Trim();

                    try {
                        T convertedValue = ParseValue<T>(value);
                        entry[key] = convertedValue;
                    } catch (Exception e) {
                        Debug.LogWarning($"CSV 값 변환 실패: [{key}] = '{value}' to {typeof(T).Name}. 에러: {e.Message}");
                        entry[key] = default;
                    }
                }

                list.Add(entry);
            }

            return list;
        }

        /// <summary>
        /// CSV를 간단한 문자열 리스트로 읽기 (string 타입 특화)
        /// </summary>
        public static List<Dictionary<string, string>> ReadAsStringList(string fileName) {
            return ReadFromResources<string>(fileName);
        }

        /// <summary>
        /// CSV 라인을 분할 (콤마 구분, 따옴표 처리)
        /// </summary>
        private static string[] SplitCSVLine(string line) {
            var result = new List<string>();
            bool inQuotes = false;
            string currentField = "";

            for (int i = 0; i < line.Length; i++) {
                char c = line[i];

                if (c == '"') {
                    inQuotes = !inQuotes;
                } else if (c == ',' && !inQuotes) {
                    result.Add(currentField);
                    currentField = "";
                } else if (c != '\r') // 캐리지 리턴 무시
                  {
                    currentField += c;
                }
            }

            result.Add(currentField);
            return result.ToArray();
        }

        private static T ParseValue<T>(string value) {
            if (string.IsNullOrEmpty(value)) {
                return default;
            }

            Type targetType = typeof(T);

            if (targetType == typeof(object)) {
                return (T)ParseValueAsObject(value);
            }

            try {
                if (targetType == typeof(string)) {
                    return (T)(object)value;
                } else if (targetType == typeof(int)) {
                    return (T)(object)int.Parse(value);
                } else if (targetType == typeof(float)) {
                    return (T)(object)float.Parse(value);
                } else {
                    return (T)Convert.ChangeType(value, targetType);
                }
            } catch (Exception e) {
                Debug.LogError($"타입 변환 실패: '{value}' -> {targetType.Name}. 에러: {e.Message}");
                return default;
            }
        }

        private static object ParseValueAsObject(string value) {
            if (string.IsNullOrEmpty(value)) return "";

            if (int.TryParse(value, out int intValue))
                return intValue;

            if (float.TryParse(value, out float floatValue))
                return floatValue;

            if (bool.TryParse(value, out bool boolValue))
                return boolValue;

            return value;
        }
    }
}