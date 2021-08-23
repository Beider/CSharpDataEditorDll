using System;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// All data classes should be tagged with this property
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CSDODataClass : CSDOCustomAtrribute
    {
        public CSDODataClass()
        {
            
        }
    }
}