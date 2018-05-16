using System.IO;
using System.Web;
using log4net;
using Seo.Prerender.Models;

namespace Seo.Prerender.Helpers
{
    internal class ConfigHelper
    {
        public static PreRenderSetting PreRenderSetting { get; set; }
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConfigHelper));

        public static void Initialize(string rootFolderPath)
        {
            PreRenderSetting = XmlSerializerHelper.ToObj<PreRenderSetting>(GetXml(rootFolderPath,"Prerender.config"));
            MailHelper.Initialize(PreRenderSetting.MailNotifySetting);
        }

        private static string GetXml(string folderPath, string fileName)
        {
            return GetXml(string.Concat(folderPath, fileName));
        }

        private static string GetXml(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else
            {
                Log.Debug("Config File " + filePath + " doesn't existed at all!");
                return null;
            }
        }
    }
}
