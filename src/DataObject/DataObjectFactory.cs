using System;
using System.IO;
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
                // Refresh if needed
                GetAssembly();
                return _systemTypes;
            }
        }

        private Dictionary<string, CSDataObjectClass> MetaDataBuffer = new Dictionary<string, CSDataObjectClass>();

        /// <summary>
        /// Do not access directly, use GetAssembly() instead if needed so you get the latest assembly
        /// </summary>
        private Assembly BinaryAssembly = null;
        public string BinaryLocation;
        private DateTime BinaryLastWriteTime = new DateTime(0);

        public Type DataAttributeType {get; private set;}

        public Exception BinaryLoadException;

        public DataObjectFactory(Type dataAttributeType, string assemblyLocation)
        {
            BinaryLocation = assemblyLocation;
            GetAssembly();
            DataAttributeType = dataAttributeType;
        }

        private DateTime GetLastBinaryWriteTime()
        {
            return File.GetLastWriteTime(BinaryLocation);
        }

        /// <summary>
        /// Get the latest version of the assembly
        /// </summary>
        /// <returns>The latest assembly</returns>
        public Assembly GetAssembly()
        {
            // We might also get an assembly string
            if (!File.Exists(BinaryLocation))
            {
                if (BinaryAssembly == null)
                {
                    // Attempt a normal load
                    try
                    {
                        BinaryAssembly = Assembly.Load(BinaryLocation);
                        _systemTypes = BinaryAssembly.GetType().Assembly.GetExportedTypes().ToList();
                    }
                    catch (Exception ex)
                    {
                        BinaryLoadException = ex;
                    }
                }
                return BinaryAssembly;
            }

            DateTime lastWrite = GetLastBinaryWriteTime();
            if (lastWrite > BinaryLastWriteTime)
            {
                BinaryLastWriteTime = lastWrite;
                try
                {
                    BinaryAssembly = Assembly.Load(File.ReadAllBytes(BinaryLocation));
                    _systemTypes = BinaryAssembly.GetType().Assembly.GetExportedTypes().ToList();
                }
                catch (Exception ex)
                {
                    BinaryLoadException = ex;
                }
            }
            return BinaryAssembly;
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
            if ((!isCustom && !isArray) || underlying.IsEnum)
            {
                dataObject = CreateDataObjectForMember(memberInfo, value, underlying);
                if (memberInfo == null)
                {
                    dataObject.CustomAttributes = parent.CustomAttributes;
                }
                FillDataObject(dataObject, memberInfo);
            }
            else if (isArray)
            {
                dataObject = new CSDataObjectMemberArray(this);
                FillDataObject(dataObject, memberInfo);
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
                            cSDataObject = CreateDataObjectForMember(null, child, underlying);
                            cSDataObject.Parent = dataObject;
                            cSDataObject.CustomAttributes = dataObject.CustomAttributes;
                        }

                        ((CSDataObjectMemberArray)dataObject).Add(cSDataObject);
                    }
                }
                
            }
            else
            {
                // We are a custom class
                dataObject = CreateDataObjectClass(value, underlying, parent);
                FillDataObject(dataObject, memberInfo);
            }

            dataObject.Parent = parent;
            dataObject.MemberInfo = memberInfo;

            return dataObject;
        }

        private CSDataObject CreateDataObjectForMember(MemberInfo memberInfo, object value, Type underlying)
        {
            if (memberInfo != null)
            {
                return new CSDataObjectMember(value, this, GetValueConverter(memberInfo));
            }
            else
            {
                return new CSDataObjectMember(value, this, new CSDOValueConverter(underlying));
            }
        }


        private void FillDataObject(CSDataObject dataObject, MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                return;
            }
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

            if (dataObject is CSDataObjectMember && memberInfo.GetUnderlyingType() == typeof(bool)
                && dataObject.GetCustomAttribute<CSDORenderer>() == null)
            {
                // We got a bool with no renderer so add bool renderer
                dataObject.CustomAttributes.Add(new CSDOListRendererBool());
            }
        }

        private CSDOValueConverter GetValueConverter(MemberInfo memberInfo)
        {
            CSDOValueConverter converter = null;
            foreach (Attribute convert in memberInfo.GetAttributesThatInheritFromType(typeof(CSDOValueConverter)))
            {
                converter = (CSDOValueConverter)convert;
                break;
            }

            if (converter == null)
            {
                converter = new CSDOValueConverter(memberInfo.GetUnderlyingType());
            }

            return converter;
        }

    }

}