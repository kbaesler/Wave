using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;

namespace Wave.Geoprocessing.Toolbox.Management
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Geoprocessing.IGPStringType"/>
    public interface IGPAutoValueType 
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="ESRI.ArcGIS.Geodatabase.IGPDataType" />
    /// <seealso cref="Wave.Geoprocessing.Toolbox.Management.IGPAutoValueType" />
    [Guid("583B4992-05BD-4E6C-97AB-C90F8FF735FB")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Wave.Geoprocessing.GPAutoValueType")]
    public class GPAutoValueType<TValue> : IGPDataType, IGPAutoValueType
    {
        #region Constants

        private const string DataTypeName = "GPAutoValueDataType";

        #endregion

        #region Fields

        private readonly IGPDataTypeFactory _Factory = new GPAutoValueDataTypeFactory<TValue>();

        #endregion

        #region Public Properties

        /// <summary>
        /// The COM class id of the data type.
        /// </summary>
        public UID ControlCLSID
        {
            get { return new GPStringTypeClass().ControlCLSID; }
        }

        /// <summary>
        /// The descriptive, user-friendly name.
        /// </summary>
        public string DisplayName
        {
            get { return DataTypeName; }
        }

        /// <summary>
        /// The associated Name object.
        /// </summary>
        public IName FullName
        {
            get { return _Factory.GetDataTypeName(this.Name) as IName; }
        }

        /// <summary>
        /// The context identifier of the topic within the help file.
        /// </summary>
        public int HelpContext
        {
            get { return 0; }
        }

        /// <summary>
        /// The name of the (CHM) file containing help information.
        /// </summary>
        public string HelpFile
        {
            get { return null; }
        }

        /// <summary>
        /// The name of the (XML) file containing the default metadata for this data type.
        /// </summary>
        public string MetadataFile
        {
            get { return string.Format("{0}.xml", DataTypeName); }
        }

        /// <summary>
        /// The name of the data type.
        /// </summary>
        public string Name
        {
            get { return DataTypeName; }
        }

        #endregion

        #region IGPDataType Members

        /// <summary>
        /// Creates a geoprocessing value object from the given string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IGPValue CreateValue(string text)
        {
            IGPValue value = new GPAutoValue<TValue>();
            IGPMessage message = value.SetAsText(text);

            if (message.IsInformational())
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Validates the type of the data.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IGPMessage ValidateDataType(IGPDataType type)
        {
            IGPMessage message = new GPMessageClass();
            IGPAutoValueType targetType = type as IGPAutoValueType;

            if (targetType == null)
            {
                message.ErrorCode = 501;
                message.Type = esriGPMessageType.esriGPMessageTypeError;
                message.Description = @"The value is not an auto value.";
            }

            return message;
        }

        /// <summary>
        /// Validates the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="domain">The domain.</param>
        /// <returns></returns>
        public IGPMessage ValidateValue(IGPValue value, IGPDomain domain)
        {
            IGPMessage message = new GPMessageClass();
            IGPUtilities3 utilities = new GPUtilitiesClass();

            IGPAutoValue autoValue = utilities.UnpackGPValue(value) as IGPAutoValue;           
            if (autoValue == null)
            {
                message.Type = esriGPMessageType.esriGPMessageTypeError;
                message.ErrorCode = 502;
                message.Description = @"The value is not an auto value.";
            }

            return message;
        }

        #endregion
    }
}