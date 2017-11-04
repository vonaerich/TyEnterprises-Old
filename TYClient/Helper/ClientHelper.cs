using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using System.Diagnostics;
using System.Configuration;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace TY.SPIMS.Client
{
    public class ClientHelper
    {
        public static bool IsEdit = false;
        public static string StatusLabelText = "Total Records: {0} | Execution Time: {1} ms.";

        #region Message Box

        public static DialogResult ShowConfirmMessage(string message)
        {
            return KryptonMessageBox.Show(message, "Confirm", MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        }

        public static void ShowErrorMessage(string message)
        {
            KryptonMessageBox.Show(message, "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static void ShowRequiredMessage(string requiredFields)
        {
            KryptonMessageBox.Show(string.Format("Required fields: {0}", requiredFields),
                "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static void ShowSuccessMessage(string message)
        {
            KryptonMessageBox.Show(message, "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public static void ShowDuplicateMessage(string message)
        {
            KryptonMessageBox.Show(string.Format("{0} already exists.", message),
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Exception Handler

        public static void LogException(Exception ex)
        {
            string sourceName = "T.Y. Enterprises - Sales and Inventory System";

            if (!EventLog.SourceExists(sourceName))
                EventLog.CreateEventSource(sourceName, "Application");

            EventLog.WriteEntry(sourceName, ex.Message.ToString(), EventLogEntryType.Error);
        }

        #endregion

        #region DB Backup

        public static void BackupDB()
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "Database backup file (*.bak)|*.bak";

            if (d.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Backup sqlBackup = new Backup();

                    sqlBackup.Action = BackupActionType.Database;
                    sqlBackup.BackupSetDescription = "ArchiveDataBase:" +
                                                     DateTime.Now.ToShortDateString();
                    sqlBackup.BackupSetName = "Archive";

                    string dbName = "TYEnterprises";
                    sqlBackup.Database = dbName;

                    BackupDeviceItem deviceItem = new BackupDeviceItem(d.FileName, DeviceType.File);

                    string ServerIP = ConfigurationManager.AppSettings["ServerIP"] != null ?
                        ConfigurationManager.AppSettings["ServerIP"] : "127.0.0.1";

                    ServerConnection connection = new ServerConnection(ServerIP, "TYLogin", "ty12345");
                    Server sqlServer = new Server(connection);

                    Database db = sqlServer.Databases[dbName];

                    sqlBackup.Initialize = true;
                    sqlBackup.Checksum = true;
                    sqlBackup.ContinueAfterError = true;

                    sqlBackup.Devices.Add(deviceItem);
                    sqlBackup.Incremental = false;

                    sqlBackup.ExpirationDate = DateTime.Now.AddDays(30);
                    sqlBackup.LogTruncation = BackupTruncateLogType.Truncate;

                    sqlBackup.FormatMedia = false;

                    sqlBackup.SqlBackup(sqlServer);

                    ShowSuccessMessage("Backup successful.");
                }
                catch (Exception ex)
                {
                   throw ex;
                }
            }
        }

        #endregion

        #region Helper

        public static long PerformFetch(Action a)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            a();
            s.Stop();

            return s.ElapsedMilliseconds;
        }

        public static string GetSaleType(int type)
        {
            string saleType = string.Empty;

            switch (type)
            {
                case 0: saleType = "Cash Invoice";
                    break;
                case 1: saleType = "Cash/Petty/SOR";
                    break;
                case 2: saleType = "Cash/Charge Invoice";
                    break;
                case 3: saleType = "Sales Order Slip";
                    break;
                case 4: saleType = "Charge Invoice";
                    break;
                case 5: saleType = "No Invoice";
                    break;
            }

            return saleType;
        }

        #endregion

    }
}
