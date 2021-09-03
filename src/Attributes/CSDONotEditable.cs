using System;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Makes the value uneditable
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Class, AllowMultiple = false)]
    public class CSDONotEditable : CSDOCustomAtrribute
    {
        
    }
}