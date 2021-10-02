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

        //private Dictionary<int, CSDataObject> DeletedValues = new Dictionary<int, CSDataObject>();

        /// <summary>
        /// Sorted list of keys to the values dictionary
        /// </summary>
        private List<int> ActiveValues = new List<int>();

        /// <summary>
        /// list of keys to the values dictionary (no particular sorting)
        /// </summary>
        private List<int> DeletedValues = new List<int>();

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
            object val = null;
            if (elementType == typeof(string))
            {
                val = "";
            }
            else
            {
                val = Activator.CreateInstance(elementType);
            }
            
            CSDataObject value = Factory.CreateDataObject(null, val, elementType, null, this);
            value.SetModificationState(ModificationStates.NEW);
            Add(value);
            NotifyChanged();
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
            ActiveValues.Add(index);
        }

        /// <summary>
        /// Remove an element from this array
        /// </summary>
        /// <param name="index">The index to remove</param>
        public void Remove(int index)
        {
            if (Values.ContainsKey(index))
            {
                DeletedValues.Add(index);
                ActiveValues.Remove(index);
                Values[index].SetModificationState(ModificationStates.DELETED);
                NotifyChanged();
            }
        }

        /// <summary>
        /// Undoes a remove
        /// </summary>
        /// <param name="index">The index to un remove</param>
        public void UndoRemove(int index)
        {
            if (DeletedValues.Contains(index))
            {
                ActiveValues.Add(index);
                DeletedValues.Remove(index);
                Values[index].SetModificationState(ModificationStates.NONE);
            }
        }

        /// <summary>
        /// Moves the index to the moveTargetIndex. If moveBefore is true it moves in before if not after.
        /// </summary>
        /// <param name="index">The index to move</param>
        /// <param name="moveTargetIndex">The index to move next to</param>
        /// <param name="moveBefore">If true move before else move after</param>
        public void Move(int index, int moveTargetIndex, bool moveBefore)
        {
            if (ActiveValues.Contains(index) && ActiveValues.Contains(moveTargetIndex))
            {
                ActiveValues.Remove(index);
                int indexOf = ActiveValues.IndexOf(moveTargetIndex);
                if (moveBefore)
                {
                    ActiveValues.Insert(indexOf, index);
                }
                else
                {
                    ActiveValues.Insert(indexOf+1, index);
                }
                NotifyChanged();
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
            return null;
        }

        /// <summary>
        /// Get the indexses of this array
        /// </summary>
        /// <returns></returns>
        public IList<int> GetUsedIndexes()
        {
            return ActiveValues.AsReadOnly();
        }

        /// <summary>
        /// Get the deleted indexes of this array
        /// </summary>
        /// <returns></returns>
        public IList<int> GetDeletedIndexses()
        {
            return DeletedValues.AsReadOnly();
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
            foreach (int key in ActiveValues)
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

        public override List<CSDataObject> GetChildren()
        {
            List<CSDataObject> children = new List<CSDataObject>();
            ActiveValues.ForEach(v => children.Add(Values[v]));
            return children;
        }
    }
}