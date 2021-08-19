using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace CSharpDataEditorDll
{
    public class CSDataObjectMemberArray : CSDataObject
    {
        private int _NextIndex = -1;

        public int NextIndex
        {
            get
            {
                _NextIndex++;
                return _NextIndex;
            }
        }

        /// <summary>
        /// Values for this array
        /// </summary>
        private Dictionary<int, CSDataObject> Values = new Dictionary<int, CSDataObject>();

        private Dictionary<int, CSDataObject> DeletedValues = new Dictionary<int, CSDataObject>();

        public CSDataObjectMemberArray(DataObjectFactory factory) : base(factory)
        {
            
        }

        public override string GetName()
        {
            return MemberInfo.Name;
        }

        /// <summary>
        /// Add a new element to this array
        /// </summary>
        /// <returns>The new CSDataObject</returns>
        public CSDataObject AddNew()
        {
            Type elementType = MemberInfo.GetUnderlyingType().GetArrayOrListUnderlyingType();
            object val = Activator.CreateInstance(elementType);
            CSDataObject value = Factory.CreateDataObject(null, val, elementType, null, this);
            value.SetModificationState(ModificationStates.NEW);
            Add(value);
            return value;
        }

        /// <summary>
        /// Add a new element to this array
        /// </summary>
        /// <param name="value">The CSDataObject to add</param>
        public void Add(CSDataObject value)
        {
            int index = NextIndex;
            value.Index = index;
            Values.Add(index, value);
        }

        /// <summary>
        /// Remove an element from this array
        /// </summary>
        /// <param name="index">The index to remove</param>
        public void Remove(int index)
        {
            if (Values.ContainsKey(index))
            {
                DeletedValues.Add(index, Values[index]);
                Values[index].SetModificationState(ModificationStates.DELETED);
                Values.Remove(index);
            }
        }

        /// <summary>
        /// Undoes a remove
        /// </summary>
        /// <param name="index">The index to un remove</param>
        public void UnRemove(int index)
        {
            if (DeletedValues.ContainsKey(index))
            {
                Values.Add(index, DeletedValues[index]);
                Values[index].SetModificationState(ModificationStates.NONE);
                DeletedValues.Remove(index);
            }
        }

        /// <summary>
        /// Get the element at the given index
        /// </summary>
        /// <param name="index">The index to get</param>
        public CSDataObject Get(int index)
        {
            if (Values.ContainsKey(index))
            {
                return Values[index];
            }
            if (DeletedValues.ContainsKey(index))
            {
                return DeletedValues[index];
            }
            return null;
        }

        /// <summary>
        /// Get the indexses of this array
        /// </summary>
        /// <returns></returns>
        public List<int> GetUsedIndexes()
        {
            return new List<int>(Values.Keys);
        }

        /// <summary>
        /// Get the deleted indexes of this array
        /// </summary>
        /// <returns></returns>
        public List<int> GetDeletedIndexses()
        {
            return new List<int>(DeletedValues.Keys);
        }

        public override object GetAsObject()
        {
            if (Values.Count == 0)
            {
                return null;
            }
            Type elementType = MemberInfo.GetUnderlyingType().GetArrayOrListUnderlyingType();
            Type listType = typeof(List<>).MakeGenericType(elementType);
            IList list = (IList)Activator.CreateInstance(listType);
            foreach (int key in Values.Keys)
            {
                list.Add(Values[key].GetAsObject());
            }

            // Check if its a list
            if (!MemberInfo.GetUnderlyingType().IsArray)
            {
                return list;
            }
            else
            {
                // It's an array
                Array arr = Array.CreateInstance(elementType, list.Count);
                list.CopyTo(arr, 0);
                return arr;
            }
        }

        
    }
}