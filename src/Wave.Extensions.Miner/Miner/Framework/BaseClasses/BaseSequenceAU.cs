using System;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;

namespace Miner.Framework
{
    /// <summary>
    ///     An abstract class for an Special AU that will query the next value in the specified Oracle Sequence when the AU is
    ///     triggered.
    ///     The resulting value can be formatted using the the abstract methods.
    /// </summary>
    public abstract class BaseSequenceAU : BaseSpecialAU
    {
        #region Fields

        private readonly string _FieldModelName;
        private readonly string _SequenceName;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseSequenceAU" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sequenceName">Name of the sequence.</param>
        /// <param name="fieldModelName">The field model name.</param>
        protected BaseSequenceAU(string name, string sequenceName, string fieldModelName)
            : base(name)
        {
            _SequenceName = sequenceName;
            _FieldModelName = fieldModelName;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Formats the sequence value for the specified <paramref name="obj" />
        /// </summary>
        /// <param name="value">The value of the sequence.</param>
        /// <param name="obj">
        ///     The object that triggered the <see cref="InternalExecute(IObject, mmAutoUpdaterMode, mmEditEvent)" />
        ///     method.
        /// </param>
        /// <returns>
        ///     A string in the format that is will be stored on the field.
        /// </returns>
        protected abstract string Format(int value, IObject obj);

        /// <summary>
        ///     Implementation of Auto Updater Enabled method for derived classes.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        ///     <c>true</c> if the AutoUpdater should be enabled; otherwise <c>false</c>
        /// </returns>
        /// <remarks>
        ///     This method will be called from IMMSpecialAUStrategy::get_Enabled
        ///     and is wrapped within the exception handling for that method.
        /// </remarks>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent editEvent)
        {
            if (editEvent != mmEditEvent.mmEventFeatureCreate)
                return false;

            return objectClass.IsAssignedFieldModelName(_FieldModelName);
        }

        /// <summary>
        ///     Implementation of Auto Updater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the Auto Udpater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <exception cref="NotSupportedException">
        ///     The sequence generator is only supported on an ORACLE workspace (remote
        ///     geodatabase).
        /// </exception>
        /// <exception cref="ArgumentNullException">obj;@The field model name is not assigned on the object.</exception>
        /// <remarks>
        ///     This method will be called from IMMSpecialAUStrategy::ExecuteEx
        ///     and is wrapped within the exception handling for that method.
        /// </remarks>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent editEvent)
        {
            IDataset dataset = (IDataset) obj.Class;
            IWorkspace workspace = dataset.Workspace;
            if (workspace.IsDBMS(DBMS.Oracle))
                throw new NotSupportedException("The sequence generator is only supported on an ORACLE workspace (remote geodatabase).");

            string fieldName = obj.Class.GetFieldName(_FieldModelName);
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException("obj", @"The field model name is not assigned on the object.");

            // Create a queryDef from the feature workspace
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace) workspace;
            IQueryDef queryDef = featureWorkspace.CreateQueryDef();

            // Set the query def to point to the sequence
            queryDef.SubFields = _SequenceName + ".NEXTVAL";
            queryDef.Tables = "SYS.DUAL";

            // Define a cursor and row, for destroy in finally
            using (ComReleaser cr = new ComReleaser())
            {
                // Fill the cursor via the query def
                ICursor cursor = queryDef.Evaluate();
                cr.ManageLifetime(cursor);

                // Now get the row from the cursor
                IRow row = cursor.NextRow();
                if (row == null) return;

                // Store the formatted value if it's configured.
                int val = TypeCast.Cast(row.get_Value(0), -1);
                string formattedValue = this.Format(val, obj);

                int pos = obj.Class.FindField(fieldName);
                obj.set_Value(pos, formattedValue);
            }
        }

        #endregion
    }
}