using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;

namespace TY.SPIMS.Client.Controls
{
    public partial class ConsolidatedSalesControl : UserControl
    {
        private readonly ISaleController saleController;
        private readonly ICustomerController customerController;
        private IQueryable<SalesView> sales;

        public ConsolidatedSalesControl()
        {
            this.saleController = IOC.Container.GetInstance<SaleController>();
            this.customerController = IOC.Container.GetInstance<CustomerController>();

            InitializeComponent();
        }

        private void ConsolidatedSalesControl_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            LoadSaleTypes();
            LoadAmountTypes();

            CustomerDropdown.Focus();
        }

        private void LoadSaleTypes()
        {
            string[] types = new string[] { "01-Cash Invoice", "02-Cash/Petty/SOR", "03-Cash/Charge Invoice", "04-Sales Order Slip",
                "05-Charge Invoice", "06-No Invoice" };

            TypeDropdown.Items.AddRange(types);
            TypeDropdown.SelectedIndex = -1;
        }

        private void LoadAmountTypes()
        {
            string[] types = new string[] { "All", "Equal", "Greater Than", "Less Than" };

            AmountTypeDropdown.Items.AddRange(types);
            AmountTypeDropdown.SelectedIndex = -1;
        }

        private void LoadCustomers()
        {
            customerDisplayModelBindingSource.DataSource = this.customerController.FetchAllCustomers();
            CustomerDropdown.SelectedIndex = -1;
        }

        private void LoadSales(SaleFilterModel filter)
        {
            int count = 0;

            long elapsed = ClientHelper.PerformFetch(() =>
            {
                this.sales = this.saleController.FetchSalesWithSearchGeneric(filter);

                var groupedSales = this.sales
                    .GroupBy(a => a.CompanyName)
                    .Select(a => new ConsolidatedSalesModel 
                    {
                        CustomerName = a.Key,
                        TotalAmount = a.Sum(x => x.TotalAmount)
                    })
                    .OrderBy(a => a.CustomerName);

                this.consolidatedSalesBindingSource.DataSource = groupedSales;
            });

            ((MainForm)this.ParentForm).AttachStatus(count, elapsed);
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            SaleFilterModel filter = ComposeSearch();
            LoadSales(filter);
        }

        private SaleFilterModel ComposeSearch()
        {
            SaleFilterModel model = new SaleFilterModel();

            if (CustomerDropdown.SelectedIndex != -1)
                model.CustomerId = (int)CustomerDropdown.SelectedValue;

            if (!string.IsNullOrWhiteSpace(InvoiceTextbox.Text))
                model.InvoiceNumber = InvoiceTextbox.Text.Trim();

            model.Type = TypeDropdown.SelectedIndex;

            if (AmountTypeDropdown.SelectedIndex != -1)
            {
                var amount = !string.IsNullOrWhiteSpace(AmountTextbox.Text) ?
                    decimal.Parse(AmountTextbox.Text) : 0;

                if (AmountTypeDropdown.SelectedIndex == 1)
                {
                    model.AmountType = NumericSearchType.Equal;
                    model.AmountValue = amount;
                }
                else if (AmountTypeDropdown.SelectedIndex == 2)
                {
                    model.AmountType = NumericSearchType.GreaterThan;
                    model.AmountValue = amount;
                }
                else if (AmountTypeDropdown.SelectedIndex == 3)
                {
                    model.AmountType = NumericSearchType.LessThan;
                    model.AmountValue = amount;
                }
            }
            else
                model.AmountType = NumericSearchType.All;

            if (!AllDateRB.Checked)
            {
                model.DateType = DateSearchType.DateRange;
                model.DateTo = DateToPicker.Value;
                model.DateFrom = DateFromPicker.Value;
            }
            else
                model.DateType = DateSearchType.All;

            if (AllPaidRB.Checked)
                model.Paid = PaidType.None;
            else if (PaidRB.Checked)
                model.Paid = PaidType.Paid;
            else if (UnpaidRB.Checked)
                model.Paid = PaidType.NotPaid;

            return model;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearSearch();
        }

        private void ClearSearch()
        {
            CustomerDropdown.ComboBox.SelectedIndex = -1;
            CustomerDropdown.SelectedIndex = -1;
            InvoiceTextbox.Clear();
            AmountTypeDropdown.SelectedIndex = -1;
            AmountTypeDropdown.ComboBox.SelectedIndex = -1;
            AllDateRB.Checked = true;
            AmountTextbox.Text = "0.00";
            DateToPicker.Value = DateTime.Now;
            DateFromPicker.Value = DateTime.Now;
            AllPaidRB.Checked = true;
            TypeDropdown.SelectedIndex = -1;
            TypeDropdown.ComboBox.SelectedIndex = -1;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                var customerName = row.Cells[customerNameColumn.Name].Value;

                var details = this.sales.Where(a => a.CompanyName == customerName);
                salesViewBindingSource.DataSource = details;
            }
            else
            {
                salesViewBindingSource.DataSource = null;
            }
        }
    }
}
