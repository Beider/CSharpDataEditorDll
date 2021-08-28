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
        /// <param name="typeName">The class type this data reader should read</param>
        /// <param name="assemblyPath">The path to the assembly this is for</param>
        bool Init(string parameters, string typeName, string assemblyPath);

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

        /// <summary>
        /// Return a list of all known objects that can be accessed with GetObject
        /// </summary>
        /// <returns>A list of object names</returns>
        string[] GetValidObjectNames();

        /// <summary>
        /// Get the last error if there was an error
        /// </summary>
        string GetError();

    }
}
