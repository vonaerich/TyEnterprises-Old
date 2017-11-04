using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TY.SPIMS.Client.Approval;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Client.Helper.CodeGenerator;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Client.Helper.Export;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Payment
{
    public partial class AddPurchasePaymentForm : InputDetailsForm
    {
        private readonly ICustomerController customerController;
        private readonly IPaymentDetailController paymentDetailController;

        public int PaymentId { get; set; }

        #region Events

        public event PurchasePaymentUpdatedEventHandler PurchasePaymentUpdated;
        protected void OnPurchasePaymentUpdated(EventArgs args)
        {
            PurchasePaymentUpdatedEventHandler handler = PurchasePaymentUpdated;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        private BackgroundWorker worker = new BackgroundWorker();
        private BackgroundWorker exportWorker = new BackgroundWorker();
        public AddPurchasePaymentForm()
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
                VoucherExportObject exportObject = (VoucherExportObject)e.Argument;
                IExportStrategy strategy = new VoucherExportStrategy(exportObject);

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

            if (PurchasePaymentUpdated != null)
                PurchasePaymentUpdated(sender, new EventArgs());

            LoadImage.Visible = false;
            ToggleButtons(true);
            SupplierDropdown.Focus();
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

        private void AddPurchasePaymentForm_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            SetupForm();
        }

        private void SetupForm()
        {
            if (PaymentId != 0)
            {
                LoadPaymentDetails();
                SupplierDropdown.Enabled = false;
            }
            else
            {
                //GenerateCode();
                SupplierDropdown.Enabled = true;
                ClearAll();
            }
        }

        private void GenerateCode()
        {
            var generator = new CodeGenerator(new VoucherPurchaseCodeGenerator());
            VoucherNumberTextbox.Text = generator.GenerateCode();
        }

        public void LoadPaymentDetails()
        {
            PaymentDetail detail = this.paymentDetailController.FetchPaymentDetailById(PaymentId);
            if (detail != null)
            {
                #region Details
                SupplierDropdown.SelectedValue = detail.CustomerId;

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
                poList.Clear();
                if (detail.PurchasePayments.Any())
                {
                    foreach (var item in detail.PurchasePayments)
                    {
                        poList.Add(new PurchaseCounterItemModel()
                        {
                            PurchaseId = item.PurchaseId,
                            ReturnId = item.PurchaseReturnDetailId,
                            InvoiceNumber = item.PurchaseId != null ? item.Purchase.PONumber : "-",
                            MemoNumber = item.PurchaseReturnDetailId != null ? item.PurchaseReturnDetail.PurchaseReturn.MemoNumber : string.Empty,
                            Amount = item.Amount,
                            Date = item.PurchaseId != null ? item.Purchase.Date : item.PurchaseReturnDetail.PurchaseReturn.ReturnDate
                        });
                    }
                }
                BindPOs();
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

        private void LoadSuppliers()
        {
            customerDisplayModelBindingSource.DataSource = this.customerController.FetchAllCustomers();
            SupplierDropdown.SelectedIndex = -1;
            SupplierDropdown.ComboBox.SelectedIndex = -1;
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

        #region POs

        private void SupplierDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            poList.Clear();
            BindPOs();

            debitIds.Clear();

            if (SupplierDropdown.SelectedIndex != -1)
                AddButton.Enabled = true;
            else
                AddButton.Enabled = false;
        }

        SortableBindingList<PurchaseCounterItemModel> poList = new SortableBindingList<PurchaseCounterItemModel>();
        List<DebitDetailModel> debitIds = new List<DebitDetailModel>();
        private void BindPOs()
        {
            purchaseDisplayModelBindingSource.DataSource = null;

            if (poList.Any())
                purchaseDisplayModelBindingSource.DataSource = poList;
        }

        private decimal totalAmountDue = 0m;
        private decimal totalDue = 0m;
        private void ComputeTotalAmountDue()
        {
            if (poList != null)
            {
                totalDue = poList.Where(a => a.PurchaseId != null).Sum(a => a.Amount).GetValueOrDefault();

                decimal debit = poList.Where(a => a.ReturnId != null).Sum(a => a.Amount).GetValue();
                DebitTextbox.Text = debit.ToString("N2");

                decimal tax = 0m;
                decimal discount = DiscountTextbox.Value;

                if (TaxCheckbox.Checked)
                    tax = (totalDue - debit - discount) / 1.12m * 0.1m * 0.1m;

                totalAmountDue = Math.Round(totalDue - debit - tax - discount, 2);
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
            decimal totalCash = CashPaymentCheckbox.Checked ? decimal.Parse(TotalCashTextbox.Text) : 0;
            decimal totalCheck = CheckPaymentCheckbox.Checked ? decimal.Parse(TotalCheckTextbox.Text) : 0;

            totalPayment = totalCash + totalCheck;
            TotalAmountTextbox.Text = totalPayment.ToString("N2");
        }

        #endregion

        #region Save
        int? approver = null;
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
                ClientHelper.ShowRequiredMessage("Counter Number");
            else if (dataGridView1.SelectedRows.Count == 0)
                ClientHelper.ShowErrorMessage("No items to pay.");
            else if (decimal.Parse(TotalAmountDueTextbox.Text) < 0)
                ClientHelper.ShowErrorMessage("Amount due is less than 0.");
            else if (hasDuplicate)
                ClientHelper.ShowDuplicateMessage("Counter Number");
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
                if (ClientHelper.ShowConfirmMessage("Total payment is greater than total amount due.\nDo you want to continue?")
                    != DialogResult.Yes)
                    willContinue = false;
            }

            if (willContinue)
            {
                PaymentDetail payment = new PaymentDetail()
                {
                    Id = PaymentId,
                    CustomerId = (int)SupplierDropdown.SelectedValue,
                    VoucherNumber = VoucherNumberTextbox.Text,
                    PaymentDate = PaymentDatePicker.Value,
                    RecordedBy = UserInfo.UserId,
                    ApprovedBy = approver,
                    TotalCashPayment = CashPaymentCheckbox.Checked ? decimal.Parse(TotalCashTextbox.Text) : 0,
                    TotalCheckPayment = CheckPaymentCheckbox.Checked ? decimal.Parse(TotalCheckTextbox.Text) : 0,
                    IsDeleted = false,
                    WitholdingTax = WitholdingTaxTextbox.Text.ToDecimal(),
                    GovtForm = GovtFormCheckbox.Checked,
                    TotalAmountDue = totalAmountDue,
                    Discount = DiscountTextbox.Value,
                    Remarks = RemarksTextbox.Text,
                    Misc = MiscTextbox.Text,
                    MiscAmount = MiscAmountTextbox.Value
                };

                if (poList != null)
                {
                    poList.ToList().ForEach(a =>
                        payment.PurchasePayments.Add(new PurchasePayment() { 
                            Amount = a.Amount,
                            PurchaseId = a.PurchaseId,
                            PurchaseReturnDetailId = a.ReturnId
                        })
                    );
                }

                if (CheckPaymentCheckbox.Checked)
                {
                    checksList.ForEach(a =>
                        payment.Check.Add(new Check()
                        {
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
            SupplierDropdown.ComboBox.SelectedIndex = -1;
            SupplierDropdown.SelectedIndex = -1;

            if (PaymentId == 0)
            {
                VoucherNumberTextbox.Clear();
                PaymentDatePicker.Value = DateTime.Today;
                TaxCheckbox.Checked = false;
                DiscountTextbox.Value = 0;
                DebitTextbox.Text = "0.00";
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

            VoucherNumberTextbox.Focus();
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
                hasDuplicate = DuplicateChecker.CodeHasDuplicate(CodeType.PurchasePayment, VoucherNumberTextbox.Text.Trim(), PaymentId);

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
            //    this.poList.Count > 0 &&
            //    ClientHelper.ShowConfirmMessage("Adding new invoices will clear the current list. Do you want to continue?") != DialogResult.Yes)
            //    return;

            if (SupplierDropdown.SelectedIndex != -1)
            {
                //if (revertCount == 0)
                //    RevertOriginalList();
                //this.poList.Clear();
                //BindPOs();

                //AddPOForm form = new AddPOForm();
                //form.SupplierId = CashCheckbox.Checked ? 0 : (int)SupplierDropdown.SelectedValue;
                //form.SupplierName = SupplierDropdown.Text;
                AddCounterItemForm form = new AddCounterItemForm()
                {
                    CustomerId = CashCheckbox.Checked ? 0 : (int)SupplierDropdown.SelectedValue,
                    CustomerName = SupplierDropdown.SelectedText,
                    InvoiceIds = poList.Where(a => a.PurchaseId != null && a.PurchaseId != 0).ToDictionary(a => a.PurchaseId, b => b.Amount),
                    ReturnIds = poList.Where(a => a.ReturnId != null && a.ReturnId != 0)
                        .ToDictionary(a => a.ReturnId, b => b.Amount),
                    Type = PaymentType.Purchase
                };
                form.SelectionComplete += new EventHandler<SelectionCompleteEventArgs>(form_SelectionComplete);
                form.Show();
            }
        }

        void form_SelectionComplete(object sender, SelectionCompleteEventArgs args)
        {
            if (args.SelectedPurchaseItems != null)
            {
                this.poList = args.SelectedPurchaseItems;
                BindPOs();
                ComputeTotalAmountDue();
            }
        }

        private void InvoiceCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (InvoiceCheckbox.Checked)
                VoucherNumberTextbox.Text = VoucherNumberTextbox.Text.StartsWith("C") ?
                    string.Format("S{0}", VoucherNumberTextbox.Text) : VoucherNumberTextbox.Text;
            else
                VoucherNumberTextbox.Text = VoucherNumberTextbox.Text.StartsWith("S") ?
                    VoucherNumberTextbox.Text.Substring(1) : VoucherNumberTextbox.Text;
        }

        int revertCount = 0;
        private void RevertOriginalList()
        {
            if (poList.Count > 0)
            {
                foreach (var item in poList)
                {
                    if (item.PurchaseId != null)
                        this.paymentDetailController.RevertItem(PaymentRevertType.Purchase, this.PaymentId, item.PurchaseId.Value, item.Amount.GetValue());
                    else if (item.ReturnId != null)
                        this.paymentDetailController.RevertItem(PaymentRevertType.PurchaseReturn, this.PaymentId, item.ReturnId.Value, item.Amount.GetValue());
                }
            }
            revertCount++;
        }

        private VoucherExportObject CreateExportObject()
        {
            VoucherExportObject obj = new VoucherExportObject()
            {
                Code = VoucherNumberTextbox.Text,
                Supplier = SupplierDropdown.Text,
                Remarks = RemarksTextbox.Text,
                Items = poList.ToList(),
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
            if (poList.Count > 0)
            {
                ToggleButtons(false);
                LoadImage.Visible = true;

                VoucherExportObject obj = CreateExportObject();
                exportWorker.RunWorkerAsync(obj);
            }
            else
                ClientHelper.ShowErrorMessage("No items to export.");
        }
    }

    public delegate void PurchasePaymentUpdatedEventHandler(object sender, EventArgs args);
}