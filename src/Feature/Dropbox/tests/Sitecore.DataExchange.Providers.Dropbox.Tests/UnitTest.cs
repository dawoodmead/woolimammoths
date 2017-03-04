using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.DataExchange.Providers.Dropbox.Helpers;

namespace Sitecore.DataExchange.Providers.Dropbox.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void CheckUrl()
        {
            var url = DownloadAndUnzipHelper.FixLink("http://sdf");
            Assert.AreEqual(url, "http://sdf/?dl=1");
        }
    }
}
