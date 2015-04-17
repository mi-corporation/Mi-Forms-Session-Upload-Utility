using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiCo.MiForms.Uploader
{
    public class Upload
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            var msg = string.Empty;            
            Log.Info("Starting Mi-Forms Session Upload Processor");

            // begin upload process
            SessionUpload su = new SessionUpload();

            Log.Info(" Mi-Forms Session Upload Processor is Complete!");

        }
    }
}
