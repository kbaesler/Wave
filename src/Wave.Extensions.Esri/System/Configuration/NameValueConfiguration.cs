using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace System.Configuration
{
    /// <summary>
    ///     Provides the ability to read the app.config into groups based on the key prefix
    ///     <example>
    ///         <![CDATA[
    /// <add key="Services/Enabled" value="true"/>
    /// ]]>
    ///     </example>
    /// </summary>
    public class NameValueConfiguration : NameValueCollection
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NameValueConfiguration" /> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        public NameValueConfiguration(string prefix)
        {
            this.Prefix = prefix;

            var items = ConfigurationManager.AppSettings.AllKeys
                .Where(key => key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(k => new KeyValuePair<string, string>(k, ConfigurationManager.AppSettings[k]));

            foreach (var tuple in items)
            {
                this[tuple.Key] = tuple.Value;
            }
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="NameValueConfiguration" /> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="configurations">The configurations.</param>
        public NameValueConfiguration(string prefix, NameValueConfiguration configurations)
        {
            this.Prefix = prefix;

            var items = configurations.AllKeys
                .Where(key => key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(k => new KeyValuePair<string, string>(k, ConfigurationManager.AppSettings[k]));

            foreach (var tuple in items)
            {
                this[tuple.Key] = tuple.Value;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NameValueConfiguration" /> class.
        /// </summary>
        public NameValueConfiguration()
        {
            var items = ConfigurationManager.AppSettings.AllKeys
                .Select(k => new KeyValuePair<string, string>(k, ConfigurationManager.AppSettings[k]));

            foreach (var tuple in items)
            {
                this[tuple.Key] = tuple.Value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the prefix.
        /// </summary>
        /// <value>
        ///     The prefix.
        /// </value>
        public string Prefix { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Adds an entry with the specified name and value to the
        ///     <see cref="T:System.Collections.Specialized.NameValueCollection" />.
        /// </summary>
        /// <param name="name">The <see cref="T:System.String" /> key of the entry to add. The key can be null.</param>
        /// <param name="value">The <see cref="T:System.String" /> value of the entry to add. The value can be null.</param>
        public override void Add(string name, string value)
        {
            base.Add(string.Format("{0}{1}", this.Prefix, name), value);
        }

        /// <summary>
        ///     Gets the value associated with the specified key and parses the value based on the function delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector">The property selector.</param>
        /// <returns>
        ///     Returns a <see cref="T" /> repersenting a strongly typed value for the key.
        /// </returns>
        public virtual string Get<T>(Expression<Func<T>> propertySelector)
        {
            var expression = (MemberExpression) propertySelector.Body;
            return this.Get(expression.Member.Name);
        }

        /// <summary>
        ///     Gets the value associated with the specified key and parses the value based on the function delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector">The property selector.</param>
        /// <param name="func">The function delegate used to parse the value into a type.</param>
        /// <returns>
        ///     Returns a <see cref="T" /> repersenting a strongly typed value for the key.
        /// </returns>
        public virtual T Get<T>(Expression<Func<T>> propertySelector, Func<string, T> func)
        {
            var value = this.Get(propertySelector);
            return func(value);
        }

        /// <summary>
        ///     Gets the value associated with the specified key and parses the value based on the function delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="func">The function delegate used to parse the value into a type.</param>
        /// <returns>Returns a <see cref="T" /> repersenting a strongly typed value for the key.</returns>
        public virtual T Get<T>(string name, Func<string, T> func)
        {
            var value = this.Get(name);
            return func(value);
        }

        /// <summary>
        ///     Gets the values associated with the specified key from the
        ///     <see cref="T:System.Collections.Specialized.NameValueCollection" /> combined into one comma-separated list.
        /// </summary>
        /// <param name="name">
        ///     The <see cref="T:System.String" /> key of the entry that contains the values to get. The key can be
        ///     null.
        /// </param>
        /// <returns>
        ///     A <see cref="T:System.String" /> that contains a comma-separated list of the values associated with the specified
        ///     key from the <see cref="T:System.Collections.Specialized.NameValueCollection" />, if found; otherwise, null.
        /// </returns>
        public override string Get(string name)
        {
            var value = base.Get(string.Format("{0}{1}", this.Prefix, name));

            if (!string.IsNullOrEmpty(value)
                && (value.StartsWith("~\\") || value.StartsWith("..") || value.StartsWith(".")))
            {
                string dir = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                if (value.StartsWith("~\\"))
                {
                    return Path.GetFullPath(Path.Combine(dir, value.Replace("~", "..")));
                }

                return Path.GetFullPath(Path.Combine(dir, value));
            }

            if (!string.IsNullOrEmpty(value))
            {
                foreach (Match m in Regex.Matches(value, @"\%(.*?)\%"))
                {
                    var variable = Environment.ExpandEnvironmentVariables(m.Value);
                    value = value.Replace(m.Value, variable);
                }
            }

            return value;
        }


        /// <summary>
        ///     Gets the values associated with the specified key from the
        ///     <see cref="T:System.Collections.Specialized.NameValueCollection" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector">The property selector.</param>
        /// <returns>
        ///     A <see cref="T:System.String" /> array that contains the values associated with the specified key from the
        ///     <see cref="T:System.Collections.Specialized.NameValueCollection" />, if found; otherwise, null.
        /// </returns>
        public virtual string[] GetValues<T>(Expression<Func<T>> propertySelector)
        {
            var expression = (MemberExpression) propertySelector.Body;
            return this.GetValues(expression.Member.Name);
        }

        /// <summary>
        ///     Gets the values associated with the specified key from the
        ///     <see cref="T:System.Collections.Specialized.NameValueCollection" />.
        /// </summary>
        /// <param name="name">
        ///     The <see cref="T:System.String" /> key of the entry that contains the values to get. The key can be
        ///     null.
        /// </param>
        /// <returns>
        ///     A <see cref="T:System.String" /> array that contains the values associated with the specified key from the
        ///     <see cref="T:System.Collections.Specialized.NameValueCollection" />, if found; otherwise, null.
        /// </returns>
        public override string[] GetValues(string name)
        {
            return base.Get(string.Format("{0}{1}", this.Prefix, name)).Split(new[] {" ,", ", ", ","}, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        ///     Returns the configurations as-a dictionary.
        /// </summary>
        /// <returns>Returns a <see cref="Dictionary{String, String}" /> representing the configurations.</returns>
        public Dictionary<string, string> ToDictionary()
        {
            return this.AllKeys.ToDictionary(key => key, key => this[key]);
        }

        #endregion

        #region Nested Type: BooleanSetting

        public class BooleanSetting : Setting<bool>
        {
            #region Constructors

            public BooleanSetting(NameValueCollection settings, string name, bool value)
                : base(settings, name, s => string.IsNullOrEmpty(s) ? value : bool.Parse(s))
            {
            }

            #endregion
        }

        #endregion

        #region Nested Type: IntegerSetting

        public class IntegerSetting : Setting<int>
        {
            #region Constructors

            public IntegerSetting(NameValueCollection settings, string name, int value)
                : base(settings, name, s => string.IsNullOrEmpty(s) ? value : int.Parse(s))
            {
            }

            #endregion
        }

        #endregion

        #region Nested Type: Setting

        public class Setting<TValue>
        {
            #region Constructors

            public Setting(NameValueCollection settings, string name, Func<string, TValue> func, Action<NameValueCollection, TValue> action)
            {
                this.Name = name;
                this.Value = func(settings[name]);

                action(settings, this.Value);
            }

            public Setting(NameValueCollection settings, string name, Func<string, TValue> func)
                : this(settings, name, func, (collection, value) => collection[name] = value.ToString())
            {
            }

            #endregion

            #region Public Properties

            public string Name { get; private set; }
            public TValue Value { get; private set; }

            #endregion
        }

        #endregion

        #region Nested Type: StringSetting

        public class StringSetting : Setting<string>
        {
            #region Constructors

            public StringSetting(NameValueCollection settings, string name, string value)
                : base(settings, name, s => string.IsNullOrEmpty(s) ? value : s)
            {
            }

            #endregion
        }

        #endregion
    }
}