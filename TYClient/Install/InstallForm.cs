using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using System.Configuration;

namespace TY.SPIMS.Client.Install
{
    public partial class InstallForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public InstallForm()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IPTextbox.Text))
                return;

            try
            {
                Configuration c = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location);
                ConnectionStringsSection section = (ConnectionStringsSection)c.GetSection("connectionStrings");
                
                string oldString = section.ConnectionStrings["TYEnterprisesEntities"].ConnectionString;
                string newString = BuildConnectionString(oldString);

                section.ConnectionStrings["TYEnterprisesEntities"].ConnectionString = newString;

                AppSettingsSection appSettings = (AppSettingsSection)c.GetSection("appSettings");
                appSettings.Settings["ServerIP"].Value = IPTextbox.Text.Trim();

                c.Save();

                this.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string BuildConnectionString(string old)
        {
            StringBuilder result = new StringBuilder();

            string[] connString = old.Split(';');
            foreach (string s in connString)
            {
                string stringToAppend = s;

                if (s.Contains("data source"))
                    stringToAppend = s.Replace("localhost", IPTextbox.Text);

                result.Append(stringToAppend);
                result.Append(';');
            }

            return result.ToString();
        }

       
    }
}