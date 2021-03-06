using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Ieedo.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ieedo
{
    public interface IDefinition
    {
        int Id { get; }
    }

    public class DataManager
    {
        public DataManager()
        {
            LoadDatabase();
            LoadCardDefinitions();
        }

        #region Database

        private Dictionary<string, List<Object>> db = new();

        public void LoadDatabase()
        {
            LoadAll<CategoryDefinition>();
            LoadAll<ActivityDefinition>();
            LoadAll<AssessmentQuestionDefinition>();
        }

        public T Get<T>(int id) where T : class, IDefinition
        {
            var all = GetAll<T>();
            return all.FirstOrDefault(x => x.Id == id);
        }

        public List<T> GetAll<T>() where T : class
        {
            return db[typeof(T).Name].ConvertAll(x => x as T);
        }
        public void PrintAll<T>() where T : Object
        {
            var s = new StringBuilder();
            s.Append($"{typeof(T).Name}:");
            s.AppendLine(GetAll<T>().ToJoinedString());
            Log.Info(s.ToString());
        }

        private void LoadAll<T>() where T : Object
        {
            var objs = Resources.LoadAll<T>(typeof(T).Name);
            db[typeof(T).Name] = new List<Object>();
            db[typeof(T).Name].AddRange(objs);
        }

        #region Cards

        public List<CardDefinition> CardDefinitions => cards.Cards.ToList();

        private CardsCollection cards = new();
        public void LoadCardDefinitions()
        {
            LoadSerialized(out cards, Application.persistentDataPath, "cards");
        }

        public void InitialiseCardDefinitions()
        {
            LoadSerialized(out cards, Application.streamingAssetsPath, "cards");
            if (cards == null) cards = new();
            SaveCardDefinitions(saveAsDefaultCards:false);
        }

        public void AddCardDefinition(CardDefinition def, bool isDefaultCard = false)
        {
            cards.Add(def);
            SaveCardDefinitions(isDefaultCard);
        }

        public void DeleteAllCardDefinitions()
        {
            cards.Cards.Clear();
            SaveCardDefinitions(saveAsDefaultCards:false);
        }

        public void SaveCardDefinitions(bool saveAsDefaultCards = false)
        {
            if (saveAsDefaultCards)
                SaveSerialized(cards, Application.streamingAssetsPath, "cards");
            else
                SaveSerialized(cards, Application.persistentDataPath, "cards");
        }

        #endregion

        #endregion


        #region Profile

        private ProfileData ProfileData;
        public ProfileData Profile => ProfileData;
        private static string profileName = "default";

        public void CreateNewProfile(ProfileDescription description)
        {
            ProfileData = new ProfileData {
                Description = description,
                OnboardingState = new OnboardingState(),
                Level = 0,
                Cards = new CardDataCollection(),
                Categories = new CategoryDataCollection(),
                ActivitiesData = new ActivitiesData(),
            };

            foreach (var def in GetAll<CategoryDefinition>())
            {
                ProfileData.Categories.Add(new() {
                    ID = def.ID,
                    AssessmentValue = 0f
                });
            }

            SaveProfile();
        }

        public void SaveProfile()
        {
            SaveSerialized(ProfileData, Application.persistentDataPath, $"profile_{profileName}");
        }

        public bool LoadProfile()
        {
            return LoadSerialized(out ProfileData, Application.persistentDataPath, $"profile_{profileName}");
        }

        private bool SaveSerialized<T>(T data, string folderPath, string filename)
        {
            string extension = "dat";
            if (Statics.App.ApplicationConfig.DebugSaveProfileInJSON)
            {
                extension = "json";
            }
            string path = $"{folderPath}/{filename}.{extension}";

            try
            {
                var settings = new JsonSerializerSettings();
                var json = JsonConvert.SerializeObject(data, settings);
                var bytes = Encoding.UTF8.GetBytes(json);
                var bf = new BinaryFormatter();
                using (FileStream stream = File.Create(path))
                {
                    if (Statics.App.ApplicationConfig.DebugSaveProfileInJSON)
                    {
                        stream.Write(bytes);
                    } else
                    {
                        bf.Serialize(stream, bytes);
                    }
                    return true;
                }
            } catch (Exception e)
            {
                Debug.LogError($"Could not save data at path {path}\nException {e.Message}");
                return false;
            }
        }

        private bool LoadSerialized<T>(out T data, string folderPath, string key)
        {
            string extension = "dat";
            if (Statics.App.ApplicationConfig.DebugSaveProfileInJSON)
            {
                extension = "json";
            }
            string path = $"{folderPath}/{key}.{extension}";
            if (!File.Exists(path))
            {
                Debug.LogError($"No file could be found at path {path}");
                data = default;
                return false;
            }

            try
            {
                if (Statics.App.ApplicationConfig.DebugSaveProfileInJSON)
                {
                    using (StreamReader r = new StreamReader(path))
                    {
                        var jsonString = r.ReadToEnd();
                        var jobj = JObject.Parse(jsonString);
                        var serializer = JsonSerializer.Create();
                        data = jobj.ToObject<T>(serializer);
                        return true;
                    }
                } else
                {
                    using (var stream = File.OpenRead(path))
                    {
                        var bf = new BinaryFormatter();
                        var bytes = bf.Deserialize(stream) as byte[];
                        var jsonString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        var jobj = JObject.Parse(jsonString);
                        var serializer = JsonSerializer.Create();
                        data = jobj.ToObject<T>(serializer);
                        return true;
                    }
                }

            } catch (Exception e)
            {
                Debug.LogError($"Could not load data at path {path}\nException {e.Message}");
                data = default;
                return false;
            }
        }

        #endregion


        public CategoryDefinition Definition(CategoryID id)
        {
            return Get<CategoryDefinition>((int)id);
        }
    }
}