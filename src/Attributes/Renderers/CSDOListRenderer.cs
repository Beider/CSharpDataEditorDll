using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Base class for all list renderers
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public abstract class CSDOList : CSDORenderer
    {
        public const string LIST_RENDERER_TYPE = "LIST";

        protected readonly bool SortList;

        public CSDOList(bool useColors, bool sortList) : base(useColors)
        {
            SortList = sortList;
        }

        /// <summary>
        /// Should return a list representing the possible values for the given Data Object
        /// </summary>
        /// <param name="dataObject">The data object we are requesting the list for</param>
        public abstract string[] GetList(CSDataObject dataObject);

        protected string[] Sort(string[] array)
        {
            return array.OrderBy(x => x).ToArray();
        }

        protected List<string> Sort(List<string> list)
        {
            return list.OrderBy(x => x).ToList();
        }
        
        public override string GetRenderType()
        {
            return LIST_RENDERER_TYPE;
        }
    }
}