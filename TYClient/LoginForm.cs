using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Controllers;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;
using SimpleInjector.Extensions.LifetimeScoping;

namespace TY.SPIMS.Client
{
    public partial class LoginForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        private readonly IInventoryUserController inventoryUserController;

        private enum LoginResult
        { Success, NotFound, RequiredMissing, CannotConnect }

        private BackgroundWorker loginWorker = new BackgroundWorker();
        public LoginForm()
        {
            this.inventoryUserController = IOC.Container.GetInstance<InventoryUserController>();

            InitializeComponent();

            loginWorker.DoWork += new DoWorkEventHandler(loginWorker_DoWork);
            loginWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(loginWorker_RunWorkerCompleted);
        }

        #region Login Worker

        void loginWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadImage.Visible = false;
            LoginButton.Enabled = true;

            LoginResult r = (LoginResult)e.Result;
            if (r == LoginResult.Success)
            {
                this.Hide();

                MainForm form = new MainForm();
                form.Show();
            }
            else if (r == LoginResult.NotFound)
                ClientHelper.ShowErrorMessage("User not found.");
            else if (r == LoginResult.RequiredMissing)
                ClientHelper.ShowErrorMessage("Please input username and password.");
            else if (r == LoginResult.CannotConnect)
                ClientHelper.ShowErrorMessage("Cannot connect to database. Please contact your administrator.");

        }

        void loginWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<string, string> userDetails = (Dictionary<string, string>)e.Argument;
            string username = userDetails["Username"];
            string pw = userDetails["Password"];

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(pw))
                e.Result = LoginResult.RequiredMissing;
            else
            {
                try
                {
                    var user = this.inventoryUserController.LoginUser(username, pw);

                    if (user != null)
                    {
                        UserInfo.UserId = user.Id;
                        UserInfo.Username = user.Username;
                        UserInfo.UserFullName = string.Format("{0} {1}", user.Firstname, user.Lastname);
                        UserInfo.IsAdmin = user.IsAdmin.HasValue ? user.IsAdmin.Value : false;
                        UserInfo.Theme = user.Theme.HasValue ? user.Theme.Value : false;
                        UserInfo.IsVisitor = user.IsVisitor.HasValue ? user.IsVisitor.Value : false;

                        e.Result = LoginResult.Success;
                    }
                    else
                        e.Result = LoginResult.NotFound;
                }
                catch (EntityException ex)
                {
                    e.Result = LoginResult.CannotConnect;
                }
            }
        }

        #endregion

        private void LoginButton_Click(object sender, EventArgs e)
        {
            LoadImage.Visible = true;
            LoginButton.Enabled = false;

            Dictionary<string, string> loginDetails = new Dictionary<string, string>();
            loginDetails.Add("Username", UsernameTextbox.Text.Trim());
            loginDetails.Add("Password", PasswordTextbox.Text.Trim());

            if (loginWorker.IsBusy == false)
                loginWorker.RunWorkerAsync(loginDetails);
        }

        #region Textbox Events

        private void UsernameTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoginButton_Click(sender, e);
        }

        private void PasswordTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoginButton_Click(sender, e);
        }

        #endregion

        #region Close

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        #endregion

    }
}