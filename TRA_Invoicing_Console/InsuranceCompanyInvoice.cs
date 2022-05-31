using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
//using System.Drawing;
using Microsoft.Reporting.WinForms;
using System.Reflection;
using System.Diagnostics;


namespace TRA_Invoicing_Console
{
    class InsuranceCompanyInvoice
    {
        protected static string DBName = ConfigurationManager.AppSettings.Get("DatabaseName");
        protected static string DBName_Base = ConfigurationManager.AppSettings.Get("Base_DatabaseName");
        protected static string AppName = ConfigurationManager.AppSettings.Get("AppName");
        protected static string ConfigDBName = ConfigurationManager.AppSettings.Get("ConfigDBName");
        protected static string SQLServerName = ConfigurationManager.AppSettings.Get("SQLServerName");
        protected static string SQLUserName = ConfigurationManager.AppSettings.Get("SQLServerUserName");
        protected static string SQLPwd = ConfigurationManager.AppSettings.Get("SQLServerPassword");

        protected static int startNbr = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Invoice_StartNumber").ToString());
        protected static string clientCycle = ConfigurationManager.AppSettings.Get("ClientCycle").ToString();

        protected static string mainViewName = ConfigurationManager.AppSettings.Get("MainViewName");
        protected static string detailViewName = ConfigurationManager.AppSettings.Get("DetailViewName");

        protected static string outputDirectory = ConfigurationManager.AppSettings.Get("OutputDirectory");
        protected static string outputFileName = ConfigurationManager.AppSettings.Get("InsCoInvoice_OutputFileName");


        public static string sqlConnString = "";
        public static string sqlConnStringBase = "";
        protected static DataSet dsInvoiceData = new DataSet();
   //     protected static ReportViewer reportViewer1 = new ReportViewer();

        public static string CreateInvoice()
        {
            sqlConnString = "user id=" + SQLUserName + ";" +
                               "password=" + SQLPwd + ";" +
                               "server=" + SQLServerName + ";" +
                               "Trusted_Connection=false;" +
                               "database=" + DBName + "; " +
                               "connection timeout=30; " +
                               "MultipleActiveResultSets=true";

            sqlConnStringBase = "user id=" + SQLUserName + ";" +
                               "password=" + SQLPwd + ";" +
                               "server=" + SQLServerName + ";" +
                               "Trusted_Connection=false;" +
                               "database=" + DBName_Base + "; " +
                               "connection timeout=30; " +
                               "MultipleActiveResultSets=true";

            string returnMsg = string.Empty;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(
                                sqlConnStringBase))
                {
                    sqlConnection.Open();


                    using (var command = new SqlCommand("CreateInvoiceInfo_I_View", sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        command.Parameters.Add("@DBName", SqlDbType.VarChar);
                        command.Parameters["@DBName"].Value = DBName;
                        command.Parameters.Add("@InvoiceStartNbr", SqlDbType.Int);
                        command.Parameters["@InvoiceStartNbr"].Value = startNbr - 1;
                        command.Parameters.Add("@ClientCycle", SqlDbType.VarChar);
                        command.Parameters["@ClientCycle"].Value = clientCycle;


                        command.ExecuteNonQuery();
                    }

                    using (var command = new SqlCommand("CreateInvoiceDetail_I_View", sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        command.Parameters.Add("@DBName", SqlDbType.VarChar);
                        command.Parameters["@DBName"].Value = DBName;
                        command.Parameters.Add("@ClientCycle", SqlDbType.VarChar);
                        command.Parameters["@ClientCycle"].Value = clientCycle;

                        command.ExecuteNonQuery();
                    }



                    //Invoice Info
                    string selectString = "SELECT * FROM dbo." + mainViewName;
                    using (SqlDataAdapter daDetail = new SqlDataAdapter(
                                    selectString, sqlConnection))
                    {
                        dsInvoiceData.Clear();
                        daDetail.Fill(dsInvoiceData, mainViewName);
                    }

                    sqlConnection.Close();
                }

                using (ReportViewer reportViewer1 = new ReportViewer())
                {
                    ReportDataSource reportDataSource = new ReportDataSource("dsMain", dsInvoiceData.Tables[0]);
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer1.LocalReport.ReportEmbeddedResource = "TRA_Invoicing_Console.Invoice_Main.rdlc";
                    reportViewer1.LocalReport.SubreportProcessing += new
                                      SubreportProcessingEventHandler(SetSubDataSource);

                    reportViewer1.RefreshReport();
                    Utilities.CreatePDF(outputDirectory + @"\" + outputFileName, reportViewer1);
                }
            }
            catch (Exception e)
            {
                returnMsg =
                  "ERROR: " + GetCallForExceptionThisMethod(MethodBase.GetCurrentMethod(), e) +
                  System.Environment.NewLine +
                  "Message: " + e.Message.ToString();
            }

            return returnMsg;
       
        }

        private static string GetCallForExceptionThisMethod(MethodBase methodBase, Exception e)
        {
            StackTrace trace = new StackTrace(e);
            StackFrame previousFrame = null;

            foreach (StackFrame frame in trace.GetFrames())
            {
                if (frame.GetMethod() == methodBase)
                {
                    break;
                }

                previousFrame = frame;
            }

            return previousFrame != null ? previousFrame.GetMethod().Name : null;
        }


        static void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Clear();

            string compName = e.Parameters["CompanyName_Parameter"].Values[0];
            DataSet ds = new DataSet();

            using (SqlConnection sqlConnection = new SqlConnection(
                            sqlConnStringBase))
            {
                sqlConnection.Open();

                //Invoice Detail
                string selectString = "SELECT * FROM dbo." + detailViewName + " WHERE InsCompany = '" + compName + "'"; // AND FundName = '" + fundName + "'";
                using (SqlDataAdapter daDetail = new SqlDataAdapter(
                                selectString, sqlConnection))
                {
                    dsInvoiceData.Clear();
                    daDetail.Fill(dsInvoiceData, detailViewName);
                }

                sqlConnection.Close();
            }

            e.DataSources.Add(new ReportDataSource("dsDetail", dsInvoiceData.Tables[1]));
        }

     
    }
}
