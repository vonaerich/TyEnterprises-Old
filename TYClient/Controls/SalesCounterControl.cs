using System;
using System.Windows.Forms;
using TY.SPIMS.Client.Payment;
using TY.SPIMS.Controllers;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Controls
{
    public partial class SalesCounterControl : UserControl
    {
        private readonly ICustomerController customerController;
        private readonly ISalesCounterController salesCounterController;

        public SalesCounterControl()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.salesCounterController = IOC.Container.GetInstance<SalesCounterController>();

            InitializeComponent();
        }

        private void SalesCounterControl_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            CustomerDropdown.Focus();
        }

        private void LoadCustomers()
        {
            customerDisplayModelBindingSource.DataSource = this.customerController.FetchAllCustomers();
            CustomerDropdown.SelectedIndex = -1;
        }

        private void LoadSalesCounters(CounterFilterModel filter)
        {
            int records = 0;
            long elapsedTime = ClientHelper.PerformFetch(() =>
            {
                var results = this.salesCounterController.FetchSalesCounterWithSearch(filter);
                salesCounterDisplayModelBindingSource.DataSource = results;

                if (salesCounterDisplayModelBindingSource.Count == 0)
                    salesCounterDisplayModelBindingSource.DataSource = null;

                records = results.Count;
            });

            ((MainForm)this.ParentForm).AttachStatus(records, elapsedTime);
        }

        private CounterFilterModel ComposeSearch()
        {
            CounterFilterModel model = new CounterFilterModel();

            if (CustomerDropdown.SelectedIndex != -1)
                model.CustomerId = (int)CustomerDropdown.SelectedValue;

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
            LoadSalesCounters(filter);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            salesCounterItemModelBindingSource.DataSource = null;
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells[SalesCounterIdColumn.Name].Value;

                salesCounterItemModelBindingSource.DataSource = this.salesCounterController.FetchSalesItems(id);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearSearch();
        }

        private void ClearSearch()
        {
            CustomerDropdown.ComboBox.SelectedIndex = -1;
            CustomerDropdown.SelectedIndex = -1;
            CounterNumberTextbox.Clear();
            AllDateRB.Checked = true;
            DateFromPicker.Value = DateTime.Now;
            DateToPicker.Value = DateTime.Now;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddSalesCounterForm form = new AddSalesCounterForm();
            form.Show();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells[SalesCounterIdColumn.Name].Value;

                AddSalesCounterForm f = new AddSalesCounterForm();
                f.SalesCounterId = id;
                f.Show();
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (ClientHelper.ShowConfirmMessage("Are you sure you want to delete this counter?") == DialogResult.Yes)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells[SalesCounterIdColumn.Name].Value;

                this.salesCounterController.Delete(id);
                ClientHelper.ShowSuccessMessage("Counter deleted successfully.");
                
                LoadSalesCounters(ComposeSearch());
            }
        }
    }
}
