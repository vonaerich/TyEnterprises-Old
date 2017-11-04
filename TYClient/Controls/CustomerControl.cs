using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TY.SPIMS.Controllers;
using TY.SPIMS.Client.Customers;
using TY.SPIMS.Utilities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Controls
{
    public partial class CustomerControl : UserControl, IRefreshable
    {
        private readonly ICustomerController customerController;

        public CustomerControl()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();

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

        private void CustomerControl_Load(object sender, EventArgs e)
        {
            if (!UserInfo.IsAdmin)
                DeleteButton.Visible = false;

            LoadCustomers();
            CustomerTextbox.Focus();
        }

        private void LoadCustomers()
        {
            customerDisplayModelBindingSource.DataSource = this.customerController.FetchCustomerWithSearch(CustomerTextbox.Text.Trim());
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
                int id = (int)dataGridView1.Rows[e.RowIndex].Cells["IdColumn"].Value;
                OpenAddForm(id);
            }
        }

        int selectedId = 0;
        private void OpenAddForm(int id)
        {
            selectedId = id;

            Form addForm = this.ParentForm.OwnedForms.FirstOrDefault(a => a.Name == "AddCustomerForm");
            if (addForm == null)
            {
                AddCustomerForm form = new AddCustomerForm();
                form.CustomerId = id;
                form.CustomerUpdated += new CustomerUpdatedEventHandler(form_CustomerUpdated);
                form.Owner = this.ParentForm;
                form.Show();
            }
            else
            {
                AddCustomerForm openedForm = (AddCustomerForm)addForm;
                openedForm.CustomerId = id;
                openedForm.LoadCustomerDetails();
                openedForm.Focus();
            }
        }

        void form_CustomerUpdated(object sender, EventArgs e)
        {
            LoadCustomers();

            var source = (SortableBindingList<CustomerDisplayModel>)customerDisplayModelBindingSource.DataSource;
            if (source != null)
            {
                if (selectedId == 0)
                    selectedId = source.Max(a => a.Id);

                if (selectedId != 0)
                {
                    CustomerDisplayModel item = source.FirstOrDefault(a => a.Id == selectedId);
                    int index = customerDisplayModelBindingSource.IndexOf(item);

                    customerDisplayModelBindingSource.Position = index;
                    dataGridView1.Rows[index].Selected = true;
                }
            }
        }

        #endregion

        #region Search

        private void SearchButton_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void CustomerTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoadCustomers();
        }

        #endregion

        #region Delete

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!UserInfo.IsAdmin)
            {
                ClientHelper.ShowErrorMessage("You are not authorized to delete this record.");
                return;
            }

            if (ClientHelper.ShowConfirmMessage("Are you sure you want to delete this customer?") == DialogResult.Yes)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dataGridView1.SelectedRows[0];
                    int id = (int)row.Cells["IdColumn"].Value;

                    this.customerController.DeleteCustomer(id);
                    ClientHelper.ShowSuccessMessage("Customer deleted successfully.");

                    LoadCustomers();
                }
            }
        }

        #endregion

        #region IRefreshable Members

        public void RefreshView()
        {
            LoadCustomers();
        }

        #endregion
    }
}
