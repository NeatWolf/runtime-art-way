using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace RuntimeArtWay.Storage
{
    public interface IStorage<T>
    {
        T Value { get; set; }

        void Save();
        void Load();
    }

    public class EditorPrefsStorage<T> : IStorage<T> where T : UnityEngine.Object
    {
        private readonly string key;
        private string oldValue = "";

        public EditorPrefsStorage(string key)
        {
            this.key = key;
        }

        public T Value { get; set; }

        public void Save()
        {
            EditorPrefs.DeleteKey(key);

            if (Value is null) return;
            if (!EditorUtility.IsPersistent(Value)) return;

            var newValue = AssetDatabase.GetAssetPath(Value);

            if (newValue == oldValue) return;

            oldValue = newValue;
            EditorPrefs.SetString(key, newValue);
        }

        public void Load()
        {
            Value = null;

            var path = EditorPrefs.GetString(key);
            oldValue = path;

            if (string.IsNullOrEmpty(path)) return;

            Value = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
        }
    }

    public class EditorPrefsListStorage<T> : IStorage<List<T>> where T : UnityEngine.Object
    {
        private const string SEPARATOR = "|";
        private readonly string key;

        public EditorPrefsListStorage(string key)
        {
            this.key = key;
        }

        public List<T> Value { get; set; }

        public void Save()
        {
            EditorPrefs.DeleteKey(key);

            if (!Value.Any()) return;

            var paths = Value
                .Where(EditorUtility.IsPersistent)
                .Select(AssetDatabase.GetAssetPath);

            string toSave = string.Join(SEPARATOR, paths);
            EditorPrefs.SetString(key, toSave);
        }

        public void Load()
        {
            if (!EditorPrefs.HasKey(key)) return;

            Value = EditorPrefs.GetString(key)
                .Split('|')
                .Select(x => AssetDatabase.LoadAssetAtPath(x, typeof(T)))
                .Cast<T>()
                .ToList();
        }
    }
}