using System;
using System.Windows.Forms;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using ComponentFactory.Krypton.Toolkit;
using System.Collections.Generic;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Payment
{
    public partial class AddInvoiceForm : KryptonForm
    {
        private readonly ISaleController saleController;
        private readonly ISalesReturnController salesReturnController;

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public event SelectionCompleteEventHandler SelectionComplete;
        protected void OnSelectionComplete(SelectionCompleteEventArgs args)
        {
            SelectionCompleteEventHandler handler = SelectionComplete;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public AddInvoiceForm()
        {
            this.saleController = IOC.Container.GetInstance<SaleController>();
            this.salesReturnController = IOC.Container.GetInstance<SalesReturnController>();

            InitializeComponent();
        }

        private void AddInvoiceForm_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("Select Invoice - {0}", this.CustomerName);
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            var dateFrom = DateFromPicker.Value;
            var dateTo = DateToPicker.Value;

            SaleFilterModel filter = new SaleFilterModel()
            {
                CustomerId = this.CustomerId,
                Paid = PaidType.NotPaid,
                DateType = dateFrom == dateTo ? DateSearchType.All : DateSearchType.DateRange,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Type = -1
            };

            var invoices = this.saleController.FetchSaleWithSearch(filter);
            saleDisplayModelBindingSource.DataSource = invoices;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            LoadInvoices();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                SortableBindingList<SalesCounterItemModel> selectedItems = GetSelectedItems();
                
                var args = new SelectionCompleteEventArgs(selectedItems);
                OnSelectionComplete(args);

                this.Close();
            }
            else
            {
                ClientHelper.ShowErrorMessage("No invoice selected.");
            }
        }

        private SortableBindingList<SalesCounterItemModel> GetSelectedItems()
        {
            SortableBindingList<SalesCounterItemModel> result = new SortableBindingList<SalesCounterItemModel>(selectedInvoices);
            return result;
        }

        SalesCounterItemModel selectedItem;
        List<SalesCounterItemModel> selectedInvoices = new List<SalesCounterItemModel>();
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];

                int id = (int)row.Cells[SalesIdColumn.Name].Value;
                DateTime date = (DateTime)row.Cells[DateColumn.Name].Value;
                string invoiceNumber = row.Cells[InvoiceNumberColumn.Name].Value.ToString();
                decimal amount = (decimal)row.Cells[BalanceColumn.Name].Value;

                selectedItem = new SalesCounterItemModel() { 
                    SaleId = id,
                    Date = date,
                    InvoiceNumber = invoiceNumber,
                    Amount = amount
                };
                
                InvoiceNumberTextbox.Text = invoiceNumber;
                InvoiceAmountTextbox.Value = amount;
                InvoiceAmountTextbox.Focus();

                CheckForInvoiceReturns();
            }
        }

        List<SalesReturnDetailModel> returnsList = new List<SalesReturnDetailModel>();
        private void CheckForInvoiceReturns()
        {
            returnsList.Clear();
            if (selectedItem != null)
            {
                returnsList = this.salesReturnController.GetReturnsPerInvoice(selectedItem.InvoiceNumber);
                salesReturnDetailModelBindingSource.DataSource = null;

                if (returnsList.Count > 0)
                {
                    salesReturnDetailModelBindingSource.DataSource = returnsList;
                    MemoNumberDropdown.SelectedIndex = -1;
                    MemoNumberDropdown.ComboBox.SelectedIndex = -1;
                    ReturnWarningLink.Visible = true;
                }
                else
                {
                    ReturnWarningLink.Visible = false;
                }

            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (selectedItem != null && !string.IsNullOrWhiteSpace(InvoiceNumberTextbox.Text))
            {
                if (InvoiceAmountTextbox.Value > selectedItem.Amount)
                {
                    ClientHelper.ShowErrorMessage("Amount cannot be greater than original amount.");
                    return;
                }

                selectedInvoices.RemoveAll(a => a.SaleId == selectedItem.SaleId);

                string poNumber = this.saleController.GetPONumber(selectedItem.SaleId.Value);
                selectedItem.Amount = InvoiceAmountTextbox.Value;
                selectedItem.PONumber = !string.IsNullOrWhiteSpace(poNumber) ? poNumber : "-";
                selectedInvoices.Insert(0, selectedItem);
                BindSelectedInvoices();
                ClearInvoiceSection();
            }
        }

        private void ClearInvoiceSection()
        {
            InvoiceNumberTextbox.Clear();
            InvoiceAmountTextbox.Value = 0;
            dataGridView1.Focus();
        }

        private void BindSelectedInvoices()
        {
            salesCounterItemModelBindingSource.DataSource = null;
            salesCounterItemModelBindingSource.DataSource = selectedInvoices;
        }

        SalesCounterItemModel selectedReturn;
        private void MemoNumberDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MemoNumberDropdown.SelectedIndex != -1)
            {
                int id = (int)MemoNumberDropdown.SelectedValue;
                var item = returnsList.Find(a => a.Id == id);

                selectedReturn = new SalesCounterItemModel()
                {
                    ReturnId = id,
                    Date = item.ReturnDate.HasValue ? item.ReturnDate.Value : DateTime.Today,
                    MemoNumber = item.MemoNumber,
                    Amount = item.Balance
                };

                ReturnAmountTextbox.Value = item.Balance;
            }
            else
            {
                selectedReturn = null;
                ReturnAmountTextbox.Value = 0;
            }
        }

        private void AddReturnButton_Click(object sender, EventArgs e)
        {
            if (selectedReturn != null)
            {
                if (ReturnAmountTextbox.Value > selectedReturn.Amount)
                {
                    ClientHelper.ShowErrorMessage("Amount cannot be greater than original amount.");
                    return;
                }

                selectedInvoices.RemoveAll(a => a.ReturnId == selectedReturn.ReturnId);

                selectedReturn.Amount = ReturnAmountTextbox.Value;
                selectedReturn.PONumber = "return";
                selectedInvoices.Insert(0, selectedReturn);
                BindSelectedInvoices();
                ClearReturnSection();
            }
        }

        private void ClearReturnSection()
        {
            MemoNumberDropdown.SelectedIndex = -1;
            MemoNumberDropdown.ComboBox.SelectedIndex = -1;
        }
    }

    public delegate void SelectionCompleteEventHandler(object sender, SelectionCompleteEventArgs args);
    
    public class SelectionCompleteEventArgs : EventArgs
    {
        public SortableBindingList<SalesCounterItemModel> SelectedSalesItems { get; set; }
        public SortableBindingList<PurchaseCounterItemModel> SelectedPurchaseItems { get; set; }

        public SelectionCompleteEventArgs(SortableBindingList<SalesCounterItemModel> items)
        {
            this.SelectedSalesItems = items;
        }

        public SelectionCompleteEventArgs(SortableBindingList<PurchaseCounterItemModel> items)
        {
            this.SelectedPurchaseItems = items;
        }
    }

}

