using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner.Interop;
using Miner.Interop.Process;

namespace Wave.Extensions.Miner.Tests
{
    /// <summary>
    /// The EDM repository for the MINERVILLE sample data.
    /// </summary>
    internal class TestPxEdmRepository : BasePxEdmRepository
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TestPxEdmRepository" /> class.
        /// </summary>
        /// <param name="pxApplication">The process framework application reference.</param>
        public TestPxEdmRepository(IMMPxApplication pxApplication)
            : base(pxApplication)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the configurations used to populate the EDM skeleton tables.
        /// </summary>
        /// <param name="pxApplication">The process framework application reference.</param>
        /// <returns>
        ///     Returns the <see cref="T:Miner.Interop.Process.EdmRepository" /> representing the EDM configurations; otherwise
        ///     <c>null</c>.
        /// </returns>
        protected override EdmRepository GetRepository(IMMPxApplication pxApplication)
        {
            EdmRepository repository = new EdmRepository();
            repository.Type = "2";
            repository.Design = new EdmTable()
            {
                TableName = "EDM_DESIGN",
                Fields = new[] {new EdmField() {Name = "STATE"}}
            };
            repository.WorkLocation = new EdmTable()
            {
                TableName = "EDM_WORK_LOCATION",
                Fields = new[] {new EdmField() {Name = "STATE"}}
            };

            return repository;
        }

        #endregion
    }

    [TestClass]
    public class IPxEdmRepositoryTest : ProcessTests
    {
        #region Constructors

        public IPxEdmRepositoryTest()
            : base(mmProductInstallation.mmPIDesigner)
        {
        }

        #endregion

        #region Public Methods

        [TestMethod]
        public void IPxEdmRepository_GetEdms_Count_Equals_0()
        {
            IPxEdmRepository repository = new TestPxEdmRepository(base.PxApplication);
            var list = repository.GetEdms(1);

            Assert.IsTrue(list.Count == 0);
        }

        #endregion
    }
}