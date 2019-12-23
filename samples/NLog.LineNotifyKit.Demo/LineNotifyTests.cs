using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NLog.LineNotifyKit.Demo
{
    [TestClass]
    public class LineNotifyTests
    {
        private LogFactory _factory;

        [TestInitialize]
        public void InitSlackSendTests()
        {
            _factory = LogManager.LoadConfiguration("NLog.config");
        }

        [TestMethod]
        public void Send()
        {
            var logger = _factory.GetCurrentClassLogger();

            logger.Info("I test send INFO message");
            //logger.Debug("I test send DEBUG message");
            //logger.Warn("I test send WARN message");
            //logger.Fatal("I test send FATAL message");

            //LineNotifyLogQueue.WaitAsyncCompleted().Wait();
        }
    }
}
