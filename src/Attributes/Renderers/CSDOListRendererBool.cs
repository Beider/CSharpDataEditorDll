using System;
using System.Collections.Generic;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Base class for all list renderers
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CSDOListRendererBool : CSDOList
    {
        private const string TRUE = "True";
        private const string FALSE = "False";
        private static string[] BoolList = null;

        public CSDOListRendererBool(bool useColors = true) : base(useColors, false)
        {
            // Do nothing
        }

        public override string[] GetList(CSDataObject dataObject)
        {
            if (BoolList == null)
            {
                List<string> tmplist = new List<string>();
                tmplist.Add(TRUE);
                tmplist.Add(FALSE);
                BoolList = tmplist.ToArray();
            }
            return BoolList;
        }

        public override string GetColor(string value, CSDataObject dataObject)
        {
            if (!UseColors)
            {
                return null;
            }
            if (TRUE.EqualsIgnoreCase(value))
            {
                return "Green";
            }
            
            return "Red";
        }

    }
}