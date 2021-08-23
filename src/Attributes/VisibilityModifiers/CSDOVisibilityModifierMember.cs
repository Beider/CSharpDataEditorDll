using System;
using System.Reflection;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Allows to alter the visibility of a member
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Class, AllowMultiple = false)]
    public class CSDOVisibilityModifierMember : CSDOVisibilityModifier
    {
        private readonly string SelfVisibleMemberName;
        private readonly string SelfVisibileValue;

        private readonly string ChildrenVisibleMemberName;
        private readonly string ChildrenVisibileValue;

        public CSDOVisibilityModifierMember(string selfVisibleMemberName, string selfVisibleValue, 
                                            string childrenVisibleMemberName = "", string childrenVisibleValue = null) : base(true, true)
        {
            SelfVisibleMemberName = selfVisibleMemberName;
            SelfVisibileValue = selfVisibleValue;
            ChildrenVisibleMemberName = childrenVisibleMemberName;
            ChildrenVisibileValue = childrenVisibleValue;
        }

        public override bool IsSelfVisible(CSDataObject dataObject)
        {
            // Default to true
            if (SelfVisibleMemberName == null || SelfVisibleMemberName == "")
            {
                return true;
            }
            
            CSDataObjectClass parentClass = (CSDataObjectClass)dataObject.Parent;
            CSDataObjectMember member = (CSDataObjectMember)parentClass.FindMemberByName(SelfVisibleMemberName);
            if (member == null)
            {
                return true;
            }
            if (SelfVisibileValue == null)
            {
                return member.CurrentValue == null;
            }
            return SelfVisibileValue.Equals(member.CurrentValue);
        }

        public override bool AreChildrenVisible(CSDataObject dataObject)
        {
            // Default to true
            if (ChildrenVisibleMemberName == null || ChildrenVisibleMemberName == "")
            {
                return true;
            }
            
            CSDataObjectClass parentClass = (CSDataObjectClass)dataObject.Parent;
            CSDataObjectMember member = (CSDataObjectMember)parentClass.FindMemberByName(ChildrenVisibleMemberName);
            if (member == null)
            {
                return true;
            }
            if (ChildrenVisibileValue == null)
            {
                return member.CurrentValue == null;
            }
            return ChildrenVisibileValue.Equals(member.CurrentValue);
        }
    }
}