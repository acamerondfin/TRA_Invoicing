using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TRA_Invoicing_Console
{
    class UpdateBaseData
    {
        protected static string DBName = ConfigurationManager.AppSettings.Get("DatabaseName");
        protected static string DBName_Base = ConfigurationManager.AppSettings.Get("Base_DatabaseName");
        protected static string SQLServerName = ConfigurationManager.AppSettings.Get("SQLServerName");
        protected static string SQLUserName = ConfigurationManager.AppSettings.Get("SQLServerUserName");
        protected static string SQLPwd = ConfigurationManager.AppSettings.Get("SQLServerPassword");

        protected static string clientCode = ConfigurationManager.AppSettings.Get("ClientCode").ToString();

        public static string sqlConnString = "";
        public static string sqlConnStringBase = "";

        private static void BuildConnectionStrings()
        {
            try
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
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static string UpdateAllInfo()
        {
            string returnMsg = string.Empty;

            try
            {
                BuildConnectionStrings();
                UpdateContactInfo();
                UpdateBillingAgreementInfo();

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



        private static void UpdateContactInfo()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(
                                sqlConnStringBase))
                {
                    sqlConnection.Open();

                    using (var command = new SqlCommand("Update_ContactsInfo", sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        command.Parameters.Add("@DBName", SqlDbType.VarChar);
                        command.Parameters["@DBName"].Value = DBName;
                        command.Parameters.Add("@ClientCode", SqlDbType.VarChar);
                        command.Parameters["@ClientCode"].Value = clientCode;

                        command.ExecuteNonQuery();
                    }

                    sqlConnection.Close();
                }

            }
            catch (Exception e)
            {
                throw;
            }

        }

        private static void UpdateBillingAgreementInfo()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(
                                sqlConnStringBase))
                {
                    sqlConnection.Open();

                    using (var command = new SqlCommand("Update_BillingAgreementInfo", sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        command.Parameters.Add("@DBName", SqlDbType.VarChar);
                        command.Parameters["@DBName"].Value = DBName;

                        command.ExecuteNonQuery();
                    }

                    sqlConnection.Close();
                }

            }
            catch (Exception e)
            {
                throw;
            }

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
    }
}
