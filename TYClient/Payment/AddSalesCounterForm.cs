using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TY.SPIMS.Client.Approval;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Client.Helper.CodeGenerator;
using TY.SPIMS.Client.Helper.Export;
using TY.SPIMS.Client.ViewManagers;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Payment
{
    public partial class AddSalesCounterForm : InputDetailsForm
    {
        private readonly ICustomerController customerController;
        private readonly ISalesCounterController salesCounterController;

        public int SalesCounterId { get; set; }

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
        public AddSalesCounterForm()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.salesCounterController = IOC.Container.GetInstance<SalesCounterController>();

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
                SalesCounterExportObject exportObject = (SalesCounterExportObject)e.Argument;
                IExportStrategy strategy = new SalesCounterExportStrategy(exportObject);

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
            if (SalesCounterId == 0)
                ClientHelper.ShowSuccessMessage("Sales counter successfully added.");
            else
                ClientHelper.ShowSuccessMessage("Sales counter successfully updated.");

            ClearAll();

            if (SalesPaymentUpdated != null)
                SalesPaymentUpdated(sender, new EventArgs());

            ToggleButtons(true);
            LoadImage.Visible = false;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                CounterSale args = (CounterSale)e.Argument;

                if (args != null)
                {
                    if (args.Id == 0)
                        this.salesCounterController.Insert(args);
                    else
                        this.salesCounterController.Update(args);
                }
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
            if (SalesCounterId != 0)
            {
                LoadCounterDetails();
                CustomerDropdown.Enabled = false;
            }
            else
            {
                //GenerateCode();
                CustomerDropdown.Enabled = true;
                CustomerDropdown.Focus();
            }
        }

        private void GenerateCode()
        {
            var generator = new CodeGenerator(new SalesCounterCodeGenerator());
            CounterNumberTextbox.Text = generator.GenerateCode();
        }

        public void LoadCounterDetails()
        {
            var detail = this.salesCounterController.FetchCounterById(SalesCounterId);
            if (detail != null)
            {
                CounterNumberTextbox.Text = detail.CounterNumber;
                CustomerDropdown.SelectedValue = detail.CustomerId;
                RemarksTextbox.Text = detail.Remarks;
                PaymentDatePicker.Value = detail.Date.GetValue();
                DiscountTextbox.Value = detail.Discount.GetValue();
                TaxCheckbox.Checked = detail.WitholdingTax.HasValue && detail.WitholdingTax.Value != 0;

                invoiceList = this.salesCounterController.FetchSalesItems(SalesCounterId);
                BindInvoices();

                ComputeTotalAmountDue();
            }
        }

        private void LoadCustomers()
        {
            customerDisplayModelBindingSource.DataSource = this.customerController.FetchAllCustomers();
            CustomerDropdown.SelectedIndex = -1;
            CustomerDropdown.Focus();
        }

        #endregion

        #region Invoices

        private void CustomerDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            invoiceList.Clear();
            BindInvoices();
            ComputeTotalAmountDue();

            if (CustomerDropdown.SelectedIndex != -1)
            {
                int id = (int)CustomerDropdown.SelectedValue;
                AddButton.Enabled = true;
            }
            else
            {
                AddButton.Enabled = false;
            }
        }

        SortableBindingList<SalesCounterItemModel> invoiceList = new SortableBindingList<SalesCounterItemModel>();
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
                totalDue = invoiceList.Where(a => a.SaleId != null).Sum(a => a.Amount).GetValue();

                decimal credit = invoiceList.Where(a => a.ReturnId != null).Sum(a => a.Amount).GetValue(); //CreditTextbox.Text.ToDecimal();
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
        }

        #endregion

        #region Save

        int? approver = null;
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
                ClientHelper.ShowRequiredMessage("Counter Number");
            else if (invoiceList.Count == 0)
                ClientHelper.ShowErrorMessage("No counter items.");
            else if (hasDuplicate)
                ClientHelper.ShowDuplicateMessage("Counter Number");
            else if (DiscountTextbox.Value != 0 && !UserInfo.IsAdmin)
            {
                ApprovalForm f = new ApprovalForm();
                f.ApprovalDone += new ApprovalDoneEventHandler(form_ApprovalDone);
                f.Show();
            }
            else
                SaveCounter();
        }

        void form_ApprovalDone(object sender, ApprovalEventArgs e)
        {
            if (e.ApproverId == 0)
                ClientHelper.ShowErrorMessage("Invalid approver.");
            else
            {
                approver = e.ApproverId;
                SaveCounter();
            }
        }

        private void SaveCounter()
        {
            CounterSale newCounter = new CounterSale() 
            {
                Id = SalesCounterId,
                CounterNumber = CounterNumberTextbox.Text.Trim(),
                Date = PaymentDatePicker.Value,
                Remarks = RemarksTextbox.Text.Trim(),
                Total = TotalAmountDueTextbox.Text.ToDecimal(),
                WitholdingTax = WitholdingTaxTextbox.Text.ToDecimal(),
                Discount = DiscountTextbox.Value,
                CustomerId = (int)CustomerDropdown.SelectedValue
            };

            foreach (var item in invoiceList)
                newCounter.CounterSalesItems.Add(new CounterSalesItem() { 
                    Amount = item.Amount,
                    SaleId = item.SaleId,
                    SalesReturnDetailId = item.ReturnId,
                });

            ToggleButtons(false);
            LoadImage.Visible = true;

            if (!worker.IsBusy) {
                worker.RunWorkerAsync(newCounter);
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
            if (SalesCounterId == 0)
            {
                CustomerDropdown.ComboBox.SelectedIndex = -1;
                CustomerDropdown.SelectedIndex = -1;
                CounterNumberTextbox.Clear();

                PaymentDatePicker.Value = DateTime.Today;
                TaxCheckbox.Checked = false;
                DiscountTextbox.Value = 0;
                CreditTextbox.Text = "0.00";
                AmountDueTextbox.Text = "0.00";
                TotalAmountDueTextbox.Text = "0.00";
                RemarksTextbox.Clear();

                invoiceList.Clear();
                BindInvoices();
            }

            SetupForm();
        }

        #endregion

        #region Validation

        private void VoucherNumberTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CounterNumberTextbox.Text))
                e.Cancel = true;
        }

        #region Check Duplicate

        private bool hasDuplicate = false;
        private void VoucherNumberTextbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CounterNumberTextbox.Text))
                hasDuplicate = DuplicateChecker.CodeHasDuplicate(CodeType.SalesPayment, CounterNumberTextbox.Text.Trim(), SalesCounterId);

            if (hasDuplicate)
                CounterNumberTextbox.StateCommon.Content.Color1 = Color.Red;
            else
                CounterNumberTextbox.StateCommon.Content.Color1 = Color.Black;
        }

        #endregion

        private void DiscountTextbox_ValueChanged(object sender, EventArgs e)
        {
            ComputeTotalAmountDue();
        }

        #endregion

        private void AddButton_Click(object sender, EventArgs e) 
        {
            if (CustomerDropdown.SelectedIndex == -1) 
                return;

            //var form = new AddInvoiceForm() 
            //{
            //    CustomerId = (int)CustomerDropdown.SelectedValue,
            //    CustomerName = CustomerDropdown.Text,
            //};

            var form = new AddCounterItemForm()
                {
                    CustomerId = (int)CustomerDropdown.SelectedValue,
                    CustomerName = CustomerDropdown.Text,
                    InvoiceIds = invoiceList.Where(a => a.SaleId != null && a.SaleId != 0).ToDictionary(a => a.SaleId, b => b.Amount),
                    ReturnIds = invoiceList.Where(a => a.ReturnId != null && a.ReturnId != 0)
                        .ToDictionary(a => a.ReturnId, b => b.Amount),
                    Type = PaymentType.Sales
                };

            form.SelectionComplete += new EventHandler<SelectionCompleteEventArgs>(form_SelectionComplete);
            form.Show();
        }

        void form_SelectionComplete(object sender, SelectionCompleteEventArgs args)
        {
            if (args.SelectedSalesItems == null) 
                return;

            this.invoiceList = args.SelectedSalesItems;
            BindInvoices();
            ComputeTotalAmountDue();
        }

        private SalesCounterExportObject CreateExportObject()
        {
            var obj = new SalesCounterExportObject() 
            {
                Code = CounterNumberTextbox.Text,
                Customer = CustomerDropdown.Text,
                Remarks = RemarksTextbox.Text,
                Items = invoiceList.ToList(),
                WitholdingTax = WitholdingTaxTextbox.Text.ToDecimal(),
                Discount = DiscountTextbox.Value,
                Date = PaymentDatePicker.Value.ToShortDateString()
            };

            return obj;
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (invoiceList.Count > 0)
            {
                ToggleButtons(false);
                LoadImage.Visible = true;

                var obj = CreateExportObject();
                exportWorker.RunWorkerAsync(obj);
            }
            else
                ClientHelper.ShowErrorMessage("No items to export.");
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }
    }
}