using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace CSharpDataEditorDll
{

    public class DataObjectFactory
    {
        private List<Type> _systemTypes;
        private List<Type> SystemTypes
        {
            get
            {
                if (_systemTypes == null)
                {
                    _systemTypes = BinaryAssembly.GetType().Assembly.GetExportedTypes().ToList();
                }
                return _systemTypes;
            }
        }

        private Dictionary<string, CSDataObjectClass> MetaDataBuffer = new Dictionary<string, CSDataObjectClass>();

        private Assembly BinaryAssembly;
        private Type DataAttributeType;

        public DataObjectFactory(Type dataAttributeType, Assembly assembly)
        {
            BinaryAssembly = assembly;
            DataAttributeType = dataAttributeType;
        }

        /// <summary>
        /// Create a CSharpDataObject from the input object
        /// </summary>
        /// <param name="inputObject"></param>
        /// <returns>A new CSharpDataObject</returns>
        public CSDataObjectClass Create(object inputObject)
        {
            return CreateDataObjectClass(inputObject, inputObject.GetType(), null);
        }

        public CSDataObjectClass CreateDataObjectClass(object inputObject, Type inputObjectType, CSDataObject parent)
        {
            CSDataObjectClass obj = new CSDataObjectClass(this);
            obj.Parent = parent;
            obj.ClassType = inputObjectType;
            FillDataObject(obj, inputObjectType);

            if (inputObject != null)
            {
                // Process all members
                foreach (MemberInfo member in inputObjectType.GetMembers())
                {
                    // Do not process members that are not json attributes
                    if (member.GetCustomAttribute(DataAttributeType) == null)
                    {
                        continue;
                    }
                    CSDataObject dataObject = CreateDataObject(inputObject, member, obj);
                    obj.ClassMembers.Add(dataObject);
                }
            }

            return obj;
        }

        public CSDataObject CreateDataObject(object inputObject, MemberInfo memberInfo, CSDataObject parent)
        {
            return CreateDataObject(inputObject, memberInfo.GetValue(inputObject), memberInfo.GetUnderlyingType(), memberInfo, parent);
        }

        public CSDataObject CreateDataObject(object inputObject, object value, Type underlying, MemberInfo memberInfo, CSDataObject parent)
        {
            CSDataObject dataObject;
            bool isArray = false;

            // Check if this is an array
            if (underlying.IsArray())
            {
                underlying = underlying.GetArrayOrListUnderlyingType();
                isArray = true;
            }

            // Check if this is a custom type
            bool isCustom = !SystemTypes.Contains(underlying);
            
            // Check if this is just a basic value
            if (!isCustom && !isArray)
            {
                dataObject = new CSDataObjectMember(value, this);
                // TODO: Check what basic type it is, for now all is string
            }
            else if (isArray)
            {
                dataObject = new CSDataObjectMemberArray(this);
                if (value != null)
                {
                    // Create CSDataObject for each array value and store in hashmap
                    IEnumerable enumerable = value as IEnumerable;
                    foreach (object child in enumerable)
                    {
                        CSDataObject cSDataObject;

                        if (isCustom)
                        {
                            cSDataObject = CreateDataObjectClass(child, underlying, dataObject);
                        }
                        else
                        {
                            cSDataObject = new CSDataObjectMember(child, this);
                        }

                        ((CSDataObjectMemberArray)dataObject).Add(cSDataObject);
                    }
                }
                
            }
            else
            {
                // We are a custom class
                dataObject = CreateDataObjectClass(value, underlying, parent);
            }

            dataObject.Parent = parent;
            dataObject.MemberInfo = memberInfo;
            FillDataObject(dataObject, memberInfo);

            return dataObject;
        }


        private void FillDataObject(CSDataObject dataObject, MemberInfo memberInfo)
        {
            if (dataObject.CustomAttributes.Count == 0 && memberInfo != null)
            {
                // Get any custom attributes
                foreach (object attribute in memberInfo.GetCustomAttributes())
                {
                    Type t = attribute.GetType();
                    if (typeof(CSDOCustomAtrribute).IsAssignableFrom(t))
                    {
                        dataObject.CustomAttributes.Add((CSDOCustomAtrribute)attribute);
                    }
                }
            }
        }

    }

}