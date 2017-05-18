using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;

namespace Wave.Geoprocessing.Toolbox.Management
{
    [Guid("A74D7FC6-8686-4DE4-B17F-68CBAA05EDD2")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Wave.Geoprocessing.GPAutoValueDataTypeFactory")]
    public class GPAutoValueDataTypeFactory<TValue> : IGPDataTypeFactory
    {
        #region Public Properties

        /// <summary>
        /// The COM class ID of the data type factory.
        /// </summary>
        public UID CLSID
        {
            get
            {
                UID uid = new UIDClass();
                uid.Value = this.GetType().GUID.ToString("B");

                return uid;
            }
        }

        #endregion

        #region IGPDataTypeFactory Members

        /// <summary>
        /// Provides the data type object given the name.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IGPDataType GetDataType(string Name)
        {
            IGPDataType dataType = null;
            switch (Name)
            {
                case "GPAutoValueDataType":
                    dataType = new GPAutoValueType<TValue>();
                    break;
            }

            return dataType;
        }

        /// <summary>
        /// Provides the data type name object given the name.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IGPName GetDataTypeName(string Name)
        {
            IGPName name = new GPDataTypeNameClass();
            name.Name = Name;
            name.DisplayName = Name;
            name.Factory = this;

            return name;
        }

        /// <summary>
        /// Provides the enumeration of data type name objects.
        /// </summary>
        /// <returns></returns>
        public IEnumGPName GetDataTypeNames()
        {
            IArray names = new EnumGPNameClass();
            names.Add(this.GetDataType("GPAutoValueDataType"));

            return (IEnumGPName) names;
        }

        #endregion

        #region "Component Category Registration"

        [ComRegisterFunction()]
        static void Reg(string regKey)
        {
            GPDataTypeFactories.Register(regKey);
        }

        [ComUnregisterFunction()]
        static void Unreg(string regKey)
        {
            GPDataTypeFactories.Unregister(regKey);
        }

        #endregion
    }
}