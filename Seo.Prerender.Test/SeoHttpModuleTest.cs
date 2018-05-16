using System.Net.Mail;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seo.Prerender.Helpers;

namespace Seo.Prerender.Test
{
    [TestClass]
    public class SeoHttpModuleTest
    {
        private SeoHttpModule _module;

        [TestInitialize]
        public void TestInitialize()
        {
            _module = new SeoHttpModule();
            ConfigHelper.Initialize(@"D:\Project\DEV\Seo\Seo.Prerender.Test\");
        }

        [TestMethod]
        public void RequestUrlWithEscapedFragmentReturnTrue()
        {
            //arrange
            string escapedfragment = "?_escaped_fragment_ =";
            string requrl = @"https://m-preview.188bet.com/zh-cn";
            var request = new HttpRequest("", requrl, escapedfragment);

            //act
            _module.SetUpConfig(request);
            bool actual = _module.ShouldShowPrerenderedPage(request);

            //assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void RequestUrlReturnCorrectFile()
        {
            //arrange
            string expeted = ConfigHelper.PreRenderSetting.GetDomainSetting("default").AccessPath + @"/zh-cn.html";
            string requrl =  @"https://m-preview.188bet.com/zh-cn";
            var request = new HttpRequest("", requrl, "");

            //act
            _module.SetUpConfig(request);
            string actual = _module.GetPrerenderedPagePath(request);

            //assert
            Assert.AreEqual(expeted, actual);
        }

        [TestMethod]
        public void RequestUrlWithQueryReturnCorrectFilePath()
        {
            //arrange
            string expeted = ConfigHelper.PreRenderSetting.GetDomainSetting("default").AccessPath + @"/en-gb/infocenter/staticpage_actionpage=aboutus.html";
            string requrl = @"https://www.toutou.com/en-gb/infocenter/staticpage?actionpage=aboutus";
            var request = new HttpRequest("", requrl, "");

            //act
            _module.SetUpConfig(request);
            string actual = _module.GetPrerenderedPagePath(request);

            //assert
            Assert.AreEqual(expeted, actual);
        }

        [TestMethod]
        public void RequestUrlWithEscapedFragmentReturnCorrectFile()
        {
            //arrange
            var expeted = ConfigHelper.PreRenderSetting.GetDomainSetting("default").AccessPath + @"/zh-cn.html";
            var requrl = @"https://m-preview.188bet.com/zh-cn?_escaped_fragment_=";
            var request = new HttpRequest("", requrl, "");

            //act
            _module.SetUpConfig(request);
            var actual = _module.GetPrerenderedPagePath(request);

            //assert
            Assert.AreEqual(expeted, actual);
        }

        [TestMethod]
        public void TestSendMail()
        {
            var msg = new MailMessage() ;
            msg.From = new MailAddress("andy.chen@xuenn.com");
            msg.To.Add("andy.chen@xuenn.com");
            msg.Body = "test";
            MailHelper.SendMail(msg);
        }
    }
}
