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
    public abstract class CSDOListRendererStatic : CSDOList
    {
        private const string COULD_NOT_BE_RESOLVED = "- Method could not be resolved -";
        private readonly Type ClassType;
        private readonly string GetListMethodName;
        private readonly string GetColorMethodName;
        private MethodInfo MethodInfoList = null;
        private MethodInfo MethodInfoColor = null;

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
            if (MethodInfoList == null)
            {
                MethodInfoList = ClassType.ResolveMethodInfo(GetListMethodName);
            }
            if (MethodInfoList == null)
            {
                List<string> tmpDict = new List<string>();
                tmpDict.Add(COULD_NOT_BE_RESOLVED);
                return tmpDict.ToArray();
            }
            string[] array = (string[])MethodInfoList.Invoke(null, new object[] { dataObject });
            if (SortList)
            {
                array = Sort(array);
            }
            return array;
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

            if (MethodInfoColor == null)
            {
                MethodInfoColor = ClassType.ResolveMethodInfo(GetColorMethodName);
            }

            return (string)MethodInfoColor.Invoke(null, new object[] { value, dataObject });
        }

    }
}