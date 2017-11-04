using System;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Controllers;
using TY.SPIMS.Utilities;
using TY.SPIMS.Client.ActionLog;
using TY.SPIMS.Client.Controls;
using TY.SPIMS.Client.Inventory;
using TY.SPIMS.Client.Notification;
using TY.SPIMS.Client.Payment;
using TY.SPIMS.Client.Returns;
using TY.SPIMS.Client.Transactions;
using TY.SPIMS.Client.Users;
using System.Web.Caching;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Client.Report;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client
{
    public partial class MainForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        private readonly IInventoryUserController inventoryUserController;

        public MainForm()
        {
            this.inventoryUserController = IOC.Container.GetInstance<InventoryUserController>();

            InitializeComponent();
        }

        #region Closing Events

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ClientHelper.ShowConfirmMessage("Are you sure you want to exit?") != DialogResult.Yes)
            { e.Cancel = true; }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Load

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadUserDetails();
            CheckUserAccess();
        }

        private void CheckUserAccess()
        {
            if (!UserInfo.IsVisitor)
            {
                if (!UserInfo.IsAdmin)
                {
                    //If not admin, hide action log, notifications, backup DB options
                    toolStripSeparator1.Visible = false;
                    toolStripSeparator2.Visible = false;

                    viewActionLogToolStripMenuItem.Visible = false;
                    notificationsToolStripMenuItem.Visible = false;
                    backupDatabaseToolStripMenuItem.Visible = false;
                }
            }
            else
            {
                addNewItemToolStripMenuItem.Visible = false;
                transactionsToolStripMenuItem.Visible = false;
                utilitiesToolStripMenuItem.Visible = false;

                OpenInPanel(new InventoryControl());
            }
            
            
        }

        private void LoadUserDetails()
        {
            UserFullNameLabel.Text = UserInfo.UserFullName;
            //UserFullNameLabel.Values.ExtraText = UserInfo.IsAdmin ? "Admin" : "User";

            if (UserInfo.Theme)
                MySkin.GlobalPaletteMode = PaletteModeManager.Office2010Blue;
            else
                MySkin.GlobalPaletteMode = PaletteModeManager.SparkleBlue;
        }

        #endregion

        #region Inventory

        private void viewInventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new InventoryControl());
        }

        private void addNewItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenForm(new AddItemForm(), FormWindowState.Normal);
        }

        private void reorderListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenForm(new ReorderListForm());
        }

        #endregion

        #region Utilities

        private void brandsManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new BrandControl());
        }

        private void customerManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new CustomerControl());
        }

        //private void supplierManagementToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    OpenInPanel(new SupplierControl());
        //}

        private void userManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new InventoryUserControl());
        }

        private void viewActionLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenForm(new ActionLogForm(), FormWindowState.Normal);
        }


        private void notificationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenForm(new NotificationForm(), FormWindowState.Normal);
        }

        #endregion

        #region Edit User

        private void EditUser_Click(object sender, EventArgs e)
        {
            AddUserForm form = new AddUserForm();
            form.UserId = UserInfo.UserId;
            form.UserEdit = true;
            form.UserUpdated += new UserUpdatedEventHandler(form_UserUpdated);

            OpenForm(form, FormWindowState.Normal);
        }

        void form_UserUpdated(object sender, EventArgs e)
        {
            LoadUserDetails();
        }

        #endregion

        #region Theme

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MySkin.GlobalPaletteMode = PaletteModeManager.SparkleBlue;
            this.inventoryUserController.UpdateUserTheme(UserInfo.UserId, false);
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MySkin.GlobalPaletteMode = PaletteModeManager.Office2010Blue;
            this.inventoryUserController.UpdateUserTheme(UserInfo.UserId, true);
        }

        #endregion

        #region ClientHelper

        private void OpenForm(Form formToOpen, FormWindowState formWindowState = FormWindowState.Normal)
        {
            Form[] f = this.MdiChildren;

            Form form = f.FirstOrDefault(a => a.GetType() == formToOpen.GetType());
            if (form != null)
            {
                form.WindowState = formWindowState;
                form.Focus();
            }
            else
            {
                formToOpen.WindowState = formWindowState;
                formToOpen.Show();
            }
        }

        private void OpenInPanel(Control userControlToOpen)
        {
            this.panel1.Controls.Clear();

            userControlToOpen.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(userControlToOpen);
            this.panel1.Controls[0].Focus();
            this.ExecutionLabel.Visible = false;
        }

        #endregion

        #region Transactions

        private void addNewSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenForm(new AddSalesForm(), FormWindowState.Normal);
        }

        private void addNewPurchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenForm(new AddPurchaseForm(), FormWindowState.Normal);
        }

        private void viewSalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new SalesControl());
        }

        private void viewPurchasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new PurchaseControl());
        }

        private void salesPaymentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenForm(new AddSalesCounterForm(), FormWindowState.Normal);
        }

        private void purchasePaymentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenForm(new AddPurchaseCounterForm(), FormWindowState.Normal);
        }

        private void viewPaymentsForSalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new SalesCounterControl());
        }

        private void viewPaymentsForPurchasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new PurchaseCounterControl());
        }

        #endregion

        #region Returns

        private void returnsSalesCreditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new SalesReturnControl());
        }

        private void returnsPurchaseDebitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new PurchaseReturnControl());
        }

        #endregion

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void backupDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientHelper.BackupDB();
        }

        #region Navigation

        private void ViewInventoryLink_LinkClicked(object sender, EventArgs e)
        {
            OpenInPanel(new InventoryControl());
        }

        private void AddAutoPartLink_LinkClicked(object sender, EventArgs e)
        {
            OpenForm(new AddItemForm(), FormWindowState.Normal);
        }

        private void ViewSalesLink_LinkClicked(object sender, EventArgs e)
        {
            OpenInPanel(new SalesControl());
        }

        private void AddSaleLink_LinkClicked(object sender, EventArgs e)
        {
            OpenForm(new AddSalesForm(), FormWindowState.Normal);
        }

        private void ViewPurchaseLink_LinkClicked(object sender, EventArgs e)
        {
            OpenInPanel(new PurchaseControl());
        }

        private void AddPurchaseLink_LinkClicked(object sender, EventArgs e)
        {
            OpenForm(new AddPurchaseForm(), FormWindowState.Normal);
        }

        private void CheckMonitoringLink_LinkClicked(object sender, EventArgs e)
        {
            OpenInPanel(new CheckControl());
        }

        private void ViewSRLink_LinkClicked(object sender, EventArgs e)
        {
            OpenInPanel(new SalesReturnControl());
        }

        private void AddSRLink_LinkClicked(object sender, EventArgs e)
        {
            OpenForm(new AddSalesReturnForm(), FormWindowState.Normal);
        }

        private void ViewPRLink_LinkClicked(object sender, EventArgs e)
        {
            OpenInPanel(new PurchaseReturnControl());
        }

        private void AddPRLink_LinkClicked(object sender, EventArgs e)
        {
            OpenForm(new AddPurchaseReturnForm(), FormWindowState.Normal);
        }

        #endregion

        private void viewChecksReceivablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new CheckControl() { Type = IssuerType.Customer });
        }

        private void viewChecksPayablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new CheckControl() { Type = IssuerType.Us });
        }

        public void AttachStatus(int records, long ms)
        {
            this.ExecutionLabel.Text = string.Format(ClientHelper.StatusLabelText,
                records,
                ms.ToString());

            this.ExecutionLabel.Visible = true;
        }

        private void viewORsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new SalesPaymentControl());
        }

        private void viewVouchersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInPanel(new PurchasePaymentControl());
        }

        private void createORToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenForm(new AddSalesPaymentForm(), FormWindowState.Normal);
        }

        private void createVoucherToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenForm(new AddPurchasePaymentForm(), FormWindowState.Normal);
        }

    }
}