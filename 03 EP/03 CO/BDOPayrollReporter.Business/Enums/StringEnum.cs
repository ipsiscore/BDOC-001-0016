using System;
using System.Collections;
using System.Reflection;

namespace BDOPayrollReporter.Business.Enums
{
    /// <summary>
    /// Helper class for working with 'extended' enums using <see cref="StringValueAttribute"/> attributes.
    /// </summary>
    public class StringEnum
    {
        #region Instance implementation

        private Type _enumType;

        /// <summary>
        /// Creates a new instance of the <see cref="StringEnum"/> class.
        /// </summary>
        /// <param name="enumType">Enum type.</param>
        public StringEnum(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException(String.Format("Supplied type must be an Enum. Type was {0}.", enumType.ToString()));

            _enumType = enumType;
        }

        /// <summary>
        /// Gets the string value associated with the given enum value.
        /// </summary>
        /// <param name="valueName">Name of the enum value.</param>
        /// <returns>String Value of the given enum value.</returns>
        public string GetStringValue(string valueName)
        {
            Enum enumType;
            string stringValue = null;
            try
            {
                enumType = (Enum)Enum.Parse(_enumType, valueName);
                stringValue = GetStringValue(enumType);
            }
            catch (Exception) { }

            return stringValue;
        }

        /// <summary>
        /// Gets the string values associated with the enum.
        /// </summary>
        /// <returns>String value array containing all string values associated with the enum.</returns>
        public Array GetStringValues()
        {
            ArrayList values = new ArrayList();
            foreach (FieldInfo fi in _enumType.GetFields())
            {
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if (attrs.Length > 0)
                    values.Add(attrs[0].Value);
            }
            return values.ToArray();
        }

        /// <summary>
        /// Gets the values as a bindable list datasource.
        /// </summary>
        /// <returns>IList for data binding.</returns>
        public IList GetListValues()
        {
            Type underlyingType = Enum.GetUnderlyingType(_enumType);
            ArrayList values = new ArrayList();
            foreach (FieldInfo fi in _enumType.GetFields())
            {
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if (attrs.Length > 0)
                    values.Add(new DictionaryEntry(Convert.ChangeType(Enum.Parse(_enumType, fi.Name), underlyingType), attrs[0].Value));
            }
            return values;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="stringValue">String value to find.</param>
        /// <returns>Returns true if an item is defined with the given String Value, otherwise return false.</returns>
        public bool IsStringDefined(string stringValue)
        {
            return Parse(_enumType, stringValue) != null;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="stringValue">String value to find.</param>
        /// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value</param>
        /// <returns>Returns true if an item is defined with the given String Value, otherwise return false.</returns>
        public bool IsStringDefined(string stringValue, bool ignoreCase)
        {
            return Parse(_enumType, stringValue, ignoreCase) != null;
        }

        /// <summary>
        /// Gets the underlying enum type for this instance.
        /// </summary>
        /// <value></value>
        public Type EnumType
        {
            get { return _enumType; }
        }

        #endregion Instance implementation

        #region Static implementation

        private static Hashtable _stringValues = new Hashtable();

        /// <summary>
        /// Gets a string value for a particular enum value.
        /// </summary>
        /// <param name="value">Enum Value to find.</param>
        /// <returns>String Value associated via a <see cref="StringValueAttribute"/> attribute, or null if not found.</returns>
        public static string GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();

            if (_stringValues.ContainsKey(value))
                output = (_stringValues[value] as StringValueAttribute).Value;
            else
            {
                FieldInfo fi = type.GetField(value.ToString());
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if (attrs.Length > 0)
                {
                    _stringValues.Add(value, attrs[0]);
                    output = attrs[0].Value;
                }
            }
            return output;
        }

        /// <summary>
        /// Parses the supplied enum and string value to find an associated enum value.
        /// </summary>
        /// <param name="type">Type to search in.</param>
        /// <param name="stringValue">String value to find.</param>
        /// <returns>Enum value associated with the string value, or null if not found.</returns>
        /// <remarks>This method is case sensitive.</remarks>
        public static object Parse(Type type, string stringValue)
        {
            return Parse(type, stringValue, false);
        }

        /// <summary>
        /// Parses the supplied enum and string value to find an associated enum value.
        /// </summary>
        /// <param name="type">Type to search in.</param>
        /// <param name="stringValue">String value to find.</param>
        /// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value.</param>
        /// <returns>Enum value associated with the string value, or null if not found.</returns>
        public static object Parse(Type type, string stringValue, bool ignoreCase)
        {
            object output = null;
            string enumStringValue = null;

            if (!type.IsEnum)
                throw new ArgumentException(String.Format("Supplied type must be an Enum.  Type was {0}", type.ToString()));

            foreach (FieldInfo fi in type.GetFields())
            {
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if (attrs.Length > 0)
                    enumStringValue = attrs[0].Value;

                if (string.Compare(enumStringValue, stringValue, ignoreCase) == 0)
                {
                    output = Enum.Parse(type, fi.Name);
                    break;
                }
            }
            return output;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="stringValue">String value to find.</param>
        /// <param name="enumType">Type of enum where search in.</param>
        /// <returns>Returns a <c>System.Boolean</c> value indicating if the given string value exists in the collection.</returns>
        public static bool IsStringDefined(Type enumType, string stringValue)
        {
            return Parse(enumType, stringValue) != null;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="stringValue">String value to find.</param>
        /// <param name="enumType">Type of enum where search in.</param>
        /// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value.</param>
        /// <returns>Returns a <c>System.Boolean</c> value indicating if the given string value exists in the collection.</returns>
        public static bool IsStringDefined(Type enumType, string stringValue, bool ignoreCase)
        {
            return Parse(enumType, stringValue, ignoreCase) != null;
        }

        /// <summary>
        /// Returns the enum associated with the given int.
        /// </summary>
        /// <typeparam name="T">The type of enum to parse.</typeparam>
        /// <param name="number">The int for which to return the enum value.</param>
        /// <returns>The enum as provided by the generic T.</returns>
        [Obsolete("This method became obsolete in order to support diferent types of enums.", false)]
        public static T NumToEnum<T>(int number)
        {
            return (T)Enum.ToObject(typeof(T), number);
        }

        #endregion Static implementation
    }
}