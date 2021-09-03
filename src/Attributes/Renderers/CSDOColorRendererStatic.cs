using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Simple color renderer that requests the color from a static method
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Class, AllowMultiple = false)]
    public class CSDOColorRendererStatic : CSDORenderer
    {
        private readonly Type ClassType;
        private readonly string GetColorMethodName;
        private readonly string GetBgColorMethodName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType">The class type where static methods are located</param>
        /// <param name="getColorMethodName">Name of method to get color, leave as null for no color. MethodImpl: string myMethod(string value, CSDataObject dataObject)</param>
        /// <param name="getBgColorMethodName">Optional name of method to get bg color, leave as null for no color. MethodImpl: string myMethod(string value, CSDataObject dataObject)</param>
        /// <returns></returns>
        public CSDOColorRendererStatic(Type classType, string getColorMethodName, string getBgColorMethodName = "") : base(true)
        {
            ClassType = classType;
            GetColorMethodName = getColorMethodName;
            GetBgColorMethodName = getBgColorMethodName;
        }

        public override string GetColor(string value, CSDataObject dataObject)
        {
            return GetColorInt(value, dataObject, ClassType, GetColorMethodName);
        }

        public override string GetBgColor(string value, CSDataObject dataObject)
        {
            return GetColorInt(value, dataObject, ClassType, GetBgColorMethodName);
        }
    }
}