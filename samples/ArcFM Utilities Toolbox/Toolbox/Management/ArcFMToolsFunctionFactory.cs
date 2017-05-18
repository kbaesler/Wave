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
            : base("ArcFM Properties Tools", "ArcFM Properties Tools")
        {
            this.Add(new AddClassModelName(this), "Feature Class");
            this.Add(new RemoveClassModelName(this), "Feature Class");
            this.Add(new AddSpecialAU(this), "Feature Class");
            this.Add(new RemoveSpecialAU(this), "Feature Class");

            this.Add(new AddFieldModelName(this), "Field");
            this.Add(new RemoveFieldModelName(this), "Field");
            this.Add(new RemoveAttributeAU(this), "Field");
            this.Add(new AddAttributeAU(this), "Field");

            this.Add(new RemoveRelationshipAU(this), "Relationship");
            this.Add(new AddRelationshipAU(this), "Relationship");
        }

        #endregion

        #region Private Methods

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

        #endregion
    }
}