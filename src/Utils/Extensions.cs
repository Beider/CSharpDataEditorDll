using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace CSharpDataEditorDll
{
    public static class Extensions
    {
        /// <summary>
        /// Sets the value of this member
        /// </summary>
        /// <param name="member">The member</param>
        /// <param name="Instance">The instance to set the value for</param>
        /// <param name="Value">The value</param>
        public static void SetValue(this MemberInfo member, object Instance, object Value)
        {
            try
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        ((FieldInfo) member).SetValue(Instance, Value);
                        break;
                    case MemberTypes.Property:
                        ((PropertyInfo) member).SetValue(Instance, Value);
                        break;
                    default:
                        Console.Error.WriteLine(
                            $"Input MemberInfo was of type {member.MemberType.ToString()}, it should be of type FieldInfo or PropertyInfo");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Get the value of this member
        /// </summary>
        /// <param name="member">The member</param>
        /// <param name="Instance">The instance to get the value of</param>
        /// <returns>The value of the member in the instance</returns>
        public static object GetValue(this MemberInfo member, object Instance)
        {
            if (Instance == null)
            {
                return null;
            }
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) member).GetValue(Instance);
                case MemberTypes.Property:
                    return ((PropertyInfo) member).GetValue(Instance);
                default:
                    Console.Error.WriteLine(
                        $"Input MemberInfo was of type {member.MemberType.ToString()}, it should be of type FieldInfo or PropertyInfo");
                    break;
            }
            return null;
        }

        /// <summary>
        /// Gets the underlying type of a member
        /// </summary>
        /// <param name="member">The member to find the type for</param>
        /// <returns>The underlying type</returns>
        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo) member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo) member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo) member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo) member).PropertyType;
                case MemberTypes.Constructor:
                    return null;
                default:
                    Console.Error.WriteLine(
                        $"Input MemberInfo was of type {member.MemberType.ToString()}, it should be of type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
                    return null;
            }
        }

        public static bool IsArray(this MemberInfo member)
        {
            return  member.GetUnderlyingType().IsArray();
        }

        public static bool IsArray(this Type type)
        {
            // Exclude string as it is an array of characters
            if (type == null || type == typeof(string))
            {
                return false;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return true;
            }
            return false;
        }

        public static MethodInfo ResolveMethodInfo(this Type classType, string name)
        {
            if (classType == null || name == null || name == "")
            {
                return null;
            }
            foreach (MethodInfo info in classType.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                if (info.Name == name)
                {
                    return info;
                }
            }

            return null;
        }

        public static Type GetArrayOrListUnderlyingType(this Type type)
        {
            if (type.GetElementType() != null)
            {
                return type.GetElementType();
            }
            else if (type.GenericTypeArguments.Length > 0)
            {
                return type.GenericTypeArguments[0];
            }
            return null;
        }

        public static List<Attribute> GetAttributesThatInheritFromType(this MemberInfo instance, Type inheritFromType)
        {
            List<Attribute> returnList = new List<Attribute>();
            foreach (Attribute attribute in instance.GetCustomAttributes())
            {
                Type type = attribute.GetType();
                if (inheritFromType.IsAssignableFrom(type))
                {
                    returnList.Add(attribute);
                }
            }

            return returnList;
        }

        public static Array AddItemToArray(this Array array, object item)
        {
            Array oldArray = array;
            Type elementType = array.GetType().GetElementType();
            Array newArray = Array.CreateInstance(elementType, oldArray.Length+1);
            Array.Copy(oldArray, newArray, oldArray.Length);
            newArray.SetValue(item, newArray.Length-1);
            return newArray;
        }

        public static MemberInfo GetMemberByName(this Type type, string name)
        {
            if (name == null || name == "")
            {
                return null;
            }
            foreach (MemberInfo info in type.GetMembers())
            {
                if (info.Name.Equals(name))
                {
                    return info;
                }
            }

            return null;
        }
    }
}