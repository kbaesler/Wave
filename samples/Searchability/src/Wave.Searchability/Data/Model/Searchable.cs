using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Wave.Searchability.Data
{
    [DebuggerDisplay("Name = {Name}, AliasName = {AliasName}")]
    [DataContract]
    public abstract class Searchable
    {
        #region Constants

        /// <summary>
        ///     A constant that represents 'Any' or 'All'.
        /// </summary>
        protected const string Any = "*";

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Searchable" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected Searchable(string name)
            : this(name, name)
        {            
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Searchable" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="aliasName">Name of the alias.</param>
        protected Searchable(string name, string aliasName)
        {
            this.Name = name;        
            this.AliasName = aliasName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the alias.
        /// </summary>
        /// <value>
        ///     The name of the alias.
        /// </value>
        [DataMember(Name = "aliasName")]
        [Category("Appearance")]
        [Description("The alias name or description of the item.")]
        public virtual string AliasName { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [DataMember(Name = "name")]
        [Category("Appearance")]
        [Description("The name that identifies the item.")]
        public virtual string Name { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a copy of the object using a the JSON Data Contract Serializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns a <see cref="T" /> repersenting a new instance of the object.</returns>
        public static T Copy<T>(T data)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T));

            using (Stream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, data);

                stream.Seek(0, SeekOrigin.Begin);

                return (T) serializer.ReadObject(stream);
            }
        }

        /// <summary>
        ///     Minifies blocks of JSON-like content into valid JSON by removing all whitespace and comments.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Returns a <see cref="string" /> representing the minified JSON data.</returns>
        public static string Minify(string data)
        {
            var min = Regex.Replace(data, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
            return min;
        }

        /// <summary>
        /// Parses the specified json file into the JSON object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonFile">The json file.</param>
        /// <param name="action">The function that is called when the file is not found.</param>
        /// <returns>
        /// Returns a <see cref="SearchablePackage" /> repesenting the package of searchable items.
        /// </returns>
        public static T Read<T>(string jsonFile, Action action) where T : Searchable
        {
            if (!File.Exists(jsonFile))
                action();

            var data = File.ReadAllText(jsonFile);
            var min = Minify(data);

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(min)))
                return (T)serializer.ReadObject(stream);
        }

        /// <summary>
        ///     Parses the specified json file into the JSON object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonFile">The json file.</param>
        /// <returns>
        ///     Returns a <see cref="SearchablePackage" /> repesenting the package of searchable items.
        /// </returns>
        public static T Read<T>(string jsonFile)
            where T : Searchable
        {
            return Read<T>(jsonFile, () => { throw new FileNotFoundException("The JSON file was not found.", jsonFile);});
        }


        /// <summary>
        ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return this.AliasName ?? this.Name;
        }

        /// <summary>
        ///     Writes the JSON data to the specified file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonFile">The json file.</param>
        /// <param name="data">The data.</param>
        public static void Write<T>(string jsonFile, T data)
            where T : Searchable
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T));
            using (FileStream stream = new FileStream(jsonFile, FileMode.Create))
                serializer.WriteObject(stream, data);
        }

        #endregion
    }
}