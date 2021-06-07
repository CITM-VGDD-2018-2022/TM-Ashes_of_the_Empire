using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Collections.Generic;

namespace DiamondEngine
{


    public static class DiamondPrefs
    {

        public static Dictionary<string, string> data = new Dictionary<string, string>();

        public static void SaveData()
        {
            if (!Directory.Exists("SaveData"))
                Directory.CreateDirectory("SaveData");

            if (!File.Exists("SaveData/saveData.json"))
                File.Create("SaveData/saveData.json");

            string jsonString = JsonSerializer.Serialize(data);
            File.WriteAllText("SaveData/saveData.json", jsonString);
        }

        public static void LoadData()
        {
            data.Clear();
            if (!File.Exists("SaveData/saveData.json"))
                return;
            string jsonString = File.ReadAllText("SaveData/saveData.json");
            if (jsonString.Length == 0) 
                return;
            data = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
        }

        public static void Write(string key, object value)
        {
            if (typeof(float) != value.GetType() && typeof(int) != value.GetType() && typeof(bool) != value.GetType() && typeof(string) != value.GetType())
                return;
            data[key] = value.ToString();
        }

        public static float ReadFloat(string key)
        {
            if (!data.ContainsKey(key))
                return 0.0f;
            return float.Parse(data[key]);
        }

        public static int ReadInt(string key)
        {
            if (!data.ContainsKey(key))
                return 0;
            return int.Parse(data[key]);
        }

        public static bool ReadBool(string key)
        {
            if (!data.ContainsKey(key))
                return false;
            return bool.Parse(data[key]);
        }

        public static string ReadString(string key)
        {
            if (!data.ContainsKey(key))
                return "";
            return data[key];
        }

        public static void Delete(string key)
        {
            if (!data.ContainsKey(key))
                return;
            data.Remove(key);
        }

        public static void Clear()
        {
            data.Clear();
        }

    }


}