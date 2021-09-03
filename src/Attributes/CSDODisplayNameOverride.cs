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
        public readonly string SecondColumnMemberName;
        public CSDODisplayNameOverride(string memberName, string secondColoumMemberName = null)
        {
            MemberName = memberName;
            SecondColumnMemberName = secondColoumMemberName;
        }
    }
}