using System;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Allows you to update something manually whenever a value is changed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public abstract class CSDOOnChangeValueUpdater : CSDOCustomAtrribute
    {
        public enum RefreshTargets
        {
            NOTHING, // Refresh nothing
            SELF, // Only refresh self
            PARENT_CLASS, // Refreshes the class this member belongs to
            ALL // Refresh entire tree
        }

        public CSDOOnChangeValueUpdater()
        {
            
        }

        /// <summary>
        /// Called when the value is updated, should return what we need to refresh
        /// </summary>
        /// <param name="dataObject">The data object that was changed</param>
        /// <returns>An enum representing what we need to refresh</returns>
        public abstract RefreshTargets OnValueUpdate(CSDataObject dataObject);
    }
}