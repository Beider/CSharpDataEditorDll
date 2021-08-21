using System;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Used to convert and validate values
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CSDOValueConverter : Attribute
    {
        public readonly Type ConversionType;

        private string Error = null;

        /// <summary>
        /// Used to convert to / from string
        /// </summary>
        /// <param name="conversionType">The type we are converting</param>
        public CSDOValueConverter(Type conversionType)
        {
            ConversionType = conversionType;
        }

        /// <summary>
        /// Convert the object to string
        /// </summary>
        /// <param name="value">The object to convert</param>
        /// <returns>The string representation of this object</returns>
        public virtual string ConvertToString(object value)
        {
            if (value == null || value.GetType() != ConversionType)
            {
                return "";
            }
            return value.ToString();
        }

        /// <summary>
        /// Converts to the underlying object type from a string
        /// </summary>
        /// <param name="value">The string value to convert</param>
        /// <returns>A value in the type of the conversion type</returns>
        public virtual object ConvertFromString(string value)
        {
            // Just pass it on if we are converting between strings
            if (ConversionType == typeof(string))
            {
                return value;
            }

            Error = null;
            if (value == null || value == "")
            {
                
                return Activator.CreateInstance(ConversionType);
            }

            try
            {
                return Convert.ChangeType(value, ConversionType);
            }
            catch (Exception ex)
            {
                Error = $"Conversion to {ConversionType.Name} failed. Error: {ex.Message}";
                return Activator.CreateInstance(ConversionType);
            }
        }

        public virtual string GetError()
        {
            return Error;
        }
    }
}