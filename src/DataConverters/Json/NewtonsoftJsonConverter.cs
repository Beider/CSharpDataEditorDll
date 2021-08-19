using System;
using System.IO;
using System.Globalization;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Expects a folder path to be passed to the Init method before use
    /// </summary>
    public class NewtonsoftJsonConverter : IDataConverter
    {
        private string Folder = "";
        private Type ObjectType = null;

        private DataObjectFactory Factory;

        public void Init(string parameters, Type type, Assembly assembly)
        {
            Folder = parameters;
            ObjectType = type;
            Factory = new DataObjectFactory(typeof(JsonPropertyAttribute), assembly);
            if (!Folder.EndsWith("/") && !Folder.EndsWith("\\"))
            {
                Folder += "/";
            }
        }

        public CSDataObjectClass GetObject(string name)
        {
            string fullPath = $"{Folder}{name}.json";
            
            object createdObject = null;
            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                createdObject = FromJson(json, ObjectType);
            }
            if (createdObject == null)
            {
                createdObject = Activator.CreateInstance(ObjectType);
            }

            return Factory.Create(createdObject);
        }

        public bool SaveObject(string name, CSDataObject dataObject)
        {
            string fullPath = $"{Folder}{name}.json";
            object obj = dataObject.GetAsObject();

            string json = ToJson(obj);
            File.WriteAllText(fullPath, json);
            return true;
        }

        private object FromJson(string json, Type type) => JsonConvert.DeserializeObject(json, type, Settings);

        private string ToJson(object JsonObject) => JsonConvert.SerializeObject(JsonObject, Settings);

        /// <summary>
        /// Settings for JSON serializer
        /// </summary>
        protected static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            Formatting = Formatting.Indented,
            DateParseHandling = DateParseHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}