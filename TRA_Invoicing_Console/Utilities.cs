using Microsoft.Reporting.WinForms;
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
using System.Drawing;

namespace TRA_Invoicing_Console
{
    class Utilities
    {
        public static string CreatePDF(string fileName, ReportViewer rv)
        {
            // Variables
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string returnMsg = string.Empty;

            try
            {

                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);
                byte[] bytes = rv.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return returnMsg;

        }
    }
}
