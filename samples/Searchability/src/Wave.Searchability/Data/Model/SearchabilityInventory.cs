using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    /// <summary>
    /// </summary>
    /// <seealso cref="Wave.Searchability.Data.Searchable" />
    [DataContract(Name = "inventory")]
    public class SearchabilityInventory : Searchable
    {
        #region Fields

        /// <summary>
        ///     The default inventory
        /// </summary>
        public static SearchabilityInventory Default = Read<SearchabilityInventory>(new SearchabilityInventory().ConfigurationFile);

        private string _Location;
        private List<SearchablePackage> _Packages;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchabilityInventory" /> class.
        /// </summary>
        public SearchabilityInventory()
            : base("Inventory")
        {
            this.Location = Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Wave"), "Searchability"), this.Name);

            if (!File.Exists(this.ConfigurationFile))
                Write(this.ConfigurationFile, this);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the location.
        /// </summary>
        /// <value>
        ///     The location.
        /// </value>
        [DataMember(Name = "location")]
        public string Location
        {
            get { return _Location; }
            set
            {
                _Location = value;
                _Packages = null;
            }
        }

        /// <summary>
        ///     Gets or sets the packages.
        /// </summary>
        /// <value>
        ///     The packages.
        /// </value>
        [IgnoreDataMember]
        public List<SearchablePackage> Packages
        {
            get { return _Packages ?? (_Packages = this.GetJsonFiles().Select(file => file.FullName).Select(Read<SearchablePackage>).ToList()); }
        }

        #endregion

        #region Private Properties

        /// <summary>
        ///     Gets the json file.
        /// </summary>
        /// <value>
        ///     The json file.
        /// </value>
        [IgnoreDataMember]
        private string ConfigurationFile
        {
            get
            {
                var jsonFile = Path.Combine(Path.GetFullPath(Path.Combine(this.Location, @"..\")), this.Name) + ".json";
                return jsonFile;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Saves the changes to the inventory.
        /// </summary>
        /// <returns>Returns <see cref="bool" /> when the inventory has been saved.</returns>
        public bool Save()
        {
            foreach (var package in this.Packages)
            {
                var jsonFile = Path.Combine(this.Location, package.Name) + ".json";
                Write(jsonFile, package);
            }

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets all of the JSON files that reside in the specified paths.
        /// </summary>
        /// <returns>Returns a <see cref="IEnumerable{FileInfo}" /> representing the files.</returns>
        private IEnumerable<FileInfo> GetJsonFiles()
        {
            var dir = Directory.CreateDirectory(this.Location);
            var files = dir.GetFiles("*.json");
            foreach (var file in files)
            {
                yield return file;
            }
        }

        #endregion
    }
}