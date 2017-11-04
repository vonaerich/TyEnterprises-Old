using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Entities;
using TY.SPIMS.Controllers;
using TY.SPIMS.Utilities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Users
{
    public partial class AddUserForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        private readonly IInventoryUserController inventoryUserController;

        public int UserId { get; set; }
        public bool UserEdit { get; set; }

        #region Events

        public event UserUpdatedEventHandler UserUpdated;

        protected virtual void OnUserUpdated(EventArgs e)
        {
            UserUpdatedEventHandler handler = UserUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        public AddUserForm()
        {
            this.inventoryUserController = IOC.Container.GetInstance<InventoryUserController>();

            InitializeComponent();
            this.AutoValidate = AutoValidate.Disable;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                if (passwordNotMatch)
                    ClientHelper.ShowErrorMessage("Passwords do not match.");
                else
                    ClientHelper.ShowRequiredMessage("Username, First Name, Password");
            }
            else if (hasDuplicate)
                ClientHelper.ShowDuplicateMessage("Username");
            else
            {
                InventoryUserColumnModel model = new InventoryUserColumnModel()
                {
                    Id = UserId,
                    Username = UsernameTextbox.Text.Trim(),
                    Firstname = FirstnameTextbox.Text.Trim(),
                    Lastname = LastnameTextbox.Text.Trim(),
                    Password = TY.SPIMS.Utilities.Helper.EncryptString(PasswordTextbox.Text.Trim()),
                    IsAdmin = IsAdminCheckbox.Checked,
                    IsApprover = IsApproverCheckbox.Checked,
                    IsVisitor = IsVisitorCheckbox.Checked,
                    IsDeleted = false,
                    Theme = false
                };

                if (UserId == 0)
                {
                    this.inventoryUserController.InsertInventoryUser(model);
                    ClientHelper.ShowSuccessMessage("User successfully added.");
                }
                else
                {
                    this.inventoryUserController.UpdateInventoryUser(model);
                    ClientHelper.ShowSuccessMessage("User details successfully updated.");
                }

                ClearForm();
                UsernameTextbox.Focus();

                if (UserEdit)
                    UserInfo.UserFullName = string.Format("{0} {1}", FirstnameTextbox.Text.Trim(), LastnameTextbox.Text.Trim());

                if (UserUpdated != null)
                    UserUpdated(sender, e);
            }
        }

        #region Validate

        private void UsernameTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextbox.Text))
                e.Cancel = true;
        }

        private void FirstnameTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstnameTextbox.Text))
                e.Cancel = true;
        }

        private void PasswordTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordTextbox.Text))
                e.Cancel = true;
        }

        bool passwordNotMatch = false;
        private void ConfirmTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.Compare(ConfirmTextbox.Text.Trim(), PasswordTextbox.Text.Trim()) != 0)
            {
                passwordNotMatch = true;
                e.Cancel = true;
            }
            else
                passwordNotMatch = false;
        }

        #region Check Duplicate

        private bool hasDuplicate = false;
        private void UsernameTextbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UsernameTextbox.Text))
                hasDuplicate = DuplicateChecker.CodeHasDuplicate(CodeType.User, UsernameTextbox.Text.Trim(), UserId);

            if (hasDuplicate)
                UsernameTextbox.StateCommon.Content.Color1 = Color.Red;
            else
                UsernameTextbox.StateCommon.Content.Color1 = Color.Black;
        }
        #endregion

        #endregion

        #region Clear

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            if (UserId == 0)
            {
                IdTextbox.Text = "0";
                UsernameTextbox.Clear();
                FirstnameTextbox.Clear();
                LastnameTextbox.Clear();
                PasswordTextbox.Clear();
                ConfirmTextbox.Clear();
                IsAdminCheckbox.Checked = false;
                IsApproverCheckbox.Checked = false;
                IsVisitorCheckbox.Checked = false;
            }
            else
                LoadUserDetails();

            passwordNotMatch = false;
        }

        #endregion

        #region Approver

        private void IsAdminCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (IsAdminCheckbox.Checked)
                IsApproverCheckbox.Checked = true;
            else
                IsApproverCheckbox.Checked = false;
        }

        #endregion

        #region Visitor

        private void IsVisitorCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (IsVisitorCheckbox.Checked)
            {
                IsApproverCheckbox.Checked = false;
                IsAdminCheckbox.Checked = false;
                IsApproverCheckbox.Enabled = false;
                IsAdminCheckbox.Enabled = false;
            }
            else
            {
                IsApproverCheckbox.Enabled = true;
                IsAdminCheckbox.Enabled = true;
            }
        }

        #endregion
        
        #region Load

        private void AddUserForm_Load(object sender, EventArgs e)
        {
            if (UserId != 0)
                LoadUserDetails();

            if (UserEdit)
            {
                UsernameTextbox.ReadOnly = true;
                IsAdminCheckbox.Visible = false;
                IsApproverCheckbox.Visible = false;
                IsVisitorCheckbox.Visible = false;
            }
        }

        public void LoadUserDetails()
        {
            if (UserId != 0)
            {
                InventoryUser u = this.inventoryUserController.FetchInventoryUserById(UserId);
                IdTextbox.Text = u.Id.ToString();
                UsernameTextbox.Text = u.Username;
                FirstnameTextbox.Text = u.Firstname;
                LastnameTextbox.Text = u.Lastname;
                IsAdminCheckbox.Checked = u.IsAdmin.HasValue ? u.IsAdmin.Value : false;
                IsApproverCheckbox.Checked = u.IsApprover.HasValue ? u.IsApprover.Value : false;
                IsVisitorCheckbox.Checked = u.IsVisitor.HasValue ? u.IsVisitor.Value : false;
            }
            else
                ClearForm();
        }

        #endregion
       
    }

    public delegate void UserUpdatedEventHandler(object sender, EventArgs e);
}