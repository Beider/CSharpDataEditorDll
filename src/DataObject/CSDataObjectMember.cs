using System;
using System.Reflection;

namespace CSharpDataEditorDll
{
    public class CSDataObjectMember : CSDataObject
    {
        public object InitialValue {get; private set;}

        public object CurrentValue {get; private set;}

        public CSDataObjectMember(object value, DataObjectFactory factory) : base(factory)
        {
            InitialValue = value;
            CurrentValue = value;
        }

        public override string GetName()
        {
            return MemberInfo.Name;
        }

        public override object GetAsObject()
        {
            return CurrentValue;
        }

        public string GetCurrentValueText()
        {
            if (CurrentValue != null)
            {
                return CurrentValue.ToString();
            }
            return "";
        }

        public void SetValue(object value)
        {
            CurrentValue = value;
            SetModificationState(ModificationStates.EDITED);
        }

        protected override bool IsModified()
        {
            return CurrentValue != InitialValue;
        }
    }
}