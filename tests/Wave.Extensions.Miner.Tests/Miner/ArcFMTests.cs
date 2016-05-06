using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner;

namespace Wave.Extensions.Miner.Tests.Miner
{
    [TestClass]
    public class ArcFMTests
    {
        [TestMethod]
        public void ArcFM_Version()
        {
            Assert.IsNotNull(ArcFM.Version);
        }
       
        [TestMethod]
        public void ArcFM_BuildNumber()
        {
            Assert.IsNotNull(ArcFM.BuildNumber);
        }
    }
}
