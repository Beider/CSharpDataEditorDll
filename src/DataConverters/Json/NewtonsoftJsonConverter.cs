using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

#pragma warning disable 0618 // Ignore JsonSchemaGenerator deprication warning
namespace CSharpDataEditorDll
{
    /// <summary>
    /// Expects a folder path to be passed to the Init method before use
    /// </summary>
    public class NewtonsoftJsonConverter : IDataConverter
    {
        private string Error = "";
        private string Folder = "";
        private Type ObjectType = null;
        private bool IsValid = true;

        private DataObjectFactory Factory;

        private JsonSchema Schema;

        public bool Init(string parameters, string typeName, string assemblyPath)
        {
            Folder = parameters;
            Factory = new DataObjectFactory(typeof(JsonPropertyAttribute), assemblyPath);
            if (Factory.GetAssembly() == null)
            {
                Error = Factory.BinaryLoadException.Message;
                return false;
            }
            ObjectType = Factory.GetAssembly().GetType(typeName);
            if (!Folder.EndsWith("/") && !Folder.EndsWith("\\"))
            {
                Folder += "/";
            }

            if (ObjectType != null)
            {
                JsonSchemaGenerator generator = new JsonSchemaGenerator();
                Schema = generator.Generate(ObjectType);
                Schema.Required = false;
            }

            return true;
        }

        public string GetError()
        {
            return Error;
        }


        public string[] GetValidObjectNames()
        {
            List<string> returnList = new List<string>();
            if (Directory.Exists(Folder))
            {
                foreach (string file in Directory.GetFiles(Folder))
                {
                    if (!file.ToLower().EndsWith(".json"))
                    {
                        continue;
                    }
                    string content = File.ReadAllText(file);
                    JObject jObject = JObject.Parse(content);
                    if (Schema != null)
                    {
                        IsValid = true;
                        jObject.Validate(Schema, OnValidationEvent);
                        if (IsValid)
                        {
                            returnList.Add(Path.GetFileNameWithoutExtension(file));
                        }
                    }
                }
            }

            return returnList.ToArray();
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

        private object FromJson(string json) => JsonConvert.DeserializeObject(json, Settings);

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
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };

        public void OnValidationEvent(object sender, ValidationEventArgs e)
        {
            if (e.Message.Contains("Required properties are missing from object:"))
            {
                return;
            }
            IsValid = false;
        }
    }
}