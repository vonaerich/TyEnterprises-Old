using System;
using System.Windows.Forms;
using TY.SPIMS.Client.Payment;
using TY.SPIMS.Controllers;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Controls
{
    public partial class PurchaseCounterControl : UserControl
    {
        private readonly ICustomerController customerController;
        private readonly IPurchaseCounterController purchaseCounterController;

        public PurchaseCounterControl()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.purchaseCounterController = IOC.Container.GetInstance<PurchaseCounterController>();

            InitializeComponent();
        }

        private void PurchaseCounterControl_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            SupplierDropdown.Focus();
        }

        private void LoadSuppliers()
        {
            supplierDisplayModelBindingSource.DataSource = this.customerController.FetchAllCustomers();
            SupplierDropdown.SelectedIndex = -1;
        }

        private void LoadPurchaseCounters(CounterFilterModel filter)
        {
            int records = 0;
            long elapsedTime = ClientHelper.PerformFetch(() =>
            {
                var results = this.purchaseCounterController.FetchPurchaseCounterWithSearch(filter);
                purchaseCounterDisplayModelBindingSource.DataSource = results;

                if (purchaseCounterDisplayModelBindingSource.Count == 0)
                    purchaseCounterDisplayModelBindingSource.DataSource = null;

                records = results.Count;
            });

            ((MainForm)this.ParentForm).AttachStatus(records, elapsedTime);
        }

        private CounterFilterModel ComposeSearch()
        {
            CounterFilterModel model = new CounterFilterModel();

            if (SupplierDropdown.SelectedIndex != -1)
                model.CustomerId = (int)SupplierDropdown.SelectedValue;

            if (!string.IsNullOrWhiteSpace(CounterNumberTextbox.Text))
                model.CounterNumber = CounterNumberTextbox.Text.Trim();

            if (!AllDateRB.Checked)
            {
                if (DateRangeRB.Checked)
                {
                    model.DateType = DateSearchType.DateRange;
                    model.DateFrom = DateFromPicker.Value;
                    model.DateTo = DateToPicker.Value;
                }
            }
            else
                model.DateType = DateSearchType.All;

            return model;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            CounterFilterModel filter = ComposeSearch();
            LoadPurchaseCounters(filter);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            purchaseCounterItemModelBindingSource.DataSource = null;
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells[PurchaseCounterIdColumn.Name].Value;

                purchaseCounterItemModelBindingSource.DataSource = this.purchaseCounterController.FetchPurchaseItems(id);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearSearch();
        }

        private void ClearSearch()
        {
            SupplierDropdown.ComboBox.SelectedIndex = -1;
            SupplierDropdown.SelectedIndex = -1;
            CounterNumberTextbox.Clear();
            AllDateRB.Checked = true;
            DateFromPicker.Value = DateTime.Now;
            DateToPicker.Value = DateTime.Now;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddPurchaseCounterForm form = new AddPurchaseCounterForm();
            form.Show();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells[PurchaseCounterIdColumn.Name].Value;

                AddPurchaseCounterForm f = new AddPurchaseCounterForm();
                f.PurchaseCounterId = id;
                f.Show();
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (ClientHelper.ShowConfirmMessage("Are you sure you want to delete this counter?") == DialogResult.Yes)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells[PurchaseCounterIdColumn.Name].Value;

                this.purchaseCounterController.Delete(id);
                ClientHelper.ShowSuccessMessage("Counter deleted successfully.");
                
                LoadPurchaseCounters(ComposeSearch());
            }
        }
    }
}
