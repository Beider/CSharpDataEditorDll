using System;
using System.Reflection;
using System.Collections.Generic;

namespace CSharpDataEditorDll
{
    public abstract class CSDataObject
    {
        public enum ModificationStates
        {
            NONE, NEW, EDITED, DELETED
        }

        public CSDataObject(DataObjectFactory factory)
        {
            Factory = factory;
        }

        /// <summary>
        /// The factory that created this data object
        /// </summary>
        public DataObjectFactory Factory;

        /// <summary>
        /// The member info this MetaData is for
        /// </summary>
        public MemberInfo MemberInfo;

        public CSDataObject Parent;

        public ModificationStates ModificationState {get; private set;} = ModificationStates.NONE;

        public List<CSDOCustomAtrribute> CustomAttributes = new List<CSDOCustomAtrribute>();

        public virtual object GetAsObject()
        {
            return null;
        }

        protected virtual bool IsModified()
        {
            return false;
        }

        public void SetModificationState(ModificationStates state)
        {
            // Do not change new items to edited
            if (ModificationState == ModificationStates.NEW && state == ModificationStates.EDITED)
            {
                return;
            }

            if (state == ModificationStates.NONE && IsModified())
            {
                ModificationState = ModificationStates.EDITED;
                return;
            }

            ModificationState = state;
        }
    }
}