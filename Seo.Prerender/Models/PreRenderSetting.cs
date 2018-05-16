using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using log4net;

namespace Seo.Prerender.Models
{
    [Serializable]
    public class PreRenderSetting
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PreRenderSetting));
        public string CrawlerUserAgents { get; set; }
        public List<DomainSetting> DomainSettings { get; set; }
        public MailNotifySetting MailNotifySetting { get; set; }
        public IEnumerable<string> CrawlerUserAgentList
        {
            get { return CrawlerUserAgents.IsBlank() ? null : CrawlerUserAgents.Trim().Split(','); }
        }

        public DomainSetting GetDomainSetting(string hostWithPort)
        {
            DomainSetting domainSetting = null;
            try
            {
                domainSetting =
                    DomainSettings.FirstOrDefault(
                        domainsetting => domainsetting.Domain.Equals(hostWithPort, StringComparison.InvariantCultureIgnoreCase));
                if (domainSetting == null)
                {
                    domainSetting = DomainSettings.First(
                        domainsetting =>
                            domainsetting.Domain.Equals("default", StringComparison.InvariantCultureIgnoreCase)); ;
                }
            }
            catch(Exception ex)
            {
                Log.Error("GetDomainSetting throw exception, might forget to set domain default", ex);
            }
            return domainSetting;
        }
    }

    public class MailNotifySetting
    {
        [XmlAttribute]
        public string MailServer { get; set; }
        [XmlAttribute]
        public int Port { get; set; }
        [XmlAttribute]
        public string Recipients { get; set; }
        [XmlAttribute]
        public string Sender { get; set; }
        [XmlAttribute]
        public bool Enable { get; set; }
    }

    public class DomainSetting
    {
        [XmlAttribute]
        public string Domain { get; set; }

        [XmlAttribute]
        public string AccessPath { get; set; }

        [XmlAttribute]
        public string ByPassPaths { get; set; }

        public IEnumerable<string> ByPassPathList
        {
            get { return ByPassPaths.IsBlank() ? null : ByPassPaths.Trim().Split(','); }
        }
    }
}