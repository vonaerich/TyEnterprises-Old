using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TY.SPIMS.Entities;
using TY.SPIMS.Controllers;
using TY.SPIMS.Utilities;
using TY.SPIMS.Client.Payment;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.POCOs;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Controls
{
    public partial class PurchasePaymentControl : UserControl, IRefreshable
    {
        private readonly ICustomerController customerController;
        private readonly IPaymentDetailController paymentDetailController;

        public PurchasePaymentControl()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.paymentDetailController = IOC.Container.GetInstance<PaymentDetailController>();

            InitializeComponent();
        }

        #region Load

        private void PurchasePaymentControl_Load(object sender, EventArgs e)
        {
            if (!UserInfo.IsAdmin)
                DeleteButton.Visible = false;

            LoadAmountTypes();
            LoadCustomers();
            CustomerDropdown.Focus();
        }

        private void LoadCustomers()
        {
            var customers = this.customerController.FetchAllCustomers();
            customerDisplayModelBindingSource.DataSource = customers;
            CustomerDropdown.SelectedIndex = -1;
        }

        private void LoadAmountTypes()
        {
            string[] types = new string[] { "All", "Equal", "Greater Than", "Less Than" };

            AmountTypeDropdown.Items.AddRange(types);
            AmountTypeDropdown.SelectedIndex = -1;
        }

        private void LoadPayments(PaymentDetailFilterModel filter)
        {
            int records = 0;
            long elapsedTime = ClientHelper.PerformFetch(() =>
            {
                var results = this.paymentDetailController.FetchPaymentDetailWithSearch(PaymentType.Purchase, filter);
                paymentDetailDisplayModelBindingSource.DataSource = results;

                if (paymentDetailDisplayModelBindingSource.Count == 0)
                    paymentDetailDisplayModelBindingSource.DataSource = null;

                records = results.Count;
            });

            ((MainForm)this.ParentForm).AttachStatus(records, elapsedTime);
        }

        private PaymentDetailFilterModel ComposeSearch()
        {
            PaymentDetailFilterModel model = new PaymentDetailFilterModel();

            if (CustomerDropdown.SelectedIndex != -1)
                model.CustomerId = (int)CustomerDropdown.SelectedValue;

            if (!string.IsNullOrWhiteSpace(VoucherTextbox.Text))
                model.VoucherNumber = VoucherTextbox.Text.Trim();

            if (AmountTypeDropdown.SelectedIndex != -1)
            {
                if (AmountTypeDropdown.SelectedIndex == 1)
                {
                    model.AmountType = NumericSearchType.Equal;
                    model.AmountValue = decimal.Parse(AmountTextbox.Text);
                }
                else if (AmountTypeDropdown.SelectedIndex == 2)
                {
                    model.AmountType = NumericSearchType.GreaterThan;
                    model.AmountValue = decimal.Parse(AmountTextbox.Text);
                }
                else if (AmountTypeDropdown.SelectedIndex == 3)
                {
                    model.AmountType = NumericSearchType.LessThan;
                    model.AmountValue = decimal.Parse(AmountTextbox.Text);
                }
            }
            else
                model.AmountType = NumericSearchType.All;

            if (AllDateRB.Checked)
                model.DateType = DateSearchType.All;
            else
            {
                model.DateType = DateSearchType.DateRange;
                model.DateFrom = DateFromPicker.Value;
                model.DateTo = DateToPicker.Value;
            }

            return model;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            PaymentDetailFilterModel filter = ComposeSearch();
            LoadPayments(filter);
        }

        #endregion

        #region Clear

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearFilter();
        }

        private void ClearFilter()
        {
            VoucherTextbox.Clear();

            AllDateRB.Checked = true;
            DateToPicker.Value = DateTime.Today;
            DateFromPicker.Value = DateTime.Today;

            AmountTypeDropdown.ComboBox.SelectedIndex = -1;
            AmountTypeDropdown.SelectedIndex = -1;
            AmountTextbox.Text = "0.00";
        }

        #endregion

        #region Add/Edit

        private void AddButton_Click(object sender, EventArgs e)
        {
            OpenAddForm(0);
        }

        int selectedId;
        private void OpenAddForm(int id)
        {
            selectedId = id;

            Form addForm = this.ParentForm.OwnedForms.FirstOrDefault(a => a.Name == "AddPurchasePaymentForm");
            if (addForm == null)
            {
                AddPurchasePaymentForm form = new AddPurchasePaymentForm();
                form.PaymentId = id;
                form.Owner = this.ParentForm;
                form.PurchasePaymentUpdated += new PurchasePaymentUpdatedEventHandler(form_PurchasePaymentUpdated);
                form.Show();
            }
            else
            {
                AddPurchasePaymentForm openedForm = (AddPurchasePaymentForm)addForm;
                openedForm.PaymentId = id;
                openedForm.LoadPaymentDetails();
                openedForm.Focus();
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                int id = (int)dataGridView1.Rows[e.RowIndex].Cells[PaymentIdColumn.Name].Value;
                OpenAddForm(id);
            }
        }

        void form_PurchasePaymentUpdated(object sender, EventArgs args)
        {
            PaymentDetailFilterModel filter = ComposeSearch();
            LoadPayments(filter);

            if (selectedId == 0)
                paymentDetailDisplayModelBindingSource.Position = paymentDetailDisplayModelBindingSource.Count - 1;
            else
            {
                PaymentDetailDisplayModel item = ((SortableBindingList<PaymentDetailDisplayModel>)paymentDetailDisplayModelBindingSource.DataSource)
                    .FirstOrDefault(a => a.Id == selectedId);
                int index = paymentDetailDisplayModelBindingSource.IndexOf(item);

                paymentDetailDisplayModelBindingSource.Position = index;
                dataGridView1.Rows[index].Selected = true;
            }
        }

        #endregion

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells[PaymentIdColumn.Name].Value;

                decimal amountPaid = row.Cells[TotalAmountColumn.Name].Value != null ?
                    (decimal)row.Cells[TotalAmountColumn.Name].Value : 0;

                VoucherNumberTextbox.Text = row.Cells[VoucherNumberColumn.Name].Value != null ?
                    row.Cells[VoucherNumberColumn.Name].Value.ToString() : "-";
                AmountPaidTextbox.Text = amountPaid.ToString("Php #,##0.00");
                SupplierTextbox.Text = row.Cells[SupplierColumn.Name].Value != null ?
                    row.Cells[SupplierColumn.Name].Value.ToString() : "-";
                DateTextbox.Text = row.Cells[DateColumn.Name].Value != null ?
                    ((DateTime)row.Cells[DateColumn.Name].Value).ToShortDateString() : "-";
                CashTextbox.Text = row.Cells[CashColumn.Name].Value != null ?
                    ((decimal)row.Cells[CashColumn.Name].Value).ToString("Php #,##0.00") : "Php 0.00";
                CheckTextbox.Text = row.Cells[CheckColumn.Name].Value != null ?
                    ((decimal)row.Cells[CheckColumn.Name].Value).ToString("Php #,##0.00") : "Php 0.00";

                PaymentDetail details = this.paymentDetailController.FetchPaymentDetailById(id);

                if (details != null)
                {
                    purchaseDisplayModelBindingSource.DataSource = null;
                    if (details.PurchasePayments.Any())
                    {
                        var purchases = details.PurchasePayments
                            .Where(a => a.PurchaseId != null)
                            .Select(a => new PurchaseDisplayModel()
                            {
                                PONumber = a.Purchase.PONumber,
                                PurchaseDate = a.Purchase.Date.Value,
                                TotalAmount = a.Amount.HasValue ? a.Amount.Value : 0
                            });
                        purchaseDisplayModelBindingSource.DataSource = purchases;
                    }
                    dataGridView2.ClearSelection();

                    checkColumnModelBindingSource.DataSource = null;
                    List<Check> checkList = details.Check.ToList();
                    if (details.Check.Any())
                    {
                        var checks = details.Check.Select(a => new CheckColumnModel()
                        {
                            CheckNumber = a.CheckNumber,
                            Bank = a.Bank,
                            Branch = a.Branch,
                            Amount = a.Amount.HasValue ? a.Amount.Value : 0
                        });
                        checkColumnModelBindingSource.DataSource = checks;
                    }
                    dataGridView3.ClearSelection();

                    DebitTextbox.Text = details.PurchasePayments.Where(a => a.PurchaseReturnDetailId != null)
                        .Sum(a => a.Amount).GetValue().ToString("Php #,##0.00");
                    AmountDueTextbox.Text = details.PurchasePayments.Where(a => a.PurchaseId != null).Sum(a => a.Amount).GetValue().ToString("Php #,##0.00");
                    DiscountTextbox.Text = details.Discount.GetValue().ToString("Php #,##0.00");
                    TaxTextbox.Text = details.WitholdingTax.GetValue().ToString("Php #,##0.00");

                    decimal totalAmountDue = details.TotalAmountDue.GetValue();
                    TotalDueTextbox.Text = totalAmountDue.ToString("Php #,##0.00");

                    if (amountPaid == 0)
                        StatusTextbox.Text = "Not Paid";
                    else if (amountPaid == totalAmountDue)
                        StatusTextbox.Text = "Fully Paid";
                    else if (amountPaid < totalAmountDue)
                        StatusTextbox.Text = "Partially Paid";
                }
            }
            else
            {
                VoucherNumberTextbox.Text = "-";
                AmountPaidTextbox.Text = "Php 0.00";
                SupplierTextbox.Text = "-";
                DateTextbox.Text = "-";
                CashTextbox.Text = "Php 0.00";
                CheckTextbox.Text = "Php 0.00";

                purchaseDisplayModelBindingSource.DataSource = null;
                checkColumnModelBindingSource.DataSource = null;
                AmountDueTextbox.Text = "Php 0.00";
                DebitTextbox.Text = "Php 0.00";
                DiscountTextbox.Text = "Php 0.00";
                TaxTextbox.Text = "Php 0.00";
                TotalDueTextbox.Text = "Php 0.00";
            }
        }

        #region IRefreshable Members

        public void RefreshView()
        {
            PaymentDetailFilterModel filter = ComposeSearch();
            LoadPayments(filter);
        }

        #endregion

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count < 1)
            {
                return;
            }

            if (!UserInfo.IsAdmin)
            {
                ClientHelper.ShowErrorMessage("You are not authorized to delete this record.");
                return;
            }

            if (ClientHelper.ShowConfirmMessage("Are you sure you want to delete this payment?") == DialogResult.Yes)
            {
                var row = dataGridView1.SelectedRows[0];
                int id = (int)row.Cells[PaymentIdColumn.Name].Value;

                this.paymentDetailController.DeletePaymentDetail(id);
                ClientHelper.ShowSuccessMessage("Payment deleted successfully.");

                this.LoadPayments(this.ComposeSearch());
            }
        }
    }
}
