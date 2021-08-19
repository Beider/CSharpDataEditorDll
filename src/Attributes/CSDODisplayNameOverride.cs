using System;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Allows you to override the display name with the value of a member
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CSDODisplayNameOverride : CSDOCustomAtrribute
    {
        public readonly string MemberName;
        public CSDODisplayNameOverride(string memberName)
        {
            MemberName = memberName;
        }
    }
}