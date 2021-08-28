using System;
using System.Reflection;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Allows to alter the visibility of a member
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Class, AllowMultiple = false)]
    public class CSDOVisibilityModifierStatic : CSDOVisibilityModifier
    {
        private readonly Type SelfVisibleClassType;
        private readonly string SelfVisibleMethodName;

        private readonly Type ChildrenVisibleClassType;
        private readonly string ChildrenVisibleMethodName;

        /// <summary>
        /// Expects methods to take in one argument CSDataObject and return a bool Example: public static bool myVisCheckMethod(CSDataObject dataObject)
        /// </summary>
        /// <param name="selfVisibleClassType">Class for self visible check</param>
        /// <param name="selfVisibleMethodName">Name of self visible method</param>
        /// <param name="childrenVisibleClassType">Class for children visible check</param>
        /// <param name="childrenVisibleMethodName">Name of children visible method</param>
        public CSDOVisibilityModifierStatic(Type selfVisibleClassType, string selfVisibleMethodName, 
                                            Type childrenVisibleClassType = null, string childrenVisibleMethodName = null) : base(true, true)
        {
            SelfVisibleClassType = selfVisibleClassType;
            SelfVisibleMethodName = selfVisibleMethodName;
            ChildrenVisibleClassType = childrenVisibleClassType;
            ChildrenVisibleMethodName = childrenVisibleMethodName;
        }

        public override bool IsSelfVisible(CSDataObject dataObject)
        {
            try
            {
                if (SelfVisibleClassType == null)
                {
                    return true;
                }
                MethodInfo methodInfo = GetMethodInfo(dataObject, SelfVisibleClassType, SelfVisibleMethodName);
                if (methodInfo != null)
                {
                    return (bool)methodInfo.Invoke(null, new object[] { dataObject });
                }
            }
            catch (Exception ex)
            {
                System.Console.Error.Write(ex);
            }
            return true;
        }

        public override bool AreChildrenVisible(CSDataObject dataObject)
        {
            try
            {
                if (ChildrenVisibleClassType == null)
                {
                    return true;
                }
                MethodInfo methodInfo = GetMethodInfo(dataObject, ChildrenVisibleClassType, ChildrenVisibleMethodName);
                if (methodInfo != null)
                {
                    return (bool)methodInfo.Invoke(null, new object[] { dataObject });
                }
            }
            catch (Exception ex)
            {
                System.Console.Error.Write(ex);
            }
            return true;
        }

        private MethodInfo GetMethodInfo(CSDataObject dataObject, Type classType, string name)
        {
            Type type = dataObject.Factory.GetAssembly().GetType(classType.FullName);
            if (type != null)
            {
                return type.ResolveMethodInfo(name);
            }
            return null;
        }
    }
}