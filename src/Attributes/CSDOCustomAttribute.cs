using System;
using System.Reflection;
using System.Collections.Generic;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Inherit from this class to have your own custom attributes stored to the metadata.
    /// </summary>
    public abstract class CSDOCustomAtrribute : Attribute
    {  
        /// <summary>
        /// This is just here to make your life easier when developing attributes
        /// </summary>
        private List<String> ErrorList = new List<string>();

        protected MethodInfo GetMethodInfo(CSDataObject dataObject, Type classType, string name)
        {
            Type type = dataObject.Factory.GetAssembly().GetType(classType.FullName);
            if (type != null)
            {
                return type.ResolveMethodInfo(name);
            }
            return null;
        }

        public List<String> GetErrors()
        {
            return ErrorList;
        }

        protected void ClearErrors()
        {
            ErrorList.Clear();
        }

        protected void LogError(string message)
        {
            ErrorList.Add(message);
        }
    }
}