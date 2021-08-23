using System;

namespace CSharpDataEditorDll
{
    /// <summary>
    /// Allows you to override the display name with the value of a member
    /// </summary>
    public abstract class CSDORenderer : CSDOCustomAtrribute
    {
        /// <summary>
        /// Denotes if we should use colors or not
        /// </summary>
        protected readonly bool UseColors;

        public CSDORenderer(bool useColors = false)
        {
            this.UseColors = useColors;
        }

        /// <summary>
        /// Get the color for this value
        /// </summary>
        /// <param name="value">The value we want the color for</param>
        /// <param name="dataObject">The data object this value belongs to</param>
        /// <returns>A string representing the color, valid color names can be found here: https://docs.godotengine.org/en/stable/classes/class_color.html.
        /// Null or empty sting for no color.</returns>
        public virtual string GetColor(string value, CSDataObject dataObject)
        {
            return null;
        }

        /// <summary>
        /// Get the background color for this value, may not be applied everywhere
        /// </summary>
        /// <param name="value">The value we want the color for</param>
        /// <param name="dataObject">The data object this value belongs to</param>
        /// <returns>A string representing the color, valid color names can be found here: https://docs.godotengine.org/en/stable/classes/class_color.html.
        /// Null or empty sting for no color.</returns>
        public virtual string GetBgColor(string value, CSDataObject dataObject)
        {
            return null;
        }

        /// <summary>
        /// The type of renderer to use. Please check the CSharpDataEditorGUI project for valid values and how you can add your own.
        /// </summary>
        /// <returns></returns>
        public virtual string GetRenderType()
        {
            return null;
        }


    }
}