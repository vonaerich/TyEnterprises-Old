using System;
using System.Windows.Forms;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using System.Collections.Generic;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Payment
{
    public partial class AddPOForm : KryptonForm
    {
        private readonly IPurchaseController purchaseController;
        private readonly IPurchaseReturnController purchaseReturnController;

        public int SupplierId { get; set; }
        public string SupplierName { get; set; }

        public event SelectionCompleteEventHandler SelectionComplete;
        protected void OnSelectionComplete(SelectionCompleteEventArgs args)
        {
            SelectionCompleteEventHandler handler = SelectionComplete;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public AddPOForm()
        {
            this.purchaseController = IOC.Container.GetInstance<PurchaseController>();
            this.purchaseReturnController = IOC.Container.GetInstance<PurchaseReturnController>();

            InitializeComponent();
        }

        private void AddPOForm_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("Select Invoice - {0}", this.SupplierName);
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            var dateFrom = DateFromPicker.Value;
            var dateTo = DateToPicker.Value;

            PurchaseFilterModel filter = new PurchaseFilterModel()
            {
                CustomerId = this.SupplierId,
                Paid = PaidType.NotPaid,
                DateType = dateFrom == dateTo ? DateSearchType.All : DateSearchType.DateRange,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Type = -1
            };

            var invoices = this.purchaseController.FetchPurchaseWithSearch(filter);
            purchaseDisplayModelBindingSource.DataSource = invoices;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            LoadInvoices();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                SortableBindingList<PurchaseCounterItemModel> selectedItems = GetSelectedItems();
                
                var args = new SelectionCompleteEventArgs(selectedItems);
                OnSelectionComplete(args);

                this.Close();
            }
            else
            {
                ClientHelper.ShowErrorMessage("No invoice selected.");
            }
        }

        private SortableBindingList<PurchaseCounterItemModel> GetSelectedItems()
        {
            SortableBindingList<PurchaseCounterItemModel> result = new SortableBindingList<PurchaseCounterItemModel>(selectedInvoices);
            return result;
        }

        PurchaseCounterItemModel selectedItem;
        List<PurchaseCounterItemModel> selectedInvoices = new List<PurchaseCounterItemModel>();
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];

                int id = (int)row.Cells[PurchaseIdColumn.Name].Value;
                DateTime date = (DateTime)row.Cells[DateColumn.Name].Value;
                string invoiceNumber = row.Cells[InvoiceNumberColumn.Name].Value.ToString();
                decimal amount = (decimal)row.Cells[BalanceColumn.Name].Value;

                selectedItem = new PurchaseCounterItemModel() { 
                    PurchaseId = id,
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

        List<PurchaseReturnDetailModel> returnsList = new List<PurchaseReturnDetailModel>();
        private void CheckForInvoiceReturns()
        {
            returnsList.Clear();
            if (selectedItem != null)
            {
                returnsList = this.purchaseReturnController.GetReturnsPerInvoice(selectedItem.InvoiceNumber);
                purchaseReturnDetailModelBindingSource.DataSource = null;

                if (returnsList.Count > 0)
                {
                    purchaseReturnDetailModelBindingSource.DataSource = returnsList;
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

                selectedInvoices.RemoveAll(a => a.PurchaseId == selectedItem.PurchaseId);

                string poNumber = this.purchaseController.GetPONumber(selectedItem.PurchaseId.Value);
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
            purchaseCounterItemModelBindingSource.DataSource = null;
            purchaseCounterItemModelBindingSource.DataSource = selectedInvoices;
        }

        PurchaseCounterItemModel selectedReturn;
        private void MemoNumberDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MemoNumberDropdown.SelectedIndex != -1)
            {
                int id = (int)MemoNumberDropdown.SelectedValue;
                var item = returnsList.Find(a => a.Id == id);

                selectedReturn = new PurchaseCounterItemModel()
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
}

