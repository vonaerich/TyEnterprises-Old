using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TY.SPIMS.Controllers;
using TY.SPIMS.Client.Users;
using TY.SPIMS.Utilities;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.POCOs;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Controls
{
    public partial class InventoryUserControl : UserControl, IRefreshable
    {
        private readonly IInventoryUserController inventoryUserController;

        public InventoryUserControl()
        {
            this.inventoryUserController = IOC.Container.GetInstance<InventoryUserController>();

            InitializeComponent();
        }

        #region Next / Previous

        private void NextButton_Click(object sender, EventArgs e)
        {
            // If nothing is selected
            if (dataGridView1.SelectedRows.Count == 0)
            {
                // If there are rows in the grid
                if (dataGridView1.Rows.Count > 0)
                {
                    // Select the first row
                    dataGridView1.Rows[0].Selected = true;
                }
            }
            else
            {
                // Find index of next row
                int index = dataGridView1.SelectedRows[0].Index + 1;

                // If past end of list then go back to the start
                if (index >= dataGridView1.Rows.Count)
                    index = 0;

                // Select the row
                dataGridView1.Rows[index].Selected = true;
            }

            dataGridView1.Refresh();
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            // If nothing is selected
            if (dataGridView1.SelectedRows.Count == 0)
            {
                // If there are rows in the grid
                if (dataGridView1.Rows.Count > 0)
                {
                    // Select the last row
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
                }
            }
            else
            {
                // Find index of previous row
                int index = dataGridView1.SelectedRows[0].Index - 1;

                // If past start of list then go back to the end
                if (index < 0)
                    index = dataGridView1.Rows.Count - 1;

                // Select the row
                dataGridView1.Rows[index].Selected = true;
            }

            dataGridView1.Refresh();
        }

        #endregion

        #region Load

        private void InventoryUserControl_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            inventoryUserDisplayModelBindingSource.DataSource =
                this.inventoryUserController.FetchInventoryUserWithSearch(UserTextbox.Text);
        }

        #endregion

        #region Add/Edit

        private void AddButton_Click(object sender, EventArgs e)
        {
            OpenAddForm(0);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                int userId = (int)dataGridView1.Rows[e.RowIndex].Cells["IdColumn"].Value;
                OpenAddForm(userId);
            }
        }

        int selectedId = 0;
        private void OpenAddForm(int userId)
        {
            selectedId = userId;

            Form addForm = this.ParentForm.OwnedForms.FirstOrDefault(a => a.Name == "AddUserForm");
            if (addForm == null)
            {
                AddUserForm form = new AddUserForm();
                form.UserId = userId;
                form.Owner = this.ParentForm;
                form.UserUpdated += new UserUpdatedEventHandler(form_UserUpdated);
                form.Show();
            }
            else
            {
                AddUserForm openedForm = (AddUserForm)addForm;
                openedForm.UserId = userId;
                openedForm.LoadUserDetails();
                openedForm.Focus();
            }
        }

        void form_UserUpdated(object sender, EventArgs e)
        {
            LoadUsers();

            var source = (SortableBindingList<InventoryUserDisplayModel>)inventoryUserDisplayModelBindingSource.DataSource;
            if (source != null)
            {
                if (selectedId == 0)
                    selectedId = source.Max(a => a.Id);

                if (selectedId != 0)
                {
                    InventoryUserDisplayModel item = source.FirstOrDefault(a => a.Id == selectedId);
                    int index = inventoryUserDisplayModelBindingSource.IndexOf(item);

                    inventoryUserDisplayModelBindingSource.Position = index;
                    dataGridView1.Rows[index].Selected = true;
                }
            }
        }

        #endregion

        #region Search

        private void SearchButton_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void UserTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoadUsers();
        }

        #endregion

        #region Delete

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (ClientHelper.ShowConfirmMessage("Are you sure you want to delete this user?") == DialogResult.Yes)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dataGridView1.SelectedRows[0];
                    int id = (int)row.Cells["IdColumn"].Value;

                    this.inventoryUserController.DeleteInventoryUser(id);
                    ClientHelper.ShowSuccessMessage("User deleted successfully.");

                    LoadUsers();
                }
            }
        }

        #endregion

        #region IRefreshable Members

        public void RefreshView()
        {
            if (tabControl1.SelectedIndex == 0)
                LoadUsers();
            else
                LoadApproval();
        }

        #endregion

        private void kryptonNavigator1_SelectedPageChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                LoadUsers();
            else
                LoadApproval();
        }

        #region Approval Settings

        private void LoadApproval()
        {
            SystemSettings.RefreshApproval();

            List<string> keys = SystemSettings.Approval.Keys.ToList();
            foreach (string key in keys)
            {
                if (kryptonHeaderGroup3.Panel.Controls.Count > 0)
                {
                    Control[] c = kryptonHeaderGroup3.Panel.Controls.Find(key, true);
                    if (c.Count() > 0)
                    {
                        KryptonCheckBox chkBox = (KryptonCheckBox)c[0];
                        chkBox.Checked = SystemSettings.Approval[key];
                    }
                }
            }
        }

        private void SaveApproval_Click(object sender, EventArgs e)
        {
            foreach (Control c in tableLayoutPanel1.Controls)
            {
                if(c.GetType() == typeof(KryptonCheckBox))
                {
                    KryptonCheckBox chk = (KryptonCheckBox)c;
                    SystemSettings.Approval[c.Name] = chk.Checked;
                }
            }

            SystemSettings.SaveApproval();
            ClientHelper.ShowSuccessMessage("Approval setting saved successfully.");
        }

        #endregion

    }
}
