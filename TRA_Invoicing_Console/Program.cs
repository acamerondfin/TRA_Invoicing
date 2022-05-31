using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRA_Invoicing_Console
{
    class Program
    {
        protected static string outputDirectory = ConfigurationManager.AppSettings.Get("OutputDirectory");
        protected static string clientCycle = ConfigurationManager.AppSettings.Get("ClientCycle").ToString();
        protected static string DBName = ConfigurationManager.AppSettings.Get("DatabaseName");
        protected static string FundCoInvoice = ConfigurationManager.AppSettings.Get("FundCoInvoice");
        protected static string InsCoInvoice = ConfigurationManager.AppSettings.Get("InsCoInvoice");
 
        static void Main(string[] args)
        {
            Console.WriteLine("Processing...please wait");
            string returnMsg = string.Empty;
            DateTime now = DateTime.Now;
            string formattedDate = now.ToString("MMddyyyy_hhmmss");
            string configInfo = "";
            string path = "";

            var dict =
                ConfigurationManager.AppSettings.AllKeys
                    .ToDictionary(k => k, v => ConfigurationManager.AppSettings[v]);

            foreach (var item in dict)
            {
                configInfo += item.Key.ToString() + " = " + item.Value.ToString() + System.Environment.NewLine;
            }


            returnMsg = UpdateBaseData.UpdateAllInfo();
            if (returnMsg.Length == 0)
                Console.WriteLine("Updated base data successfully...");

            if (FundCoInvoice.ToUpper() == "YES")
            {
                returnMsg = FundCompanyInvoice.CreateInvoice();
                if (returnMsg.Length == 0)
                    Console.WriteLine("Created Fund Company invoice successfully...");

                path = outputDirectory + @"\" + DBName + "_fundco_" + formattedDate + "_log.txt";


                if (returnMsg.Length > 0)
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    File.WriteAllText(path.Replace("_log.txt", "_ERROR_log.txt"), "---ERROR---" +
                                System.Environment.NewLine + returnMsg +
                                    System.Environment.NewLine +
                                    System.Environment.NewLine +
                                    "********************************************************************************" +
                                    System.Environment.NewLine +
                                    "Config file settings:" +
                                    System.Environment.NewLine +
                                    configInfo +
                                    System.Environment.NewLine +
                                     "********************************************************************************");
                }
                else File.WriteAllText(path, DBName + " successful" +
                    System.Environment.NewLine +
                    System.Environment.NewLine +
                    "********************************************************************************" +
                    System.Environment.NewLine +
                    "Config file settings:" +
                    System.Environment.NewLine +
                    configInfo +
                    System.Environment.NewLine +
                     "********************************************************************************");
            }

            if (InsCoInvoice.ToUpper() == "YES")
            {

                returnMsg = InsuranceCompanyInvoice.CreateInvoice();
                if (returnMsg.Length == 0)
                    Console.WriteLine("Created Insurance Company invoice successfully...");

                path = outputDirectory + @"\" + DBName + "_insco_" + formattedDate + "_log.txt";

                if (returnMsg.Length > 0)
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    File.WriteAllText(path.Replace("_log.txt", "_ERROR_log.txt"), "---ERROR---" +
                        System.Environment.NewLine + returnMsg +
                            System.Environment.NewLine +
                            System.Environment.NewLine +
                            "********************************************************************************" +
                            System.Environment.NewLine +
                            "Config file settings:" +
                            System.Environment.NewLine +
                            configInfo +
                            System.Environment.NewLine +
                             "********************************************************************************");
                }
                else File.WriteAllText(path, DBName + " successful" +
                    System.Environment.NewLine +
                    System.Environment.NewLine +
                    "********************************************************************************" +
                    System.Environment.NewLine +
                    "Config file settings:" +
                    System.Environment.NewLine +
                    configInfo +
                    System.Environment.NewLine +
                     "********************************************************************************");
            }
        }
    }
}
