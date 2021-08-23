using System;
using System.Reflection;
using System.Collections.Generic;

namespace CSharpDataEditorDll
{
    public abstract class CSDataObject
    {
        public enum ModificationStates
        {
            NONE, NEW, EDITED, DELETED
        }

        public CSDataObject(DataObjectFactory factory)
        {
            Factory = factory;
        }

        /// <summary>
        /// The factory that created this data object
        /// </summary>
        public DataObjectFactory Factory;

        /// <summary>
        /// The member info this MetaData is for
        /// </summary>
        public MemberInfo MemberInfo;

        /// <summary>
        /// Our dear parent
        /// </summary>
        public CSDataObject Parent;

        /// <summary>
        /// Will be set if we are part of an array
        /// </summary>
        public int Index = -1;

        /// <summary>
        /// Has this item been modified or not?
        /// </summary>
        public ModificationStates ModificationState {get; private set;} = ModificationStates.NONE;

        /// <summary>
        /// Contains all custom attributes
        /// </summary>
        public List<CSDOCustomAtrribute> CustomAttributes = new List<CSDOCustomAtrribute>();

        /// <summary>
        /// Can be used to store whatever you need for the editor
        /// </summary>
        protected Dictionary<string, object> Metadata = new Dictionary<string, object>();

        public List<CSDataObject> InvalidObjects = new List<CSDataObject>();

        /// <summary>
        /// Convert this back into an object
        /// </summary>
        public abstract object GetAsObject();

        protected virtual bool IsModified()
        {
            return false;
        }

        /// <summary>
        /// Unique key identifier of this item
        /// </summary>
        public string GetKey()
        {
            if (GetParentKey() == ".")
            {
                // We are at the root, don't add root
                return "";
            }
            return $"{GetParentKey()}{GetName()}{GetIndex()}/";
        }

        /// <summary>
        /// Get any object by their unique key, should be called on the root object
        /// </summary>
        /// <param name="key">The key obtained by GetKey()</param>
        public CSDataObject GetObjectByKey(string key)
        {
            if (key == "")
            {
                return this;
            }

            int firstSlash = key.IndexOf('/');
            string currentKey = key.Substring(0, firstSlash);
            string nextKey = key.Substring(firstSlash+1);
            int index = -1;
            if (currentKey.Contains("#"))
            {
                string[] splitKey = currentKey.Split('#');
                currentKey = splitKey[0];
                index = int.Parse(splitKey[1]);
            }

            if (this is CSDataObjectClass)
            {
                CSDataObjectClass dataObjectClass = (CSDataObjectClass)this;
                foreach (CSDataObject obj in dataObjectClass.ClassMembers)
                {
                    if (obj.GetName().Equals(currentKey))
                    {
                        return obj.GetObjectByKey(nextKey);
                    }
                }
            }
            else if (this is CSDataObjectMemberArray)
            {
                CSDataObjectMemberArray dataObjectMemberArray = (CSDataObjectMemberArray)this;
                return dataObjectMemberArray.Get(index).GetObjectByKey(nextKey);
            }

            return null;
        }

        public abstract string GetName();

        /// <summary>
        /// Returns all CSDataObjects in the tree that has any attribute that inherits from T
        /// </summary>
        /// <typeparam name="T">The attribute you are looking for</typeparam>
        public virtual List<CSDataObject> GetAllWithCustomAttribute<T>() where T : CSDOCustomAtrribute
        {
            List<CSDataObject> returnList = new List<CSDataObject>();

            if (GetCustomAttribute<T>() != null)
            {
                returnList.Add(this);
            }

            return returnList;
        }

        protected string GetParentKey()
        {
            if (Parent != null)
            {
                return Parent.GetKey();
            }
            else
            {
                return ".";
            }
        }

        protected string GetIndex()
        {
            if (Index >= 0)
            {
                return $"#{Index}";
            }
            return "";
        }

        public void SetModificationState(ModificationStates state)
        {
            // Do not change new items to edited
            if (ModificationState == ModificationStates.NEW && state == ModificationStates.EDITED)
            {
                return;
            }

            if (state == ModificationStates.NONE && IsModified())
            {
                ModificationState = ModificationStates.EDITED;
                return;
            }

            ModificationState = state;
        }

        /// <summary>
        /// Get a single custom attribute of the given type
        /// </summary>
        /// <typeparam name="T">The type of attribute you want</typeparam>
        /// <returns>The attribute or null if not found</returns>
        public T GetCustomAttribute<T>() where T : CSDOCustomAtrribute
        {
            foreach (CSDOCustomAtrribute attribute in CustomAttributes)
            {
                if (typeof(T).IsAssignableFrom(attribute.GetType()))
                {
                    return (T)attribute;
                }
            }
            return null;
        }

        /// <summary>
        /// Get a all custom attribute of the given type
        /// </summary>
        /// <typeparam name="T">The type of attribute you want</typeparam>
        /// <returns>List of attributes of that type, could be empty</returns>
        public List<T> GetCustomAttributes<T>() where T : CSDOCustomAtrribute
        {
            List<T> attributeList = new List<T>();
            foreach (CSDOCustomAtrribute attribute in CustomAttributes)
            {
                if (typeof(T).IsAssignableFrom(attribute.GetType()))
                {
                    attributeList.Add((T)attribute);
                }
            }
            return attributeList;
        }

        /// <summary>
        /// Get the value of the metadata or the default value
        /// </summary>
        public T GetMetadata<T>(string key, T defaultValue)
        {
            if (Metadata.ContainsKey(key))
            {
                return (T)Metadata[key];
            }
            return defaultValue;
        }

        /// <summary>
        /// Get the value of the metadata or null
        /// </summary>
        public object GetMetadata(string key)
        {
            if (Metadata.ContainsKey(key))
            {
                return Metadata[key];
            }
            return null;
        }

        public void SetMetadata(string key, object value)
        {
            if (!Metadata.ContainsKey(key))
            {
                Metadata.Add(key, value);
            }
            else
            {
                Metadata[key] = value;
            }
        }

        /// <summary>
        /// Reports any objects in error state to root
        /// </summary>
        /// <param name="dataObject">The object that has an error state change</param>
        /// <param name="hasError">True if it has an error, false if error has been resolved</param>
        protected void ReportErrorState(CSDataObject dataObject, bool hasError)
        {
            if (Parent != null)
            {
                Parent.ReportErrorState(dataObject, hasError);
                return;
            }

            if (hasError && !InvalidObjects.Contains(dataObject))
            {
                InvalidObjects.Add(dataObject);
            }
            else if (!hasError && InvalidObjects.Contains(dataObject))
            {
                InvalidObjects.Remove(dataObject);
            }
        }

        /// <summary>
        /// Convenience implementation so you don't need to cast to Array or Class to get children
        /// </summary>
        public virtual List<CSDataObject> GetChildren()
        {
            return new List<CSDataObject>();
        }
    }
}