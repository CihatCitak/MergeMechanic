using System.IO;
using UnityEngine;

namespace Game.Core.Utilities
{
    public static class JsonFileUtility
    {
        public static void Load(string fileName, object target)
        {
            var path = PathOf(fileName);
            if (File.Exists(path))
                JsonUtility.FromJsonOverwrite(File.ReadAllText(path), target);
        }

        public static void Save(string fileName, object source)
        {
            File.WriteAllText(PathOf(fileName), JsonUtility.ToJson(source));
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
