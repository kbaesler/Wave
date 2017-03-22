using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Geoprocessing;

using Miner.ComCategories;

namespace Wave.Geoprocessing.Toolbox.Management
{
    /// <summary>
    ///     A function factory used to group the tools or functions together in logical sets.
    /// </summary>
    [ComponentCategory(ComCategory.GeoprocessorFunctionFactory)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    [Guid("F542D575-6B7E-4059-A33A-4F471992AE36")]
    [ProgId("Wave.Geoprocessing.Toolbox.Management.ArcFMToolsFunctionFactory")]
    public class ArcFMToolsFunctionFactory : BaseFunctionFactory
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ArcFMToolsFunctionFactory" /> class.
        /// </summary>
        public ArcFMToolsFunctionFactory()
            : base("ArcFM Properties Management Tools", "ArcFM Properties Management Tools")
        {
            this.Functions.Add(new AddClassModelNameFunction(this), "Feature Class");
            this.Functions.Add(new RemoveClassModelNameFunction(this), "Feature Class");          

            this.Functions.Add(new AddFieldModelNameFunction(this), "Field");
            this.Functions.Add(new RemoveFieldModelNameFunction(this), "Field");

            this.Functions.Add(new RemoveAttributeAutoUpdaterFunction(this), "Field");
            this.Functions.Add(new RemoveSpecialAutoUpdaterFunction(this), "Feature Class");
            this.Functions.Add(new RemoveRelationshipAutoUpdaterFunction(this), "Relationship");
        }

        #endregion


        [ComRegisterFunction()]
        static void Reg(string regKey)
        {
            GPFunctionFactories.Register(regKey);
        }

        [ComUnregisterFunction()]
        static void Unreg(string regKey)
        {
            GPFunctionFactories.Unregister(regKey);
        }

    }
}