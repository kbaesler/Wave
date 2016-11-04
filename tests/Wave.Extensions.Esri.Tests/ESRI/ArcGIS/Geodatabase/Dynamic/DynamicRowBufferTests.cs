using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wave.Extensions.Esri.Tests;

namespace ESRI.ArcGIS.Geodatabase.Dynamic.Tests
{
    [TestClass]
    public class DynamicRowBufferTests : EsriTests
    {
        #region Public Methods

        [TestMethod]
        public void DynamicRowBuffer_Get_Field_Exposed_As_Member()
        {
            var table = base.GetTestClass();
            dynamic d = null;
            var oid = 0;

            table.Fetch(null, row =>
            {
                oid = row.OID;
                d = row.ToDynamic();
                return false;
            });

            Assert.AreEqual(d.ObjectId, oid);
        }

        [TestMethod]
        public void DynamicRowBuffer_Set_Field_Exposed_As_Member()
        {
            var table = base.GetTestClass();
            dynamic d = null;
            object user = null;

            table.Fetch(null, row =>
            {
                user = row.Value[row.Fields.FindField("LASTUSER")];
                d = row.ToDynamic();
                return false;
            });

            d.LastUser = "SCOTT";

            Assert.AreNotEqual(d.LastUser, user);
        }

        [TestMethod]
        [ExpectedException(typeof (KeyNotFoundException))]
        public void DynamicRowBuffer_Update_A_Field_That_Is_Not_Defined_Exposed_As_Member()
        {
            var table = base.GetTestClass();
            dynamic d = null;

            table.Fetch(null, row =>
            {
                d = row.ToDynamic();
                return false;
            });

            d.AttemptToUpdateAFieldThatIsNotDefined = true;
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void DynamicRowBuffer_Update_Non_Editable_Field_Exposed_As_Member()
        {
            var table = base.GetTestClass();
            dynamic d = null;

            table.Fetch(null, row =>
            {
                d = row.ToDynamic();
                return false;
            });

            d.ObjectID = 123;
        }

        #endregion
    }
}