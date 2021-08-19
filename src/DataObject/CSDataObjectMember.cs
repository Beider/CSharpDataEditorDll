using System;
using System.Reflection;

namespace CSharpDataEditorDll
{
    public class CSDataObjectMember : CSDataObject
    {
        public CSDataObjectMember(object value, DataObjectFactory factory) : base(factory)
        {
            InitialValue = value;
            CurrentValue = value;
        }

        public object InitialValue {get; private set;}

        public object CurrentValue {get; private set;}

        public override object GetAsObject()
        {
            return CurrentValue;
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