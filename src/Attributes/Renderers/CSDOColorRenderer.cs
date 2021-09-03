using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Simple color renderer to add a single color regardless of value
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Class, AllowMultiple = false)]
    public class CSDOColorRenderer : CSDORenderer
    {
        protected readonly string Color;
        protected readonly string BgColor;

        public CSDOColorRenderer(string color, string bgColor = null) : base(true)
        {
            Color = color;
            BgColor = bgColor;
        }

        public override string GetColor(string value, CSDataObject dataObject)
        {
            return Color;
        }

        public override string GetBgColor(string value, CSDataObject dataObject)
        {
            return BgColor;
        }
    }
}