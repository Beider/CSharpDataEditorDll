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
        private MethodInfo SelfVisibleMethodInfo = null;

        private readonly Type ChildrenVisibleClassType;
        private readonly string ChildrenVisibleMethodName;
        private MethodInfo ChildrenVisibleMethodInfo = null;

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
            if (SelfVisibleMethodInfo == null)
            {
                SelfVisibleMethodInfo = SelfVisibleClassType.ResolveMethodInfo(SelfVisibleMethodName);
            }
            if (SelfVisibleMethodInfo != null)
            {
                return (bool)SelfVisibleMethodInfo.Invoke(null, new object[] { dataObject });
            }
            return true;
        }

        public override bool AreChildrenVisible(CSDataObject dataObject)
        {
            if (ChildrenVisibleMethodInfo == null)
            {
                ChildrenVisibleMethodInfo = ChildrenVisibleClassType.ResolveMethodInfo(ChildrenVisibleMethodName);
            }
            if (ChildrenVisibleMethodInfo != null)
            {
                return (bool)ChildrenVisibleMethodInfo.Invoke(null, new object[] { dataObject });
            }
            return true;
        }
    }
}