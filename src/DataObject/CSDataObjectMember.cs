using System;
using System.Reflection;

namespace CSharpDataEditorDll
{
    public class CSDataObjectMember : CSDataObject
    {
        public string InitialValue {get; private set;}

        public string CurrentValue {get; private set;}

        public CSDOValueConverter ValueConverter = null;

        public string CurrentError = null;

        public CSDataObjectMember(object value, DataObjectFactory factory, CSDOValueConverter converter) : base(factory)
        {
            ValueConverter = converter;
            string initialValue = converter.ConvertToString(value);
            InitialValue = initialValue;
            CurrentValue = initialValue;
        }

        public override string GetName()
        {
            return MemberInfo.Name;
        }

        public override object GetAsObject()
        {
            return ValueConverter.ConvertFromString(CurrentValue);
        }

        public void SetValue(string value)
        {
            CurrentValue = value;
            SetModificationState(ModificationStates.EDITED);
            ValidateErrorState();
        }

        /// <summary>
        /// Checks if we changed error state and report accordingly
        /// </summary>
        protected void ValidateErrorState()
        {
            object testConvert = ValueConverter.ConvertFromString(CurrentValue);
            if (ValueConverter.GetError() != CurrentError)
            {
                CurrentError = ValueConverter.GetError();
                ReportErrorState(this, CurrentError != null);
            }
        }

        protected override bool IsModified()
        {
            return CurrentValue != InitialValue;
        }
    }
}