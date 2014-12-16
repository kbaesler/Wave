using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;

using Miner.Interop.msxml2;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Allows for reading and writing of the extend data.
    ///     When the design is opened, Designer calls the IMMWMSExtendedData::GetEDMAsXML method for each design object.
    ///     In this case, the appropriate Extended Data must be read from the corresponding database table
    ///     and formulated into the "EDM" structure (this EDM element and children are the return value from this method).
    ///     In both the save and open cases, the Design object is always the first to be sent to this component
    /// </summary>
    public abstract class BasePxEdmRepository : IMMWMSExtendedData, IDisposable, IPxEdmRepository
    {
        #region Fields

        private DataSet _Dataset;
        private EdmRepository _EdmRepository;
        private int _ID;
        private bool _IsDeleted;
        private IMMPxApplication _PxApp;
        private IMMWMSExtendedData _WmsExtendedData;        

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxEdmRepository" /> class.
        /// </summary>
        /// <param name="pxApplication">The process framework application reference.</param>
        protected BasePxEdmRepository(IMMPxApplication pxApplication)
        {
            _PxApp = pxApplication;
        }

        #endregion

        #region Private Properties

        /// <summary>
        ///     Gets the tables.
        /// </summary>
        private IEnumerable<EdmTable> Tables
        {
            get
            {
                if (_EdmRepository == null)
                    _EdmRepository = this.GetRepository(_PxApp);

                if (_EdmRepository == null)
                    return new Collection<EdmTable>();

                return new[]
                {
                    _EdmRepository.WorkRequest,
                    _EdmRepository.Design,
                    _EdmRepository.WorkLocation,
                    _EdmRepository.CompatibleUnit
                }.Where(o => o != null);
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region IMMWMSExtendedData Members

        /// <summary>
        ///     Returns the "EDM" node and child EDMPROP elements containing Extended Data associated with the specified WFM node.
        /// </summary>
        /// <param name="wmsNode">A WFM node object representing a Work Request, Design, Work Location, or Compatible Unit.</param>
        /// <returns>An XML element corresponding to the "EDM" structure.</returns>
        public virtual IXMLDOMNode GetEDMAsXML(IMMWMSNode wmsNode)
        {
            if (_EdmRepository == null)
                return null;

            // If design not valid exit.
            if (!this.IsValid(wmsNode))
                return null;

            // Only load the data from the Extended Data tables once.
            this.Load();

            // Call the OOTB EDM component to get any Site Condition data.
            IXMLDOMNode node = _WmsExtendedData.GetEDMAsXML(wmsNode);
            IXMLDOMElement element;

            if (node == null)
            {
                // Create the base "EDM" element.
                IXMLDOMDocument dom = new DOMDocumentClass();
                element = dom.createElement("EDM");
                dom.documentElement = element;
                node = element;
            }
            else
            {
                element = (IXMLDOMElement) node;
            }

            // Set the appropriate EDM Property data.
            this.UpdateEdm(element, wmsNode);

            // Return the updated node.
            return node;
        }

        /// <summary>
        ///     Initialized the <see cref="Miner.Interop.Process.IMMWMSExtendedData" /> component that is used to save the Site
        ///     Conditions.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing the initialization was succesful; otherwise <c>false</c>.
        /// </returns>
        public virtual bool Initialize(IMMPxApplication pxApp)
        {
            _PxApp = pxApp;
            _EdmRepository = this.GetRepository(pxApp);

            // Create an instance of the default Workflow Manager EDM persist object.  Use this to take care of Site Condition EDM.
            _WmsExtendedData = (IMMWMSExtendedData) Activator.CreateInstance(Type.GetTypeFromProgID("mmWMSExtendedData.clsWmsEDM"));
            return _WmsExtendedData.Initialize(pxApp);
        }

        /// <summary>
        ///     Saves Extended Data for the specified element.
        /// </summary>
        /// <param name="wmsNode">The WFM object being processed (Work Request, Design, Work Location, or Compatible Unit).</param>
        /// <param name="edmNode">The "EDM" XML element from the design XML that corresponds to the WFM object.</param>
        public virtual void WriteEDMFromXML(IMMWMSNode wmsNode, IXMLDOMNode edmNode)
        {
            if (_EdmRepository == null)
                return;

            // If design not valid exit.
            if (!this.IsValid(wmsNode))
                return;

            // Remove any existing Extended Data rows, if necessary.
            if (!_IsDeleted)
            {
                // Delete all the EDM data from the Work Request, Design, Work Locaiton and CUs associated with the current design id.
                this.Delete(_ID);

                // Update the flag.
                _IsDeleted = true;
            }

            // Iterate through each of the Extended Data properties associated with the node and save non Site-Condition EDM to EDM tables.
            IXMLDOMNodeList nodelist = edmNode.selectNodes("EDMPROP");
            foreach (IXMLDOMNode node in nodelist)
            {
                EDM edm = new EDM();
                edm.Name = node.attributes.getNamedItem("Name").text;
                edm.Value = node.text;
                edm.Type = node.attributes.getNamedItem("Type").text;

                if (_EdmRepository.Type.Equals(edm.Type, StringComparison.OrdinalIgnoreCase))
                {
                    this.Save(wmsNode, edm);
                }
            }

            // Use the OOTB component to save Site Condition info.
            _WmsExtendedData.WriteEDMFromXML(wmsNode, edmNode);
        }

        #endregion

        #region IPxEdmRepository Members

        /// <summary>
        ///     Delete all the EDM data from the Work Request, Design, Work Locaiton and CUs associated with the current design id.
        /// </summary>
        /// <param name="designId">The design ID.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of records that have been deleted.
        /// </returns>
        public int Delete(int designId)
        {
            int recordsAffected = 0;

            // Iterate through all of the valid tables.
            foreach (var o in this.Tables)
            {
                // Only proceed if the table is valid.
                if (o.Valid)
                {
                    // Delete all of the records matching the given design ID.
                    string sql = string.Format(CultureInfo.InvariantCulture, "DELETE FROM {0} WHERE {1} = {2}", _PxApp.GetQualifiedTableName(o.TableName), Fields.DesignID, designId);
                    recordsAffected += _PxApp.ExecuteNonQuery(sql);
                }
            }

            return recordsAffected;
        }

        /// <summary>
        ///     Loads all of the EDM data from the Work Request Design, Work Location and CUs associated with the design id.
        /// </summary>
        /// <param name="designId">The design identifier.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{K, V}" /> representing the table and EDMs for the design.
        /// </returns>
        public Dictionary<string, List<EDM>> GetEdms(int designId)
        {
            var list = new Dictionary<string, List<EDM>>();

            // Iterate through all of the valid tables.
            foreach (var o in this.Tables)
            {
                if (o.Valid)
                {
                    var rows = this.GetRows(o, string.Format(CultureInfo.InvariantCulture, "WHERE {0} = {1}", Fields.DesignID, designId));
                    if (rows != null)
                    {
                        list.Add(o.TableName, rows.Select(row => new EDM(row)).ToList());
                    }
                }
            }

            return list;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the configurations used to populate the EDM skeleton tables.
        /// </summary>
        /// <param name="pxApplication">The process framework application reference.</param>
        /// <returns>
        ///     Returns the <see cref="EdmRepository" /> representing the EDM configurations; otherwise <c>null</c>.
        /// </returns>
        protected abstract EdmRepository GetRepository(IMMPxApplication pxApplication);

        #endregion

        #region Private Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_Dataset != null)
                {
                    _Dataset.Dispose();
                    _Dataset = null;
                }
            }
        }

        /// <summary>
        ///     Gets all of the rows that are within the specified <paramref name="edmTable" /> that satisfies the given
        ///     <paramref name="filter" />.
        /// </summary>
        /// <param name="edmTable">The edm table.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        ///     An array of <see cref="DataRow" /> objects; otherwise <c>null</c>.
        /// </returns>
        private IEnumerable<DataRow> GetRows(EdmTable edmTable, string filter)
        {
            if (_Dataset == null)
                return null;

            if (edmTable == null || !edmTable.Valid)
                return null;

            string tableName = _PxApp.GetQualifiedTableName(edmTable.TableName);
            if (!_Dataset.Tables.Contains(tableName))
                return null;

            // Locate the table within the dataset that matches the given name.
            DataTable table = _Dataset.Tables[tableName];
            if (table == null) return null;

            // Obtain all of the rows the satisfy the given filter.
            DataRow[] rows = table.Select(filter, "", DataViewRowState.CurrentRows);
            return rows;
        }

        /// <summary>
        ///     Determines if the node is valid, which means it needs to have a design id > 0.
        /// </summary>
        /// <param name="node">IMMWMSNode which can be the WorkRequest, Design, WorkLocation, or CU.</param>
        /// <returns>Returns true if this node has a valid IMMWMSDesign ID</returns>
        private bool IsValid(IMMWMSNode node)
        {
            // The design node should be the first node passed to this component (even before the Work Request).
            if (_ID != 0) return true;

            IMMWMSDesign design = node as IMMWMSDesign;
            if (design != null)
                _ID = design.ID;
            else
                return false;

            return _ID != 0;
        }

        /// <summary>
        ///     Loads all the EDM data fro the Work Request, Design, Work Locations, and CUs associated with the current Design ID.
        /// </summary>
        private void Load()
        {
            if (_Dataset == null)
            {
                // Only load those tables that are configured.
                _Dataset = new DataSet("EDM");
                _Dataset.Locale = CultureInfo.InvariantCulture;

                // Iterate through all of the valid tables.
                foreach (var o in this.Tables)
                {
                    // Only proceed if the table is valid.
                    if (o.Valid)
                    {
                        // Query the table for all of the records matching the given design ID.
                        string sql = string.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE {1} = {2}", _PxApp.GetQualifiedTableName(o.TableName), Fields.DesignID, _ID);
                        DataTable table = _PxApp.ExecuteQuery(sql);

                        // Add the table to the dataset.
                        _Dataset.Tables.Add(table);
                    }
                }                
            }
        }

        /// <summary>
        ///     This will save the given node and edm struct data into the correct configured edm tables.
        /// </summary>
        /// <param name="node">IMMWMSNode which can be the WorkRequest, Design, WorkLocation, or CU.</param>
        /// <param name="edm">EDM struct containing the data.</param>
        private void Save(IMMWMSNode node, EDM edm)
        {
            string sql = null;
            EdmTable table = null;

            IMMWMSWorkRequest workRequest = node as IMMWMSWorkRequest;
            if (workRequest != null) // WorkRequest
            {
                table = _EdmRepository.WorkRequest;
                sql = string.Format(CultureInfo.InvariantCulture, "INSERT INTO {0} ({1},{2},{3},{4},{5}) VALUES ({6}, {7},'{8}','{9}','{10}')", "{0}",
                    Fields.WorkRequestID, Fields.DesignID, EDM.Fields.Name, EDM.Fields.Value, EDM.Fields.Type,
                    workRequest.ID, _ID, edm.Name, edm.Value, edm.Type);
            }
            else
            {
                IMMWMSDesign design = node as IMMWMSDesign;
                if (design != null) // Design
                {
                    table = _EdmRepository.Design;
                    sql = string.Format(CultureInfo.InvariantCulture, "INSERT INTO {0} ({1},{2},{3},{4}) VALUES ({5}, {6},'{7}','{8}', '{9}')", "{0}",
                        Fields.DesignID, EDM.Fields.Name, EDM.Fields.Value, EDM.Fields.Type,
                        design.ID, _ID, edm.Name, edm.Value, edm.Type);
                }
                else
                {
                    IMMWMSWorklocation workLocation = node as IMMWMSWorklocation;
                    if (workLocation != null) // WorkLocation
                    {
                        table = _EdmRepository.WorkLocation;
                        sql = string.Format(CultureInfo.InvariantCulture, "INSERT INTO {0} ({1},{2},{3},{4},{5}) VALUES ({6}, {7},'{8}','{9}','{10}')", "{0}",
                            Fields.WorkLocationID, Fields.DesignID, EDM.Fields.Name, EDM.Fields.Value, EDM.Fields.Type,
                            workLocation.ID, _ID, edm.Name, edm.Value, edm.Type);
                    }
                    else
                    {
                        IMMWMSCompatibleUnit compatibleUnit = node as IMMWMSCompatibleUnit;
                        if (compatibleUnit != null) // CompatibleUnit
                        {
                            table = _EdmRepository.CompatibleUnit;
                            sql = string.Format(CultureInfo.InvariantCulture, "INSERT INTO {0} ({1},{2},{3},{4},{5}) VALUES ({6}, {7},'{8}','{9}','{10}')", "{0}",
                                Fields.CompatibleUnitID, Fields.DesignID, EDM.Fields.Name, EDM.Fields.Value, EDM.Fields.Type,
                                compatibleUnit.ID, _ID, edm.Name, edm.Value, edm.Type);
                        }
                    }
                }
            }

            // Insert the EDM when the table is valid.
            if (table != null && table.Valid)
            {
                // Check to see that the field is not being excluded.
                if (table.Fields.Count(o => o.Name.Equals(edm.Name, StringComparison.OrdinalIgnoreCase)) == 0)
                {
                    // Add the EDM record into the table.
                    _PxApp.ExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, sql, _PxApp.GetQualifiedTableName(table.TableName)));
                }
            }
        }

        /// <summary>
        ///     Given the <paramref name="element" /> and the <paramref name="edmTable" /> and
        ///     all of the records that satsify the <paramref name="filter" /> will be added as EDMPROP elements in the document
        ///     from the table.
        /// </summary>
        /// <param name="element">IXMLDOMElement of the current xml document.</param>
        /// <param name="edmTable">The edm table.</param>
        /// <param name="filter">Filter used to narrow down the table search.</param>
        private void SetProperty(IXMLDOMElement element, EdmTable edmTable, string filter)
        {
            // Obtain all of the rows the satisfy the given filter.
            IEnumerable<DataRow> rows = this.GetRows(edmTable, filter);
            if (rows == null) return;

            // Iterate through all of the rows.
            foreach (DataRow row in rows)
            {
                EDM edm = new EDM(row);
                IXMLDOMElement edmprop = element.ownerDocument.createElement("EDMPROP");
                edmprop.setAttribute("Name", edm.Name);
                edmprop.setAttribute("Type", edm.Type);
                edmprop.text = edm.Value;
                element.appendChild(edmprop);
            }
        }

        /// <summary>
        ///     Updates the EDMPROP element with the Extended Data associated with the specified WFM node.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="node">A WFM node object representing a Work Request, Design, Work Location, or Compatible Unit.</param>
        private void UpdateEdm(IXMLDOMElement element, IMMWMSNode node)
        {
            IMMWMSWorkRequest workRequest = node as IMMWMSWorkRequest;
            if (workRequest != null)
            {
                this.SetProperty(element, _EdmRepository.WorkRequest, string.Format(CultureInfo.InvariantCulture, "{0} = {1}", Fields.WorkRequestID, workRequest.ID));
            }
            else
            {
                IMMWMSDesign design = node as IMMWMSDesign;
                if (design != null)
                {
                    this.SetProperty(element, _EdmRepository.Design, string.Format(CultureInfo.InvariantCulture, "{0} = {1}", Fields.DesignID, design.ID));
                }
                else
                {
                    IMMWMSWorklocation workLocation = node as IMMWMSWorklocation;
                    if (workLocation != null)
                    {
                        this.SetProperty(element, _EdmRepository.WorkLocation, string.Format(CultureInfo.InvariantCulture, "{0} = {1}", Fields.WorkLocationID, workLocation.ID));
                    }
                    else
                    {
                        IMMWMSCompatibleUnit compatibleUnit = node as IMMWMSCompatibleUnit;
                        if (compatibleUnit != null)
                        {
                            this.SetProperty(element, _EdmRepository.CompatibleUnit, string.Format(CultureInfo.InvariantCulture, "{0} = {1}", Fields.CompatibleUnitID, compatibleUnit.ID));
                        }
                    }
                }
            }
        }

        #endregion

        #region Nested Type: Fields

        /// <summary>
        ///     The fields names that are required by the underlying storage tables.
        /// </summary>
        internal struct Fields
        {
            #region Constants

            /// <summary>
            ///     The COMPATIBLE_UNIT_ID field name.
            /// </summary>
            public const string CompatibleUnitID = "COMPATIBLE_UNIT_ID";

            /// <summary>
            ///     The DESIGN_ID field name.
            /// </summary>
            public const string DesignID = "DESIGN_ID";

            /// <summary>
            ///     The WORK_LOCATION_ID field name.
            /// </summary>
            public const string WorkLocationID = "WORK_LOCATION_ID";

            /// <summary>
            ///     The WORK_REQUEST_ID field name.
            /// </summary>
            public const string WorkRequestID = "WORK_REQUEST_ID";

            #endregion
        }

        #endregion
    }
}