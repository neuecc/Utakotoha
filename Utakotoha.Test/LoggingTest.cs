using System;
using System.IO.IsolatedStorage;
using System.IO.IsolatedStorage.Moles;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utakotoha.Test
{
    [TestClass]
    public class LoggingTest
    {
        [TestMethod, HostType("Moles")]
        public void WriteReadDelete()
        {
            MIsolatedStorageFile.GetUserStoreForApplication = () => IsolatedStorageFile.GetUserStoreForDomain();
            Logging.DebugWrite("aaa");
            Logging.DebugWrite("bbb");
            Logging.DebugWrite("ccc");

            var data = Logging.ReadData();
            data.IsNotNull();

            data.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .Where(s => s != "")
                .Select(s => s.Split('|')[1])
                .Is("aaa", "bbb", "ccc");

            Logging.DeleteData();

            Logging.ReadData().IsNull();
        }
    }
}
