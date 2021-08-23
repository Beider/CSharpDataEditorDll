using System;
using System.Reflection;
using System.Collections.Generic;

namespace CSharpDataEditorDll
{
    public class CSDataObjectClass : CSDataObject
    {
        /// <summary>
        /// The type of this class
        /// </summary>
        public Type ClassType {get; set;}

        /// <summary>
        /// List of all members in this class
        /// </summary>
        public List<CSDataObject> ClassMembers = new List<CSDataObject>();

        public CSDataObjectClass(DataObjectFactory factory) : base(factory)
        {
            
        }

        public override string GetName()
        {
            return ClassType.Name;
        }


        public void InitializeEmptyClass()
        {
            object value = Activator.CreateInstance(ClassType);
            CSDataObjectClass tmpClass = Factory.CreateDataObjectClass(value, ClassType, null);
            ClassMembers = tmpClass.ClassMembers;
            SetModificationState(ModificationStates.NEW);
        }

        public override object GetAsObject()
        {
            if (ClassMembers.Count == 0)
            {
                // We got no data
                return null;
            }

            object value = Activator.CreateInstance(ClassType);

            foreach (CSDataObject obj in ClassMembers)
            {
                obj.MemberInfo.SetValue(value, obj.GetAsObject());
            }

            return value;
        }

        public CSDataObject FindMemberByName(string name)
        {
            foreach (CSDataObject dataObject in ClassMembers)
            {
                if (dataObject.MemberInfo.Name.Equals(name))
                {
                    return dataObject;
                }
            }
            return null;
        }

        public override List<CSDataObject> GetAllWithCustomAttribute<T>()
        {
            List<CSDataObject> returnList = base.GetAllWithCustomAttribute<T>();

            foreach (CSDataObject dataObject in ClassMembers)
            {
                returnList.AddRange(dataObject.GetAllWithCustomAttribute<T>());
            }

            return returnList;
        }

        public override List<CSDataObject> GetChildren()
        {
            return ClassMembers;
        }
    }
}