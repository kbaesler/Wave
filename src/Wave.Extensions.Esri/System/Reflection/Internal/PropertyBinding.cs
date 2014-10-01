using System.Linq;

namespace System.Reflection.Internal
{
    /// <summary>
    ///     A supporting class used to handle interacte with obtaining the value from the object using reflection.
    /// </summary>
    internal static class PropertyBinding
    {
        #region Public Methods

        /// <summary>
        ///     Gets the value for the specified property on the object.
        /// </summary>
        /// <param name="x">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        ///     A <see cref="object" /> for the corresponding property name, otherwise null.
        /// </returns>
        public static object GetValue(object x, string propertyName)
        {
            object xo = null;
            PropertyInfo xp;

            if (x == null)
                return null;

            PropertyInfo[] props = x.GetType().GetProperties();
            if (propertyName.Contains("."))
            {
                int index = propertyName.IndexOf('.');
                string startsWith = propertyName.Substring(0, index);

                xp = props.FirstOrDefault(p => p.Name.Equals(startsWith));
                if (xp != null)
                {
                    string endsWith = propertyName.Substring(index + 1, propertyName.Length - index - 1);
                    return GetValue(xp.GetValue(x, new object[] {}), endsWith);
                }
            }

            xp = props.FirstOrDefault(p => p.Name.Equals(propertyName));
            if (xp != null) xo = xp.GetValue(x, new object[] {});
            return xo;
        }

        #endregion
    }
}