using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TY.SPIMS.Client.Approval;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Client.Helper.CodeGenerator;
using TY.SPIMS.Client.Helper.Export;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Payment
{
    public partial class AddSalesPaymentForm : InputDetailsForm
    {
        private readonly ICustomerController customerController;
        private readonly IPaymentDetailController paymentDetailController;

        public int PaymentId { get; set; }

        #region Events

        public event SalesPaymentUpdatedEventHandler SalesPaymentUpdated;
        protected void OnSalesPaymentUpdated(EventArgs args)
        {
            SalesPaymentUpdatedEventHandler handler = SalesPaymentUpdated;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        private BackgroundWorker worker = new BackgroundWorker();
        private BackgroundWorker exportWorker = new BackgroundWorker();
        public AddSalesPaymentForm()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.paymentDetailController = IOC.Container.GetInstance<PaymentDetailController>();

            InitializeComponent();

            this.AutoValidate = AutoValidate.Disable;

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            exportWorker.DoWork += new DoWorkEventHandler(exportWorker_DoWork);
            exportWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(exportWorker_RunWorkerCompleted);
        }

        void exportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ToggleButtons(true);
            LoadImage.Visible = false;

            ClientHelper.ShowSuccessMessage("Export complete.");
        }

        void exportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ORExportObject exportObject = (ORExportObject)e.Argument;
                IExportStrategy strategy = new ORExportStrategy(exportObject);

                var exporter = new ReportExporter(strategy);
                exporter.ExportReport();
            }
            catch (Exception ex)
            {
                ClientHelper.LogException(ex);
                ClientHelper.ShowErrorMessage("An error occurred while exporting. Please try again.");
            }
        }

        #region Worker

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (PaymentId == 0)
                ClientHelper.ShowSuccessMessage("Payment successfully added.");
            else
                ClientHelper.ShowSuccessMessage("Payment successfully updated.");

            SetupForm();

            if (SalesPaymentUpdated != null)
                SalesPaymentUpdated(sender, new EventArgs());

            LoadImage.Visible = false;
            ToggleButtons(true);
            CustomerDropdown.Focus();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                PaymentDetail payment = (PaymentDetail)e.Argument;

                if (payment.Id == 0)
                    this.paymentDetailController.InsertPaymentDetail(payment);
                else
                    this.paymentDetailController.UpdatePaymentDetail(payment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Load

        private void AddSalesPaymentForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            SetupForm();
        }

        private void SetupForm()
        {
            if (PaymentId != 0)
            {
                LoadPaymentDetails();
                CustomerDropdown.Enabled = false;
            }
            else
            {
                GenerateCode();
                ClearAll();
                CustomerDropdown.Enabled = true;
            }
        }

        private void GenerateCode()
        {
            var generator = new CodeGenerator(new ORCodeGenerator());
            VoucherNumberTextbox.Text = generator.GenerateCode();
        }

        public void LoadPaymentDetails()
        {
            PaymentDetail detail = this.paymentDetailController.FetchPaymentDetailById(PaymentId);
            if (detail != null)
            {
                #region Details
                CustomerDropdown.SelectedValue = detail.CustomerId;

                //Display details
                VoucherNumberTextbox.Text = detail.VoucherNumber;
                InvoiceCheckbox.Checked = detail.VoucherNumber.StartsWith("S");
                PaymentDatePicker.Value = detail.PaymentDate.HasValue ? detail.PaymentDate.Value : DateTime.Today;
                DiscountTextbox.Value = detail.Discount.HasValue ? detail.Discount.Value : 0;
                RemarksTextbox.Text = detail.Remarks;
                MiscCheckbox.Checked = !string.IsNullOrWhiteSpace(detail.Misc) || (detail.MiscAmount != null && detail.MiscAmount.Value != 0);
                MiscTextbox.Text = detail.Misc;
                MiscAmountTextbox.Value = detail.MiscAmount.GetValue();

                if (detail.WitholdingTax != null && detail.WitholdingTax.Value > 0)
                {
                    TaxCheckbox.Checked = true;
                    WitholdingTaxTextbox.Text = detail.WitholdingTax.Value.ToString();

                    if (detail.GovtForm != null)
                        GovtFormCheckbox.Checked = detail.GovtForm.Value;
                }

                //Display invoices
                invoiceList.Clear();
                if (detail.SalesPayments.Any())
                {
                    foreach(var item in detail.SalesPayments)
                    {
                        var newItem = new SalesCounterItemModel()
                        {
                            SaleId = item.SalesId,
                            ReturnId = item.SalesReturnDetailId,
                            InvoiceNumber = item.SalesId != null ? item.Sale.InvoiceNumber : "-",
                            MemoNumber = item.SalesReturnDetailId != null ? item.SalesReturnDetail.SalesReturn.MemoNumber : string.Empty,
                            Amount = item.Amount,
                            Date = item.SalesId != null ? item.Sale.Date : item.SalesReturnDetail.SalesReturn.ReturnDate
                        };
                        invoiceList.Add(newItem);
                    }
                }
                BindInvoices();
                ComputeTotalAmountDue();

                #endregion

                #region Cash/Check payment

                //If cash payment
                if (detail.TotalCashPayment.HasValue)
                {
                    if (detail.TotalCashPayment.Value != 0m)
                    {
                        CashPaymentCheckbox.Checked = true;
                        TotalCashTextbox.Text = detail.TotalCashPayment.Value.ToString("N2");
                    }
                    else
                    {
                        CashPaymentCheckbox.Checked = false;
                        TotalCashTextbox.Text = "0.00";
                    }
                }

                //If check payment
                if (detail.TotalCheckPayment.HasValue)
                {
                    if (detail.TotalCheckPayment.Value != 0m)
                    {
                        CheckPaymentCheckbox.Checked = true;
                        TotalCheckTextbox.Text = detail.TotalCheckPayment.Value.ToString("N2");

                        //Display Checks in Grid
                        checksList = this.paymentDetailController.FetchPaymentChecks(PaymentId);
                        
                    }
                    else
                    {
                        CheckPaymentCheckbox.Checked = false;
                        TotalCheckTextbox.Text = "0.00";

                        checksList = null;
                    }

                    BindChecks();
                }

                //Compute Payment
                ComputeTotalPayment();

                #endregion
            }
            else
            {
                ClearAll();
                SetupForm();
            }
        }

        private void LoadCustomers()
        {
            customerDisplayModelBindingSource.DataSource = this.customerController.FetchAllCustomers();
            CustomerDropdown.SelectedIndex = -1;
            CustomerDropdown.ComboBox.SelectedIndex = -1;
        }

        #endregion

        #region Checks

        private void AddCheckLink_LinkClicked(object sender, EventArgs e)
        {
            AddCheckForm form = new AddCheckForm();
            form.MdiParent = this.MdiParent;
            form.CheckAdded += new CheckAddedEventHandler(form_CheckAdded);
            form.Show();
        }

        List<CheckColumnModel> checksList = new List<CheckColumnModel>();
        void form_CheckAdded(object sender, CheckAddedEventArgs e)
        {
            if (checksList == null)
                checksList = new List<CheckColumnModel>();

            checksList.Add(e.CheckModel);
            BindChecks();

            ComputeTotalCheckAmount();
        }

        private void BindChecks()
        {
            checkColumnModelBindingSource.DataSource = null;
            checkColumnModelBindingSource.DataSource = checksList;
        }

        private void ComputeTotalCheckAmount()
        {
            decimal total = 0m;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells[4].Value != null)
                {
                    decimal amount = (decimal)row.Cells[4].Value;
                    total += amount;
                }
            }

            TotalCheckTextbox.Text = total.ToString("N2");
            ComputeTotalPayment();
        }

        private void dataGridView2_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            ComputeTotalCheckAmount();
        }

        #endregion

        #region Invoices

        private void CustomerDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            invoiceList.Clear();
            BindInvoices();

            creditIds.Clear();

            if (CustomerDropdown.SelectedIndex != -1)
                AddButton.Enabled = true;
            else
                AddButton.Enabled = false;
        }

        SortableBindingList<SalesCounterItemModel> invoiceList = new SortableBindingList<SalesCounterItemModel>();
        List<CreditDetailModel> creditIds = new List<CreditDetailModel>();
        private void BindInvoices()
        {
            salesCounterItemModelBindingSource.DataSource = null;

            if (invoiceList.Any())
                salesCounterItemModelBindingSource.DataSource = invoiceList;
        }

        private decimal totalAmountDue = 0m;
        private decimal totalDue = 0m;
        private void ComputeTotalAmountDue()
        {
            if (invoiceList != null)
            {
                totalDue = invoiceList.Where(a => a.SaleId != null).Sum(a => a.Amount).GetValueOrDefault();

                decimal credit = invoiceList.Where(a => a.ReturnId != null).Sum(a => a.Amount).GetValue();
                CreditTextbox.Text = credit.ToString("N2");

                decimal tax = 0m;
                decimal discount = DiscountTextbox.Value;

                if (TaxCheckbox.Checked)
                    tax = (totalDue - credit - discount) / 1.12m * 0.1m * 0.1m;

                totalAmountDue = Math.Round(totalDue - credit - tax - discount, 2);
                AmountDueTextbox.Text = totalDue.ToString("N2");
                WitholdingTaxTextbox.Text = tax.ToString("N2");
                TotalAmountDueTextbox.Text = totalAmountDue.ToString("N2");
            }
        }

        private void TaxCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ComputeTotalAmountDue();

            if (TaxCheckbox.Checked)
                GovtFormCheckbox.Enabled = true;
            else
            {
                GovtFormCheckbox.Checked = false;
                GovtFormCheckbox.Enabled = false;
            }
        }

        #endregion

        #region Checkbox Handling

        private void CashPaymentCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCashPayments();
        }

        private void ToggleCashPayments()
        {
            if (CashPaymentCheckbox.Checked)
                TotalCashTextbox.Enabled = true;
            else
            {
                TotalCashTextbox.Enabled = false;
                TotalCashTextbox.Text = "0.00";

                ComputeTotalPayment();
            }
        }

        private void CheckPaymentCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCheckPayments();
        }

        private void ToggleCheckPayments()
        {
            if (CheckPaymentCheckbox.Checked)
                AddCheckLink.Enabled = true;
            else
            {
                AddCheckLink.Enabled = false;

                checkColumnModelBindingSource.DataSource = null;
                if (checksList != null)
                    checksList.Clear();

                ComputeTotalCheckAmount();
            }
        }

        #endregion

        #region Total

        private void TotalCashTextbox_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TotalCashTextbox.Text))
                ComputeTotalPayment();
            else
                TotalCashTextbox.Text = "0.00";
        }

        private decimal totalPayment = 0m;
        private void ComputeTotalPayment()
        {
            decimal totalCash = CashPaymentCheckbox.Checked ? TotalCashTextbox.Text.ToDecimal() : 0;
            decimal totalCheck = CheckPaymentCheckbox.Checked ? TotalCheckTextbox.Text.ToDecimal() : 0;

            totalPayment = totalCash + totalCheck;
            TotalAmountTextbox.Text = totalPayment.ToString("N2");
        }

        #endregion

        #region Save

        int? approver = null;
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
                ClientHelper.ShowRequiredMessage("OR Number");
            else if (dataGridView1.SelectedRows.Count == 0)
                ClientHelper.ShowErrorMessage("No items to pay.");
            else if (TotalAmountDueTextbox.Text.ToDecimal() < 0)
                ClientHelper.ShowErrorMessage("Amount due is less than 0.");
            else if (hasDuplicate)
                ClientHelper.ShowDuplicateMessage("OR Number");
            else if (DiscountTextbox.Value != 0 && !UserInfo.IsAdmin)
            {
                ApprovalForm f = new ApprovalForm();
                f.ApprovalDone +=new ApprovalDoneEventHandler(form_ApprovalDone);
                f.Show();
            }
            else
                SavePayment();
        }

        void form_ApprovalDone(object sender, ApprovalEventArgs e)
        {
            if (e.ApproverId == 0)
                ClientHelper.ShowErrorMessage("Invalid approver.");
            else
            {
                approver = e.ApproverId;
                SavePayment();
            }
        }

        private void SavePayment()
        {
            bool willContinue = true;

            //If payment is more than due, confirm
            if (totalPayment > totalAmountDue)
            {
                if (ClientHelper.ShowConfirmMessage("Total payment is greater than total amount due.\nAre you sure you want to continue?")
                    != DialogResult.Yes)
                    willContinue = false;
            }

            if (willContinue)
            {
                PaymentDetail payment = new PaymentDetail() 
                {
                    Id = PaymentId,
                    CustomerId = (int)CustomerDropdown.SelectedValue,
                    VoucherNumber = VoucherNumberTextbox.Text,
                    PaymentDate = PaymentDatePicker.Value,
                    RecordedBy = UserInfo.UserId,
                    ApprovedBy = this.approver,
                    TotalCashPayment = CashPaymentCheckbox.Checked ? TotalCashTextbox.Text.ToDecimal() : 0,
                    TotalCheckPayment = CheckPaymentCheckbox.Checked ? TotalCheckTextbox.Text.ToDecimal() : 0,
                    IsDeleted = false,
                    WitholdingTax = WitholdingTaxTextbox.Text.ToDecimal(),
                    GovtForm = GovtFormCheckbox.Checked,
                    TotalAmountDue = totalAmountDue,
                    Discount = DiscountTextbox.Value,
                    Remarks = RemarksTextbox.Text,
                    Misc = MiscTextbox.Text,
                    MiscAmount = MiscAmountTextbox.Value
                };

                if (invoiceList != null)
                {
                    invoiceList.ToList().ForEach(a => 
                            payment.SalesPayments.Add(new SalesPayment() {
                                Amount = a.Amount,
                                SalesId = a.SaleId,
                                SalesReturnDetailId = a.ReturnId,
                            })
                        );
                }

                if (CheckPaymentCheckbox.Checked)
                {
                    checksList.ForEach(a => 
                        payment.Check.Add(new Check() { 
                            Amount = a.Amount,
                            Bank = a.Bank,
                            Branch = a.Branch,
                            CheckDate = a.CheckDate,
                            CheckNumber = a.CheckNumber,
                            ClearingDate = a.ClearingDate,
                            IsDeleted = false
                        }));
                }

                ToggleButtons(false);
                LoadImage.Visible = true;

                if (!worker.IsBusy)
                    worker.RunWorkerAsync(payment);
            }
        }

        private void ToggleButtons(bool enabled)
        {
            ExportButton.Enabled = enabled;
            SaveButton.Enabled = enabled;
            ClearButton.Enabled = enabled;
        }

        #endregion

        #region Clear All

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            CustomerDropdown.ComboBox.SelectedIndex = -1;
            CustomerDropdown.SelectedIndex = -1;

            if (PaymentId == 0)
            {
                PaymentDatePicker.Value = DateTime.Today;
                RemarksTextbox.Clear();
                TaxCheckbox.Checked = false;
                DiscountTextbox.Value = 0;
                CreditTextbox.Text = "0.00";
                AmountDueTextbox.Text = "0.00";
                TotalAmountDueTextbox.Text = "0.00";
                InvoiceCheckbox.Checked = false;
                MiscCheckbox.Checked = false;
                CashCheckbox.Checked = false;

                CashPaymentCheckbox.Checked = false;
                TotalCashTextbox.Text = "0.00";

                CheckPaymentCheckbox.Checked = false;
                ComputeTotalPayment();
            }
            else
                LoadPaymentDetails();

            CustomerDropdown.Focus();
        }

        #endregion

        #region Validation

        private void VoucherNumberTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(VoucherNumberTextbox.Text))
                e.Cancel = true;
        }

        #region Check Duplicate

        private bool hasDuplicate = false;
        private void VoucherNumberTextbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(VoucherNumberTextbox.Text))
                hasDuplicate = DuplicateChecker.CodeHasDuplicate(CodeType.SalesPayment, VoucherNumberTextbox.Text.Trim(), PaymentId);

            if (hasDuplicate)
                VoucherNumberTextbox.StateCommon.Content.Color1 = Color.Red;
            else
                VoucherNumberTextbox.StateCommon.Content.Color1 = Color.Black;
        }

        #endregion

        private void DiscountTextbox_ValueChanged(object sender, EventArgs e)
        {
            ComputeTotalAmountDue();
        }

        #endregion

        private void AddButton_Click(object sender, EventArgs e)
        {
            //if (this.PaymentId != 0 && 
            //    this.invoiceList.Count > 0 &&
            //    ClientHelper.ShowConfirmMessage("Adding new invoices will clear the current list. Do you want to continue?") != DialogResult.Yes)
            //    return;

            if (CustomerDropdown.SelectedIndex != -1)
            {
                //if(revertCount == 0)
                //     RevertOriginalList();
                //this.invoiceList.Clear();
                //BindInvoices();

                //AddInvoiceForm form = new AddInvoiceForm();
                //form.CustomerId = CashCheckbox.Checked ? 0 : (int)CustomerDropdown.SelectedValue;
                //form.CustomerName = CustomerDropdown.Text;
                AddCounterItemForm form = new AddCounterItemForm() { 
                    CustomerId = CashCheckbox.Checked ? 0 : (int)CustomerDropdown.SelectedValue,
                    CustomerName = CustomerDropdown.Text,
                    InvoiceIds = invoiceList.Where(a => a.SaleId != null && a.SaleId != 0).ToDictionary(a => a.SaleId, b => b.Amount),
                    ReturnIds = invoiceList.Where(a => a.ReturnId != null && a.ReturnId != 0).ToDictionary(a => a.ReturnId, b => b.Amount),
                    Type = PaymentType.Sales
                };
                form.SelectionComplete += new EventHandler<SelectionCompleteEventArgs>(form_SelectionComplete);
                form.Show();
            }
        }

        void form_SelectionComplete(object sender, SelectionCompleteEventArgs args)
        {
            if (args.SelectedSalesItems != null)
            {
                this.invoiceList = args.SelectedSalesItems;
                BindInvoices();
                ComputeTotalAmountDue();
            }
        }

        private void InvoiceCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (InvoiceCheckbox.Checked)
                VoucherNumberTextbox.Text = VoucherNumberTextbox.Text.StartsWith("O") ? 
                    string.Format("S{0}", VoucherNumberTextbox.Text) : VoucherNumberTextbox.Text;
            else
                VoucherNumberTextbox.Text = VoucherNumberTextbox.Text.StartsWith("S") ?
                    VoucherNumberTextbox.Text.Substring(1) : VoucherNumberTextbox.Text;
        }

        int revertCount = 0;
        private void RevertOriginalList()
        {
            if (invoiceList.Count > 0)
            {
                foreach (var item in invoiceList)
                {
                    if (item.SaleId != null)
                        this.paymentDetailController.RevertItem(PaymentRevertType.Sale, this.PaymentId, item.SaleId.Value, item.Amount.GetValue());
                    else if (item.ReturnId != null)
                        this.paymentDetailController.RevertItem(PaymentRevertType.SalesReturn, this.PaymentId, item.ReturnId.Value, item.Amount.GetValue());
                }
            }
            revertCount++;
        }

        private ORExportObject CreateExportObject()
        {
            ORExportObject obj = new ORExportObject()
            {
                Code = VoucherNumberTextbox.Text,
                Customer = CustomerDropdown.Text,
                Remarks = RemarksTextbox.Text,
                Items = invoiceList.ToList(),
                WitholdingTax = WitholdingTaxTextbox.Text.ToDecimal(),
                Discount = DiscountTextbox.Value,
                Checks = checksList,
                Cash = TotalCashTextbox.Text.ToDecimal(),
                Month = PaymentDatePicker.Value.ToString("MMMM")
            };

            return obj;
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (invoiceList.Count > 0)
            {
                ToggleButtons(false);
                LoadImage.Visible = true;

                ORExportObject obj = CreateExportObject();
                exportWorker.RunWorkerAsync(obj);
            }
            else
                ClientHelper.ShowErrorMessage("No items to export.");
        }

        private void MiscCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (MiscCheckbox.Checked)
            {
                ToggleMisc(true);
            }
            else
            {
                ToggleMisc(false);
                MiscAmountTextbox.Value = 0;
                MiscTextbox.Text = string.Empty;
            }
        }

        private void ToggleMisc(bool enabled)
        {
            MiscAmountTextbox.Enabled = enabled;
            MiscTextbox.Enabled = enabled;
        }
    }

    public delegate void SalesPaymentUpdatedEventHandler(object sender, EventArgs args);
}