using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Client.Transactions;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Controls
{
    public partial class PurchaseControl : UserControl, IRefreshable
    {
        private readonly ICustomerController customerController;
        private readonly IPurchaseController purchaseController;
        private readonly IPurchaseReturnController purchaseReturnController;
        private readonly IPaymentDetailController paymentDetailController;

        public PurchaseControl()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.purchaseController = IOC.Container.GetInstance<PurchaseController>();
            this.purchaseReturnController = IOC.Container.GetInstance<PurchaseReturnController>();
            this.paymentDetailController = IOC.Container.GetInstance<PaymentDetailController>();
            
            InitializeComponent();
        }

        #region Load
        private void PurchaseControl_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            LoadAmountTypes();
            LoadPurchaseTypes();
            CustomerDropdown.Focus();
        }

        private void LoadPurchaseTypes()
        {
            string[] types = new string[] { "01-Cash Invoice", "02-Delivery Receipt", "03-Charge Invoice", "04-Cash No Invoice" };

            TypeDropdown.Items.AddRange(types);
            TypeDropdown.SelectedIndex = -1;
        }

        private void LoadCustomers()
        {
            customerDisplayModelBindingSource.DataSource = this.customerController.FetchAllCustomers();
            CustomerDropdown.SelectedIndex = -1;
        }

        private void LoadAmountTypes()
        {
            string[] types = new string[] { "All", "Equal", "Greater Than", "Less Than" };

            AmountTypeDropdown.Items.AddRange(types);
            AmountTypeDropdown.SelectedIndex = -1;
        }

        private void LoadPurchases(PurchaseFilterModel filter)
        {
            int records = 0;
            long elapsedTime = ClientHelper.PerformFetch(() => {

                var purchases = this.purchaseController.FetchPurchaseWithSearch(filter);
                purchaseDisplayModelBindingSource.DataSource = purchases;

                if (purchaseDisplayModelBindingSource.Count == 0)
                    purchaseDetailViewModelBindingSource.DataSource = null;

                records = purchases.Count;
                TotalSalesTextbox.Text = purchases.Sum(a => a.TotalAmount).Value.ToString("Php #,##0.00");
            });

            ((MainForm)this.ParentForm).AttachStatus(records, elapsedTime);
        }

        private void CreateColumns()
        {
            foreach(var info in typeof(PurchasesView).GetProperties())
            {
                dataGridView1.Columns.Add(info.Name, info.Name);
            }
        }

        private PurchaseFilterModel ComposeSearch()
        {
            PurchaseFilterModel model = new PurchaseFilterModel();

            if (CustomerDropdown.SelectedIndex != -1)
                model.CustomerId = (int)CustomerDropdown.SelectedValue;

            if (!string.IsNullOrWhiteSpace(POTextbox.Text))
                model.InvoiceNumber = POTextbox.Text.Trim();

            model.Type = TypeDropdown.SelectedIndex;

            if (AmountTypeDropdown.SelectedIndex != -1)
            {
                if (AmountTypeDropdown.SelectedIndex == 1)
                {
                    model.AmountType = NumericSearchType.Equal;
                    model.AmountValue = AmountTextbox.Text.ToDecimal();
                }
                else if (AmountTypeDropdown.SelectedIndex == 2)
                {
                    model.AmountType = NumericSearchType.GreaterThan;
                    model.AmountValue = AmountTextbox.Text.ToDecimal();
                }
                else if (AmountTypeDropdown.SelectedIndex == 3)
                {
                    model.AmountType = NumericSearchType.LessThan;
                    model.AmountValue = AmountTextbox.Text.ToDecimal();
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

            model.PR = PRTextbox.Text;
            model.PO = POrderTextbox.Text;

            return model;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            PurchaseFilterModel filter = ComposeSearch();
            LoadPurchases(filter);
        }

        #endregion

        #region Details

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = row.Cells[PurchaseIdColumn.Name].Value != null ? (int)row.Cells[PurchaseIdColumn.Name].Value : 0;

                string poNumber = row.Cells[POColumn.Name].Value != null ?
                    row.Cells[POColumn.Name].Value.ToString() : string.Empty;

                PONumberTextbox.Text = poNumber;
                TypeTextbox.Text = row.Cells[TypeColumn.Name].Value != null ?
                    row.Cells[TypeColumn.Name].Value.ToString() : "-";
                TotalAmountTextbox.Text = row.Cells[TotalAmountColumn.Name].Value != null ?
                    ((decimal)row.Cells[TotalAmountColumn.Name].Value).ToString("Php #,##0.00") : "Php 0.00";
                SupplierTextbox.Text = row.Cells[SupplierColumn.Name].Value != null ?
                    row.Cells[SupplierColumn.Name].Value.ToString() : "-";
                PurchaseDateTextbox.Text = row.Cells[PurchaseDateColumn.Name].Value != null ?
                    ((DateTime)row.Cells[PurchaseDateColumn.Name].Value).ToShortDateString() : "-";
                InvoiceDiscountTextbox.Text = row.Cells[InvoiceDiscountColumn.Name].Value != null ?
                    ((decimal)row.Cells[InvoiceDiscountColumn.Name].Value).ToString("Php #,##0.00") : "Php 0.00";

                purchaseDetailViewModelBindingSource.DataSource = this.purchaseController.FetchPurchaseDetails(id);
                DisplayPaymentAndReturnInfo(id, poNumber);
            }
            else
            {
                PONumberTextbox.Text = "-";
                TypeTextbox.Text = "-";
                TotalAmountTextbox.Text = "Php 0.00";
                SupplierTextbox.Text = "-";
                PurchaseDateTextbox.Text = "-";
                InvoiceDiscountTextbox.Text = "Php 0.00";
            }
        }

        private void DisplayPaymentAndReturnInfo(int id, string poNumber)
        {
            if (id != 0)
            {
                var detail = this.paymentDetailController.SearchPurchasePayment(id);
                if (detail != null)
                   purchasePaymentDisplayModelBindingSource.DataSource = detail;
                else
                    ClearPaymentArea();
            }

            if (!string.IsNullOrWhiteSpace(poNumber))
            {
                var returnDetail = this.purchaseReturnController
                    .FetchAllReturnsInPurchase(poNumber);

                if (returnDetail != null)
                {
                    var detail = returnDetail.FirstOrDefault();
                    if (detail != null)
                    {
                        MemoNumberTextbox.Text = detail.MemoNumber;
                        ReturnDateTextbox.Text = detail.ReturnDate.HasValue ? detail.ReturnDate.Value.ToShortDateString() : "-";
                        purchasePaymentDisplayModelBindingSource.DataSource = returnDetail;
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
            purchaseReturnDetailModelBindingSource.DataSource = null;
        }

        private void ClearPaymentArea()
        {
            purchasePaymentDisplayModelBindingSource.DataSource = null;
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
            POTextbox.Clear();
            //AllTypeRB.Checked = true;
            TypeDropdown.SelectedIndex = -1;
            TypeDropdown.ComboBox.SelectedIndex = -1;
            AllDateRB.Checked = true;
            //AllDeliveredRB.Checked = true;
            DateToPicker.Value = DateTime.Now;
            DateFromPicker.Value = DateTime.Now;
            AmountTextbox.Text = "0.00";
            AmountTypeDropdown.ComboBox.SelectedIndex = -1;
            AmountTypeDropdown.SelectedIndex = -1;
            AmountTextbox.Clear();
            PRTextbox.Clear();
            POTextbox.Clear();
        }

        #endregion

        #region Add/Edit Purchases

        private void AddButton_Click(object sender, EventArgs e)
        {
            OpenAddForm(0);
        }

        void form_PurchaseUpdated(object sender, EventArgs e)
        {
            PurchaseFilterModel filter = ComposeSearch();
            LoadPurchases(filter);

            dataGridView1.ClearSelection();

            if (selectedId == 0)
                purchaseDisplayModelBindingSource.Position = purchaseDisplayModelBindingSource.Count - 1;
            else
            {
                PurchasesView item = ((SortableBindingList<PurchasesView>)purchaseDisplayModelBindingSource.DataSource)
                    .FirstOrDefault(a => a.Id == selectedId);
                int index = purchaseDisplayModelBindingSource.IndexOf(item);

                purchaseDisplayModelBindingSource.Position = index;
                dataGridView1.Rows[index].Selected = true;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                int id = (int)dataGridView1.Rows[e.RowIndex].Cells[PurchaseIdColumn.Name].Value;
                if (this.purchaseController.ItemHasPaymentOrReturn(id))
                {
                    if (ClientHelper.ShowConfirmMessage("This purchase is already paid or has a return item. Do you want to continue?") != DialogResult.Yes)
                        return;
                }
                OpenAddForm(id);
            }
        }

        int selectedId;
        private void OpenAddForm(int id)
        {
            selectedId = id;

            Form addForm = this.ParentForm.OwnedForms.FirstOrDefault(a => a.Name == "AddPurchaseForm");
            if (addForm == null)
            {
                AddPurchaseForm form = new AddPurchaseForm();
                form.PurchaseId = id;
                form.Owner = this.ParentForm;
                form.PurchaseUpdated += new PurchaseUpdatedEventHandler(form_PurchaseUpdated);
                form.Show();
            }
            else
            {
                AddPurchaseForm openedForm = (AddPurchaseForm)addForm;
                openedForm.PurchaseId = id;
                openedForm.LoadPurchaseDetails();
                openedForm.Focus();
            }
        }

        #endregion

        #region IRefreshable Members

        public void RefreshView()
        {
            PurchaseFilterModel filter = ComposeSearch();
            LoadPurchases(filter);
        }

        #endregion

        #region Delete

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells[PurchaseIdColumn.Name].Value;
                if (this.purchaseController.ItemHasPaymentOrReturn(id))
                {
                    ClientHelper.ShowErrorMessage("This purchase is already paid or has a return item.");
                    return;
                }

                if (ClientHelper.ShowConfirmMessage("Are you sure you want to delete this purchase?") == DialogResult.Yes)
                {
                    this.purchaseController.DeletePurchase(id);
                    PurchaseFilterModel filter = ComposeSearch();
                    LoadPurchases(filter);
                }
            }
        }

        #endregion
    }
}
