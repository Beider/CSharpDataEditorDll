using System;
using System.Reflection;

namespace CSharpDataEditorDll
{
    public interface IDataConverter
    {
        /// <summary>
        /// Initializes the data converter
        /// </summary>
        /// <param name="parameters">The parameter for initialization</param>
        /// <param name="type">The class type this data reader should read</param>
        /// <param name="assembly">The assembly this type is from</param>
        void Init(string parameters, Type type, Assembly assembly);

        /// <summary>
        /// Request the object with the given name
        /// </summary>
        /// <param name="name">Name of the object</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        CSDataObjectClass GetObject(string name);


        /// <summary>
        /// Save the object to storage
        /// </summary>
        /// <param name="name">Name of the object</param>
        /// <param name="dataObject">The object to store</param>
        /// <returns></returns>
        bool SaveObject(string name, CSDataObject dataObject);

    }
}
