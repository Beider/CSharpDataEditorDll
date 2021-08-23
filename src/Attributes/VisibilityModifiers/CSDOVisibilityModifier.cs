using System;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Allows to alter the visibility of a member
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Class, AllowMultiple = false)]
    public class CSDOVisibilityModifier : CSDOCustomAtrribute
    {
        protected bool SelfVisible;
        protected bool ChildrenVisible;

        public CSDOVisibilityModifier(bool selfVisible = false, bool childrenVisible = false)
        {
            SelfVisible = selfVisible;
            ChildrenVisible = childrenVisible;
        }

        public virtual bool IsSelfVisible(CSDataObject dataObject)
        {
            return SelfVisible;
        }

        public virtual bool AreChildrenVisible(CSDataObject dataObject)
        {
            return ChildrenVisible;
        }
    }
}