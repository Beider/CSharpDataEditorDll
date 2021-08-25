using System;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Makes the array / list start collapsed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Class, AllowMultiple = false)]
    public class CSDODescription : CSDOCustomAtrribute
    {
        public readonly string Description;

        public CSDODescription(string description)
        {
            Description = description;
        }
    }
}