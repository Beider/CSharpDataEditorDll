using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Defines this as a list of options, the method has to be a public static method
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CSDOListRendererStatic : CSDOList
    {
        private const string COULD_NOT_BE_RESOLVED = "- Method could not be resolved -";
        private readonly Type ClassType;
        private readonly string GetListMethodName;
        private readonly string GetColorMethodName;

        /// <summary>
        /// Will call static methods to get values, please consider buffering in static methods
        /// </summary>
        /// <param name="classType">The class type where static methods are located</param>
        /// <param name="getListMethod">Name of method to get list. MethodImpl: string[] myMethod(CSDataObject dataObject)</param>
        /// <param name="getColorMethod">Optional name of method to get color, leave as null for no color. MethodImpl: string myMethod(string value, CSDataObject dataObject)</param>
        /// <param name="sortList">Should we sort this list, default true</param>
        /// <returns></returns>
        public CSDOListRendererStatic(Type classType, string getListMethod, string getColorMethod = null, bool sortList = true) : base(getColorMethod != null, sortList)
        {
            ClassType = classType;
            GetColorMethodName = getColorMethod;
            GetListMethodName = getListMethod;
        }

        public override string[] GetList(CSDataObject dataObject)
        {
            try
            {
                MethodInfo methodInfo = GetMethodInfo(dataObject, GetListMethodName);
                if (methodInfo == null)
                {
                    List<string> tmpDict = new List<string>();
                    tmpDict.Add(COULD_NOT_BE_RESOLVED);
                    return tmpDict.ToArray();
                }
                string[] array = (string[])methodInfo.Invoke(null, new object[] { dataObject });
                if (SortList)
                {
                    array = Sort(array);
                }
                return array;
            }
            catch (Exception ex)
            {
                System.Console.Error.Write(ex);
                return new string[] { ex.Message };
            }
        }

        public override string GetColor(string value, CSDataObject dataObject)
        {
            if (COULD_NOT_BE_RESOLVED.Equals(value))
            {
                return "Red";
            }

            if (!UseColors)
            {
                return null;
            }

            try
            {
                MethodInfo methodInfo = GetMethodInfo(dataObject, GetColorMethodName);
                if (methodInfo == null)
                {
                    return "Red";
                }
                return (string)methodInfo.Invoke(null, new object[] { value, dataObject });
            }
            catch (Exception ex)
            {
                System.Console.Error.Write(ex);
                return "Red";
            }
        }

        private MethodInfo GetMethodInfo(CSDataObject dataObject, string name)
        {
            Type type = dataObject.Factory.GetAssembly().GetType(ClassType.FullName);
            if (type != null)
            {
                return type.ResolveMethodInfo(name);
            }
            return null;
        }

    }
}