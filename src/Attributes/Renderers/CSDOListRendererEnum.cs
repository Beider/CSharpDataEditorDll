using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Will make a list based on an enum, will by default attempt to resolve it from the binary this attribute was from
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public abstract class CSDOListRendererEnum : CSDOList
    {
        private Type EnumType;

        private readonly string Color;

        public CSDOListRendererEnum(Type enumTypeName, string color = null, bool sortList = true) : base(true, sortList)
        {
            EnumType = enumTypeName;
            Color = color;
        }

        private Type LoadEnumFromAssembly(CSDataObject dataObject)
        {
            Type assemblyEnumType = dataObject.Factory.GetAssembly().GetType(EnumType.FullName);
            if (assemblyEnumType == null || !assemblyEnumType.IsEnum)
            {
                return EnumType;
            }

            return assemblyEnumType;
        }

        public override string[] GetList(CSDataObject dataObject)
        {
            Type enumType = LoadEnumFromAssembly(dataObject);
            List<string> retList = new List<string>();
            foreach (string name in Enum.GetNames(enumType))
            {
                retList.Add(name);
            }
            if (retList.Count == 0)
            {
                retList.Add($"- Enum {enumType.Name} was empty -");
            }

            if (SortList)
            {
                retList = Sort(retList);
            }
            return retList.ToArray();
        }

        public override string GetColor(string value, CSDataObject dataObject)
        {
            return Color;
        }

    }
}