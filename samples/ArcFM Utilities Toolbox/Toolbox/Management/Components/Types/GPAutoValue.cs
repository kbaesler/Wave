using System;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;

using Miner.Interop;

namespace Wave.Geoprocessing.Toolbox.Management
{
    public interface IGPAutoValue
    {
        #region Public Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        string Name { get; }

        /// <summary>
        ///     Gets the uid.
        /// </summary>
        /// <value>
        ///     The uid.
        /// </value>
        IUID UID { get; }

        #endregion
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="ESRI.ArcGIS.Geodatabase.IGPValue" />
    public class GPAutoValue<TValue> : IGPValue, IGPAutoValue, IGPString
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GPAutoValue{TValue}" /> class.
        /// </summary>
        internal GPAutoValue()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GPAutoValue{TValue}" /> class.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public GPAutoValue(IUID uid)
        {
            this.UID = uid;
            this.Value = uid.Create<TValue>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The data type of the value object.
        /// </summary>
        public IGPDataType DataType
        {
            get { return new GPAutoValueType<TValue>(); }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name
        {
            get { return this.GetAsText(); }
        }

        /// <summary>
        ///     Gets or sets the uid.
        /// </summary>
        /// <value>
        ///     The uid.
        /// </value>
        public IUID UID { get; private set; }

        

        #endregion
        
        #region Private Properties

        /// <summary>
        ///     Gets or sets the type of the target.
        /// </summary>
        /// <value>
        ///     The type of the target.
        /// </value>
        public TValue Value { get; set; }

        #endregion

        #region IGPValue Members

        /// <summary>
        ///     Clears the value object.
        /// </summary>
        public void Empty()
        {
            this.UID = null;
            this.Value = default(TValue);
        }

        /// <summary>
        ///     Provides the value of the value object.
        /// </summary>
        /// <returns></returns>
        public string GetAsText()
        {
            IMMSpecialAUStrategyEx s = this.Value as IMMSpecialAUStrategyEx;
            if (s != null) return s.Name;

            IMMRelationshipAUStrategy r = this.Value as IMMRelationshipAUStrategy;
            if (r != null) return r.Name;

            IMMAttrAUStrategy a = this.Value as IMMAttrAUStrategy;
            if (a != null) return a.Name;

            IMMExtObject e = this.Value as IMMExtObject;
            if (e != null) return e.Name;

            return "UNREGISTERED PROGRAM";
        }

        /// <summary>
        ///     Indicates if the value object is empty.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return this.UID == null;
        }

        /// <summary>
        ///     Provides the value of the value object with the given string value.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IGPMessage SetAsText(string text)
        {
            IGPMessage message = new GPMessageClass();

            try
            {
                this.UID = new UIDClass()
                {
                    Value = text
                };

                this.Value = this.UID.Create<TValue>();
            }
            catch (Exception e)
            {
                message.Type = esriGPMessageType.esriGPMessageTypeError;
                message.Description = e.Message;
                message.ErrorCode = 500;
            }

            return message;
        }

        #endregion

        string IGPString.Value
        {
            get { return this.GetAsText(); }
            set { this.SetAsText(value); }
        }
    }
}