using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Controllers;
using TY.SPIMS.POCOs;
using TY.SPIMS.POCOs.BaseClasses;
using TY.SPIMS.Utilities;
using System.Threading;
using TY.SPIMS.Entities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Payment
{
    public partial class AddCounterItemForm : KryptonForm 
    {
        private readonly ISaleController saleController;
        private readonly ISalesReturnController salesReturnController;
        private readonly IPurchaseController purchaseController;
        private readonly IPurchaseReturnController purchaseReturnController;

        public int CustomerId  { get; set; }
        public string CustomerName { get; set; }
        public Dictionary<int?, decimal?> InvoiceIds { get; set; }
        public Dictionary<int?, decimal?> ReturnIds { get; set; }
        public PaymentType Type { get; set; }

        public event EventHandler<SelectionCompleteEventArgs> SelectionComplete;  
        protected void OnSelectionComplete(SelectionCompleteEventArgs args)
        {
            var handler = SelectionComplete;
            if (handler != null)
            {
                handler(this, args);
            }
        } 

        public AddCounterItemForm()
        {
            this.saleController = IOC.Container.GetInstance<SaleController>();
            this.salesReturnController = IOC.Container.GetInstance<SalesReturnController>();
            this.purchaseController = IOC.Container.GetInstance<PurchaseController>();
            this.purchaseReturnController = IOC.Container.GetInstance<PurchaseReturnController>();

            InitializeComponent();
        }

        #region Load

        private void AddCounterItemForm_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("Select Invoice - {0}", this.CustomerName);
            LoadInvoices();
            LoadReturns();

            if(InvoiceIds.Count > 0)
                SelectInvoices();

            if (ReturnIds.Count > 0)
                SelectReturns();
        }

        private void SelectReturns()
        {
            foreach (DataGridViewRow row in kryptonDataGridView1.Rows)
            {
                var id = row.Cells[returnIdColumn.Name].Value.ToInt();
                if (ReturnIds.Keys.Contains(id))
                {
                    row.Cells[returnChkColumn.Name].Value = true;
                    row.Cells[returnAmountColumn.Name].Value = ReturnIds[id];
                }
            }
        }

        private void SelectInvoices()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var id = row.Cells[IdColumn.Name].Value.ToInt();
                if (InvoiceIds.Keys.Contains(id))
                {
                    row.Cells[ChkColumn.Name].Value = true;
                    row.Cells[AmountPaidColumn.Name].Value = InvoiceIds[id];
                }
            }
        }

        private void LoadReturns()
        {
            if (this.Type == PaymentType.Sales)
            {
                var returns = this.salesReturnController.GetReturnsWithBalance(CustomerId);
                AddMissingReturns(returns);
                salesReturnDetailModelBindingSource.DataSource = new SortableBindingList<SalesReturnDetailModel>(returns);
                kryptonDataGridView1.DataSource = salesReturnDetailModelBindingSource;
            }
            else 
            {
                var returns = this.purchaseReturnController.GetReturnsWithBalance(CustomerId);
                AddMissingReturns(returns);
                salesReturnDetailModelBindingSource.DataSource = new SortableBindingList<SalesReturnDetailModel>(returns);
                kryptonDataGridView1.DataSource = salesReturnDetailModelBindingSource;
            }
        }

        private void LoadInvoices()
        {
            var dateFrom = DateFromPicker.Value;
            var dateTo = DateToPicker.Value;

            var filter = GetFilterModel();
            filter.CustomerId = this.CustomerId;
            filter.Paid = PaidType.NotPaid;
            filter.DateType = dateFrom.Date == dateTo.Date ? DateSearchType.All : DateSearchType.DateRange;
            filter.DateFrom = dateFrom;
            filter.DateTo = dateTo;
            filter.Type = -1;

            if (this.Type == PaymentType.Sales)
            {
                var invoices = this.saleController.FetchSaleWithSearch((SaleFilterModel)filter);
                AddMissingInvoices(invoices);
                saleDisplayModelBindingSource.DataSource = new SortableBindingList<SalesView>(invoices);
                dataGridView1.DataSource = saleDisplayModelBindingSource;
            }
            else
            {
                var invoices = this.purchaseController.FetchPurchaseWithSearch((PurchaseFilterModel)filter);
                AddMissingInvoices(invoices);
                purchaseDisplayModelBindingSource.DataSource = new SortableBindingList<PurchasesView>(invoices);
                dataGridView1.DataSource = purchaseDisplayModelBindingSource;
            }
        }

        private void AddMissingInvoices(dynamic invoices)
        {
            //Add missing items
            var missingSalesInvoices = this.saleController.FetchSaleByIds(InvoiceIds.Keys.Select(a => a.Value).ToList());
            var missingPurchasingInvoices = this.purchaseController.FetchPurchaseByIds(InvoiceIds.Keys.Select(a => a.Value).ToList());

            if (this.Type == PaymentType.Sales)
            {
                missingSalesInvoices.ToList().ForEach(a => {
                    a.Balance = a.TotalAmount;
                    if (!invoices.Contains(a)) { invoices.Insert(0, a); }
                });
            }
            else
            {
                missingPurchasingInvoices.ToList().ForEach(a => {
                    a.Balance = a.TotalAmount;
                    if (!invoices.Contains(a)) { invoices.Insert(0, a); }
                });
            }
        }

        private void AddMissingReturns(dynamic returns)
        {
            var missingSaleReturns = this.salesReturnController.FetchSalesReturnByIds(ReturnIds.Keys.Select(a => a.Value).ToList());
            var missingPurchaseReturns = this.purchaseReturnController.FetchPurchaseReturnByIds(ReturnIds.Keys.Select(a => a.Value).ToList());

            if (this.Type == PaymentType.Sales)
            {
                missingSaleReturns.ToList().ForEach(a =>
                {
                    a.Balance = a.TotalAmount;
                    if (!returns.Contains(a)) { returns.Insert(0, a); }
                });
            }
            else
            {
                missingPurchaseReturns.ToList().ForEach(a =>
                {
                    a.Balance = a.TotalAmount;
                    if (!returns.Contains(a)) { returns.Insert(0, a); }
                });
            }
        }

        private FilterModelBase GetFilterModel()
        {
            return this.Type == PaymentType.Sales ? new SaleFilterModel() as FilterModelBase : 
                new PurchaseFilterModel() as FilterModelBase;
        }

        #endregion

        #region Invoice Selection

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
                return;

            var row = dataGridView1.Rows[e.RowIndex];
            CheckInvoice(row);
            ComputeTotal();
            dataGridView1.CurrentCell = row.Cells[AmountPaidColumn.Name];
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && (e.ColumnIndex != 0 || e.ColumnIndex != 10))
                return;

            if (e.ColumnIndex == 0)
            {
                var row = dataGridView1.Rows[e.RowIndex];
                CheckInvoice(row);
            }

            ComputeTotal();
        }

        private void CheckInvoice(DataGridViewRow row)
        {
            var chkCell = row.Cells[ChkColumn.Name] as KryptonDataGridViewCheckBoxCell;
            if (chkCell != null && chkCell.EditedFormattedValue != null && (bool)chkCell.EditedFormattedValue == true)
                row.Cells[AmountPaidColumn.Name].Value = row.Cells[balanceColumn.Name].Value;
            else
                row.Cells[AmountPaidColumn.Name].Value = null;
        }

        #endregion

        #region Returns
        private void kryptonDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
                return;

            var row = kryptonDataGridView1.Rows[e.RowIndex];
            CheckReturn(row);
            ComputeTotal();
            kryptonDataGridView1.CurrentCell = row.Cells[returnAmountColumn.Name];
        }

        private void kryptonDataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 6)
                return;

            ComputeTotal();
        }

        private void CheckReturn(DataGridViewRow row)
        {
            var chkCell = row.Cells[returnChkColumn.Name] as KryptonDataGridViewCheckBoxCell;
            if (chkCell != null && chkCell.EditedFormattedValue != null && (bool)chkCell.EditedFormattedValue == true)
                row.Cells[returnAmountColumn.Name].Value = row.Cells[returnBalanceColumn.Name].Value;
            else
                row.Cells[returnAmountColumn.Name].Value = null;
        }
        #endregion

        private void FilterButton_Click(object sender, EventArgs e)
        {
            LoadInvoices();
            ComputeTotal();
        }

        private SortableBindingList<SalesCounterItemModel> GetSelectedInvoices()
        {
            List<SalesCounterItemModel> list = new List<SalesCounterItemModel>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var chkCell = row.Cells[ChkColumn.Name] as KryptonDataGridViewCheckBoxCell;
                if (chkCell != null && chkCell.EditedFormattedValue != null && (bool)chkCell.EditedFormattedValue == true)
                {
                    var saleId = row.Cells[IdColumn.Name].Value.ToInt();
                    list.Add(new SalesCounterItemModel() { 
                        SaleId = saleId,
                        Amount = row.Cells[AmountPaidColumn.Name].Value.ToDecimal(),
                        InvoiceNumber = row.Cells[InvoiceNumberColumn.Name].Value.ToString(),
                        Date = row.Cells[DateColumn.Name].Value.ToDate(),
                        PONumber = this.saleController.GetPONumber(saleId)
                    });
                }
            }

            foreach (DataGridViewRow row in kryptonDataGridView1.Rows)
            {
                var chkCell = row.Cells[returnChkColumn.Name] as KryptonDataGridViewCheckBoxCell;
                if (chkCell != null && chkCell.EditedFormattedValue != null && (bool)chkCell.EditedFormattedValue == true)
                {
                    list.Add(new SalesCounterItemModel()
                    {
                        ReturnId = row.Cells[returnIdColumn.Name].Value.ToInt(),
                        MemoNumber = row.Cells[memoNumberColumn.Name].Value.ToString(),
                        Amount = row.Cells[returnAmountColumn.Name].Value.ToDecimal(),
                        InvoiceNumber = row.Cells[returnInvoiceColumn.Name].Value.ToString(),
                        Date = row.Cells[returnDateColumn.Name].Value.ToDate()
                    });
                }
            }

            return list.ToSortableBindingList();
        }

        private SortableBindingList<PurchaseCounterItemModel> GetSelectedPurchases()
        {
            List<PurchaseCounterItemModel> list = new List<PurchaseCounterItemModel>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var chkCell = row.Cells[ChkColumn.Name] as KryptonDataGridViewCheckBoxCell;
                if (chkCell != null && chkCell.EditedFormattedValue != null && (bool)chkCell.EditedFormattedValue == true)
                {
                    var purchaseId = row.Cells[IdColumn.Name].Value.ToInt();
                    list.Add(new PurchaseCounterItemModel()
                    {
                        PurchaseId = purchaseId,
                        Amount = row.Cells[AmountPaidColumn.Name].Value.ToDecimal(),
                        InvoiceNumber = row.Cells[InvoiceNumberColumn.Name].Value.ToString(),
                        Date = row.Cells[DateColumn.Name].Value.ToDate(),
                        PONumber = this.purchaseController.GetPONumber(purchaseId)
                    });
                }
            }

            foreach (DataGridViewRow row in kryptonDataGridView1.Rows)
            {
                var chkCell = row.Cells[returnChkColumn.Name] as KryptonDataGridViewCheckBoxCell;
                if (chkCell != null && chkCell.EditedFormattedValue != null && (bool)chkCell.EditedFormattedValue == true)
                {
                    list.Add(new PurchaseCounterItemModel()
                    {
                        ReturnId = row.Cells[returnIdColumn.Name].Value.ToInt(),
                        MemoNumber = row.Cells[memoNumberColumn.Name].Value.ToString(),
                        Amount = row.Cells[returnAmountColumn.Name].Value.ToDecimal(),
                        InvoiceNumber = row.Cells[returnInvoiceColumn.Name].Value.ToString(),
                        Date = row.Cells[returnDateColumn.Name].Value.ToDate()
                    });
                }
            }

            return list.ToSortableBindingList();
        }


        private void OkButton_Click(object sender, EventArgs e)
        {
            SelectionCompleteEventArgs args = null;
            if (this.Type == PaymentType.Sales)
            {
                var invoices = GetSelectedInvoices();
                args = new SelectionCompleteEventArgs(invoices);
            }
            else
            {
                var invoices = GetSelectedPurchases();
                args = new SelectionCompleteEventArgs(invoices);
            }
            OnSelectionComplete(args);
            
            this.Close();
        }

        #region Computation
        private void ComputeTotal()
        {
            var total = 0m;
            var totalCount = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var chkCell = row.Cells[ChkColumn.Name] as KryptonDataGridViewCheckBoxCell;
                if (chkCell != null && chkCell.EditedFormattedValue != null && (bool)chkCell.EditedFormattedValue == true)
                {
                    total += row.Cells[AmountPaidColumn.Name].Value != null ?
                        row.Cells[AmountPaidColumn.Name].Value.ToDecimal() : 0m;
                    totalCount++;
                }
            }

            var totalReturns = 0m;
            var totalReturnCount = 0;
            foreach (DataGridViewRow row in kryptonDataGridView1.Rows)
            {
                var chkCell = row.Cells[returnChkColumn.Name] as KryptonDataGridViewCheckBoxCell;
                if (chkCell != null && chkCell.EditedFormattedValue != null && (bool)chkCell.EditedFormattedValue == true)
                {
                    totalReturns += row.Cells[returnAmountColumn.Name].Value != null ?
                        row.Cells[returnAmountColumn.Name].Value.ToDecimal() : 0m;
                    totalReturnCount++;
                }
            }

            InvoiceCountTextbox.Text = totalCount.ToString();
            InvoiceAmountTextbox.Text = total.ToString("N2");
            ReturnCountTextbox.Text = totalReturnCount.ToString();
            ReturnAmountTextbox.Text = totalReturns.ToString("N2");
        }
        #endregion

        #region Validation

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != 10)
                return; 

            var row = dataGridView1.Rows[e.RowIndex];
            if (e.FormattedValue.ToDecimal() > row.Cells[balanceColumn.Name].Value.ToDecimal())
            {
                ClientHelper.ShowErrorMessage("Amount cannot be greater than balance.");
                e.Cancel = true;
            }
        }

        private void kryptonDataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != 6)
                return;

            var row = kryptonDataGridView1.Rows[e.RowIndex];
            if (e.FormattedValue.ToDecimal() > row.Cells[returnBalanceColumn.Name].Value.ToDecimal())
            {
                ClientHelper.ShowErrorMessage("Amount cannot be greater than balance.");
                e.Cancel = true;
            }
        }

        #endregion
    }
}