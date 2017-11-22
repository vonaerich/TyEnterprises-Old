using System;
using System.Linq;
using System.Windows.Forms;
using TY.SPIMS.Client.Transactions;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Controls
{
    public partial class SalesControl : UserControl, IRefreshable
    {
        private readonly ISaleController saleController;
        private readonly ISalesReturnController salesReturnController;
        private readonly ICustomerController customerController;
        private readonly IPaymentDetailController paymentDetailController;

        public SalesControl()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.saleController = IOC.Container.GetInstance<SaleController>();
            this.salesReturnController = IOC.Container.GetInstance<SalesReturnController>();
            this.paymentDetailController = IOC.Container.GetInstance<PaymentDetailController>();

            InitializeComponent();
        }

        #region Load

        private void SalesControl_Load(object sender, EventArgs e)
        {
            if (!UserInfo.IsAdmin)
                DeleteButton.Visible = false;

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
                SortableBindingList<SalesView> sales = this.saleController.FetchSaleWithSearch(filter);
                salesViewBindingSource.DataSource = sales;

                if (salesViewBindingSource.Count == 0)
                    salesDetailViewModelBindingSource.DataSource = null;

                TotalSalesTextbox.Text = sales.Sum(a => a.TotalAmount).Value.ToString("Php #,##0.00");
                count = sales.Count;
            });

            ((MainForm)this.ParentForm).AttachStatus(count, elapsed);
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
            else if(PaidRB.Checked)
                model.Paid = PaidType.Paid;
            else if(UnpaidRB.Checked)
                model.Paid = PaidType.NotPaid;

            model.PR = PRTextbox.Text;
            model.PO = POTextbox.Text;

            return model;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            SaleFilterModel filter = ComposeSearch();
            LoadSales(filter);
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == dataGridView2.Columns["TotalDiscountColumn"].Index) && e.Value != null)
            {
                DataGridViewCell cell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.ToolTipText = dataGridView2.Rows[e.RowIndex].Cells["DiscountPercentsColumn"].Value.ToString();
            }
        }

        #endregion

        #region Details

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells[SaleIdColumn.Name].Value;

                string invoiceNumber = row.Cells[InvoiceNumberColumn.Name].Value != null ?
                    row.Cells[InvoiceNumberColumn.Name].Value.ToString() : string.Empty;

                InvoiceNumberTextbox.Text = invoiceNumber;
                TypeTextbox.Text = row.Cells[TypeColumn.Name].Value != null ?
                    row.Cells[TypeColumn.Name].Value.ToString() : "-";
                TotalAmountTextbox.Text = row.Cells[TotalAmountColumn.Name].Value != null ?
                    ((decimal)row.Cells[TotalAmountColumn.Name].Value).ToString("Php #,##0.00") : "Php 0.00";
                CustomerTextbox.Text = row.Cells[CustomerColumn.Name].Value != null ?
                    row.Cells[CustomerColumn.Name].Value.ToString() : "-";
                SaleDateTextbox.Text = row.Cells[DateColumn.Name].Value != null ?
                    ((DateTime)row.Cells[DateColumn.Name].Value).ToShortDateString() : "-";
                InvoiceDiscountTextbox.Text = row.Cells[InvoiceDiscountColumn.Name].Value != null ?
                    ((decimal)row.Cells[InvoiceDiscountColumn.Name].Value).ToString("Php #,##0.00") : "Php 0.00";


                salesDetailViewModelBindingSource.DataSource = this.saleController.FetchSaleDetails(id);
                DisplayPaymentAndReturnInfo(id, invoiceNumber);
            }
            else
            {
                InvoiceNumberTextbox.Text = "-";
                TypeTextbox.Text = "-";
                TotalAmountTextbox.Text = "Php 0.00";
                CustomerTextbox.Text = "-";
                SaleDateTextbox.Text = "-";
                InvoiceDiscountTextbox.Text = "Php 0.00";

                ClearPaymentArea();
            }
        }

        private void DisplayPaymentAndReturnInfo(int id, string invoiceNumber)
        {
            if (id != 0)
            {
                var detail = this.paymentDetailController.SearchSalePayment(id);
                if (detail != null)
                    salesPaymentDisplayModelBindingSource.DataSource = detail;
                else
                    ClearPaymentArea();
            }

            if (!string.IsNullOrWhiteSpace(invoiceNumber))
            {
                var returnDetail = this.salesReturnController.FetchAllReturnsInSale(invoiceNumber);

                if (returnDetail != null)
                {
                    var detail = returnDetail.FirstOrDefault();
                    if (detail != null)
                    {
                        MemoNumberTextbox.Text = detail.MemoNumber;
                        ReturnDateTextbox.Text = detail.ReturnDate.HasValue ? detail.ReturnDate.Value.ToShortDateString() : "-";
                        salesReturnDetailModelBindingSource.DataSource = returnDetail;
                    }
                    else
                        ClearReturnArea();
                }
            }
        }

        private void ClearReturnArea()
        {
            MemoNumberTextbox.Text = "-";
            ReturnDateTextbox.Text = "-";
            salesReturnDetailModelBindingSource.DataSource = null;
        }

        private void ClearPaymentArea()
        {
            salesPaymentDisplayModelBindingSource.DataSource = null;
        }

        #endregion

        #region Clear

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
            PRTextbox.Clear();
            POTextbox.Clear();
        }

        #endregion

        #region Add/Edit Sales

        private void AddButton_Click(object sender, EventArgs e)
        {
            OpenAddForm(0);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                int id = (int)dataGridView1.Rows[e.RowIndex].Cells[SaleIdColumn.Name].Value;
                if (this.saleController.ItemHasPaymentOrReturn(id))
                {
                    if (ClientHelper.ShowConfirmMessage("This sale is already paid or has a return item. Do you want to continue?") != DialogResult.Yes)
                        return; 
                }
                OpenAddForm(id);
            }
        }

        int selectedId;
        private void OpenAddForm(int id)
        {
            selectedId = id;

            Form addForm = this.ParentForm.OwnedForms.FirstOrDefault(a => a.Name == "AddSalesForm");
            if (addForm == null)
            {
                AddSalesForm form = new AddSalesForm();
                form.SaleId = id;
                form.Owner = this.ParentForm;
                form.SalesUpdated += new SalesUpdatedEventHandler(form_SalesUpdated);
                form.Show();
            }
            else
            {
                AddSalesForm openedForm = (AddSalesForm)addForm;
                openedForm.SaleId = id;
                openedForm.LoadSaleDetails();
                openedForm.Focus();
            }
        }

        void form_SalesUpdated(object sender, EventArgs e)
        {
            SaleFilterModel filter = ComposeSearch();
            LoadSales(filter);

            dataGridView1.ClearSelection();

            if (selectedId == 0)
                salesViewBindingSource.Position = salesViewBindingSource.Count - 1;
            else
            {
                SalesView item = ((SortableBindingList<SalesView>)salesViewBindingSource.DataSource)
                    .FirstOrDefault(a => a.Id == selectedId);
                int index = salesViewBindingSource.IndexOf(item);

                salesViewBindingSource.Position = index;
                if(index != -1)
                    dataGridView1.Rows[index].Selected = true;
            }
        }

        #endregion

        #region IRefreshable Members

        public void RefreshView()
        {
            SaleFilterModel filter = ComposeSearch();
            LoadSales(filter);
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

            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells[SaleIdColumn.Name].Value;
                if (this.saleController.ItemHasPaymentOrReturn(id))
                {
                    ClientHelper.ShowErrorMessage("This sale is already paid or has a return item.");
                    return;
                }

                if (ClientHelper.ShowConfirmMessage("Are you sure you want to delete this sale?") == DialogResult.Yes)
                {
                    this.saleController.DeleteSale(id);

                    SaleFilterModel filter = ComposeSearch();
                    LoadSales(filter);
                }
            }
        }

        #endregion
     
    }
}
