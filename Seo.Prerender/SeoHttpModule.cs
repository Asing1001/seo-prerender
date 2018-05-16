using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Timers;
using System.Web;
using log4net;
using Seo.Prerender.Helpers;
using Seo.Prerender.Models;

namespace Seo.Prerender
{
    public class SeoHttpModule : IHttpModule
    {
        private const string EscapedFragment = "_escaped_fragment_";
        private HttpApplication _application;
        private PreRenderSetting _prerenderSetting;
        private DomainSetting _domainSetting;

        private static bool _hasRegisterFolderCheck;
        private static readonly ILog Log = LogManager.GetLogger(typeof(SeoHttpModule));

        public void Dispose()
        {

        }

        public void Init(HttpApplication application)
        {
            try
            {
                this._application = application;
                application.BeginRequest += context_BeginRequest;
                string rootFolderPath = HttpContext.Current.Server.MapPath("\\");
                ConfigHelper.Initialize(rootFolderPath);
                if (!_hasRegisterFolderCheck)
                {
                    RegisterCrawlerCheck();
                    DoCrawlerCheck(null, null);
                    _hasRegisterFolderCheck = true;
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        private void RegisterCrawlerCheck()
        {
            var timer = new Timer(new TimeSpan(1, 0, 0, 0).TotalMilliseconds);
            timer.Elapsed += DoCrawlerCheck;
            timer.Start();
        }

        private void DoCrawlerCheck(object sender, ElapsedEventArgs e)
        {
            string yesterdaySitemapPath = string.Format("{0}/{1}sitemap.xml",
                ConfigHelper.PreRenderSetting.GetDomainSetting("default").AccessPath,
                DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy"));
            bool crawlerRunYesterday = File.Exists(yesterdaySitemapPath);
            if (!crawlerRunYesterday)
            {
                string notifyMsgBody = "Cralwer Not Work! Can not find yesterday sitemap at: " +
                                       yesterdaySitemapPath;
                if (ConfigHelper.PreRenderSetting.MailNotifySetting.Enable)
                {
                    SendCrawlerNotRunMail(notifyMsgBody);
                }
                else
                {
                    Log.Error(notifyMsgBody);
                }
            }
        }

        private void SendCrawlerNotRunMail(string msgBody)
        {
            var mailSetting = ConfigHelper.PreRenderSetting.MailNotifySetting;
            var mail = new MailMessage();
            mail.From = new MailAddress(mailSetting.Sender);
            mail.To.Add(mailSetting.Recipients);
            mail.Subject = "Cralwer Not Work!!";
            mail.Body = msgBody;
            MailHelper.SendMail(mail);
        }

        private void context_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                //different request will use different configuration 
                SetUpConfig(_application.Request);
                DoPrerender(_application);
            }
            catch (Exception exception)
            {
                var request = _application.Request;
                Log.Error(exception);
                Log.Error(string.Format("Seo DoPrerender throw error: request url:{0}, useragent:{1}, contentType:{2}", request.Url, request.UserAgent, request.ContentType));
            }
        }

        protected internal void SetUpConfig(HttpRequest request)
        {
            _prerenderSetting = ConfigHelper.PreRenderSetting;
            _domainSetting = ConfigHelper.PreRenderSetting.GetDomainSetting(request.Url.Authority);
        }

        private void DoPrerender(HttpApplication application)
        {
            var httpContext = application.Context;
            var request = httpContext.Request;
            var response = httpContext.Response;
            if (ShouldShowPrerenderedPage(request))
            {
                var filename = GetPrerenderedPagePath(request);
                var fileInfo = new FileInfo(filename);
                if (fileInfo.Exists)
                {
                    response.ContentType = "text/html";
                    response.WriteFile(filename);
                    application.CompleteRequest(); //directly end request
                }
            }
        }

        internal bool ShouldShowPrerenderedPage(HttpRequest request)
        {
            var userAgent = request.UserAgent ?? string.Empty;
            bool isAjaxRequest = request.AcceptTypes != null && request.AcceptTypes.Contains("application/json");
            bool isStaticFile = Path.HasExtension(request.FilePath);
            bool isByPassRequest =
                _domainSetting.ByPassPathList.Any(
                    byPassPath =>
                        byPassPath.Equals(request.Url.AbsolutePath, StringComparison.InvariantCultureIgnoreCase));
            if ((HasEscapedFragment(request) || IsInSearchUserAgent(userAgent)) && !isAjaxRequest && !isStaticFile && !isByPassRequest)
            {
                return true;
            }

            return false;
        }

        internal string GetPrerenderedPagePath(HttpRequest request)
        {
            //e.g./en-gb/poker
            var uri = request.Url;
            //e.g. @"D:\SeoPrerenderPage\en-gb\.html";
            string fileNameWithFullPath = string.Format(@"{0}{1}{2}.html", _domainSetting.AccessPath, uri.AbsolutePath, MakeValidFileName(uri.Query));
            return fileNameWithFullPath;
        }

        private string MakeValidFileName(string queryString)
        {
            if (queryString.Contains(EscapedFragment))
            {
                queryString = queryString.Replace("?" + EscapedFragment + "=", "");
                queryString = queryString.Replace("&" + EscapedFragment + "=", "");
            }
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(queryString, invalidRegStr, "_");
        }


        private bool IsInSearchUserAgent(string userAgent)
        {
            var crawlerUserAgents = GetCrawlerUserAgents();

            return
                (crawlerUserAgents.Any(
                    crawlerUserAgent =>
                    userAgent.IndexOf(crawlerUserAgent, StringComparison.InvariantCultureIgnoreCase) >= 0));
        }

        private IEnumerable<String> GetCrawlerUserAgents()
        {
            var crawlerUserAgents = new List<string>(new[]
                {
                    "GoogleBot", "Applebot","bingbot", "baiduspider", "facebookexternalhit", "twitterbot", "yandex", "rogerbot",
                    "linkedinbot", "embedly", "bufferbot", "quora link preview", "showyoubot", "outbrain"
                });

            //can add CrawlerUserAgents detection in web.config seo section
            if (_prerenderSetting.CrawlerUserAgents.IsNotEmpty())
            {
                crawlerUserAgents.AddRange(_prerenderSetting.CrawlerUserAgentList);
            }
            return crawlerUserAgents;
        }

        private bool HasEscapedFragment(HttpRequest request)
        {
            return request.QueryString.AllKeys.Contains(EscapedFragment) || request.QueryString.ToString().Contains(EscapedFragment);
        }

    }
}
