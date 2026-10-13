using System;
using System.IO;
using UnityEngine;

namespace Game.Core.Utilities
{
    public static class JsonFileUtility
    {
        public static bool Load(string fileName, object target)
        {
            var path = PathOf(fileName);
            if (!File.Exists(path))
                return false;

            try
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(path), target);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[JsonFileUtility] {fileName} could not be read: {exception.Message}");
                return false;
            }
        }

        public static void Save(string fileName, object source)
        {
            var path = PathOf(fileName);
            var tempPath = path + ".tmp";
            File.WriteAllText(tempPath, JsonUtility.ToJson(source));
            if (File.Exists(path))
                File.Replace(tempPath, path, null);
            else
                File.Move(tempPath, path);
        }

        public static void Delete(string fileName)
        {
            File.Delete(PathOf(fileName));
        }

        private static string PathOf(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }
    }
}
