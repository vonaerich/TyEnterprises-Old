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
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Returns
{
    public partial class AddSalesReturnForm : InputDetailsForm
    {
        private readonly ISaleController saleController;
        private readonly ISalesReturnController salesReturnController;
        private readonly ICustomerController customerController;

        public int ReturnId { get; set; }

        #region Props
        private int QtyLimit = 0; //for storing the total qty of an invoice item
        #endregion

        #region Event

        public event SalesReturnUpdatedEventHandler SalesReturnUpdated;

        protected void OnSalesReturnUpdated(EventArgs e)
        {
            SalesReturnUpdatedEventHandler handler = SalesReturnUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        BackgroundWorker worker = new BackgroundWorker();
        BackgroundWorker exportWorker = new BackgroundWorker();
        public AddSalesReturnForm()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.saleController = IOC.Container.GetInstance<SaleController>();
            this.salesReturnController = IOC.Container.GetInstance<SalesReturnController>();

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
                SalesReturnExportObject exportObject = (SalesReturnExportObject)e.Argument;
                IExportStrategy strategy = new SalesReturnExportStrategy(exportObject);

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
            if (ReturnId == 0)
                ClientHelper.ShowSuccessMessage(string.Format("Memo number {0} successfully added.", MemoTextbox.Text));
            else
                ClientHelper.ShowSuccessMessage(string.Format("Memo number {0} successfully updated.", MemoTextbox.Text));

            PrepareForm();
            if (SalesReturnUpdated != null)
                SalesReturnUpdated(new object(), new EventArgs());

            ToggleButtons(true);
            LoadImage.Visible = false;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                SalesReturnColumnModel model = (SalesReturnColumnModel)e.Argument;

                if (model.Id == 0)
                    this.salesReturnController.InsertSalesReturn(model);
                else
                    this.salesReturnController.UpdateSalesReturn(model);
            }
        }

        #endregion

        #region Load

        private void AddSalesReturnForm_Load(object sender, EventArgs e)
        {
            LoadCustomerDropdown();
            PrepareForm();
        }

        private void PrepareForm()
        {
            if (ReturnId != 0)
            {
                LoadSalesReturnDetails();
                MemoTextbox.ReadOnly = true;
                CustomerDropdown.Enabled = false;
            }
            else
            {
                ClearAll();
                LoadMemoNumber();
                MemoTextbox.ReadOnly = false;
                CustomerDropdown.Enabled = true;
            }

            CheckIfEditAllowed();
        }

        private void LoadMemoNumber()
        {
            var generator = new CodeGenerator(new SalesReturnCodeGenerator());
            string memoNumber = generator.GenerateCode();

            MemoTextbox.Text = memoNumber;
        }

        private void CheckIfEditAllowed()
        {
            //CustomerCredit c = CustomerCreditController.Instance.FetchCustomerCreditByReference(MemoTextbox.Text);
            bool editAllowed = true;
 
            //if (c != null)
            //{
            //    if (c.Credited.Value)
            //        editAllowed = false;
            //}

            if (!editAllowed)
            {
                SaveButton.Visible = false;
                ClearButton.Visible = false;

                AddDetailsPanel.Visible = false;
                ErrorPanel.Visible = true;
            }
            else
            {
                SaveButton.Visible = true;
                ClearButton.Visible = true;

                AddDetailsPanel.Visible = true;
                ErrorPanel.Visible = false;
            }
        }

        private void CustomerDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearList();
            LoadInvoiceNumbers();
        }

        private void ClearList()
        {
            list.Clear();
            salesReturnDetailModelBindingSource.DataSource = null;
        }

        public void LoadSalesReturnDetails()
        {
            SalesReturn sReturn = this.salesReturnController.FetchSalesReturnById(ReturnId);

            if (sReturn != null)
            {
                MemoTextbox.Text = sReturn.MemoNumber;
                ReturnDatePicker.Value = sReturn.ReturnDate.HasValue ? sReturn.ReturnDate.Value : DateTime.Today;
                CustomerDropdown.SelectedValue = sReturn.CustomerId;
                RemarksTextbox.Text = sReturn.Remarks;

                list = this.salesReturnController.FetchSalesReturnDetails(ReturnId).ToList();
                if (list.Any())
                {
                    BindDetails();
                    ComputeTotalCredit();
                }
            }
            else
                PrepareForm();
        }

        private void LoadInvoiceNumbers()
        {
            if (CustomerDropdown.SelectedIndex != -1)
            {
                int customerId = (int)CustomerDropdown.SelectedValue;
                saleDisplayModelBindingSource.DataSource = this.saleController.FetchAllInvoiceNumbersByCustomer(customerId);
                InvoiceDropdown.SelectedIndex = -1;
            }
            else
                saleDisplayModelBindingSource.DataSource = null;
        }

        private void LoadCustomerDropdown()
        {
            customerDisplayModelBindingSource.DataSource = this.customerController.FetchAllCustomers();
            CustomerDropdown.SelectedIndex = -1;
        }

        List<SalesDetailViewModel> saleDetails = null;
        private void InvoiceDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            saleDetails = null;

            if (InvoiceDropdown.SelectedIndex != -1)
            {
                string invNumber = InvoiceDropdown.SelectedItem.ToString();
                saleDetails = this.saleController.FetchSaleDetailsPerInvoice(invNumber);
            }

            salesDetailViewModelBindingSource.DataSource = saleDetails;
            PartNumberDropdown.SelectedIndex = -1;
            ClearItem();
        }

        private decimal _oldUnitPrice;
        private void PartNumberDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PartNumberDropdown.SelectedIndex != -1 && saleDetails != null)
            {
                _oldUnitPrice = 0m;
                int id = (int)PartNumberDropdown.SelectedValue;

                SalesDetailViewModel details = saleDetails.FirstOrDefault(a => a.AutoPartDetailId == id);
                if (details != null)
                {
                    //Get Previously returned value of this invoice number
                    int customerId = (int)CustomerDropdown.SelectedValue;
                    string invNumber = InvoiceDropdown.SelectedItem.ToString();
                    int qtyReturned = this.salesReturnController.FetchQuantityReturned(customerId, invNumber, id, ReturnId);

                    //Get quantities in grid
                    int qtyInGrid = 0;
                    if (list.Any())
                    {
                        qtyInGrid = list.Where(a => a.PartDetailId == id && a.InvoiceNumber == invNumber).Sum(a => a.Quantity);
                    }

                    AutoPartTextbox.Text = details.AutoPartName;

                    //Qty that can be returned
                    int qtyLimit = details.Quantity - qtyReturned - qtyInGrid;
                    QtyTextbox.Text = qtyLimit.ToString();
                    QtyLimit = qtyLimit;

                    _oldUnitPrice = details.DiscountedPrice;
                    UnitPriceTextbox.Text = _oldUnitPrice.ToString("N2");
                }

                QtyTextbox.Focus();
            }
        }

        #endregion

        #region Details

        #region Add

        List<SalesReturnDetailModel> list = new List<SalesReturnDetailModel>();
        private void AddButton_Click(object sender, EventArgs e)
        {
            if (InvoiceDropdown.SelectedIndex == -1 || PartNumberDropdown.SelectedIndex == -1 || string.IsNullOrWhiteSpace(UnitPriceTextbox.Text))
                return;
            else if (QtyTextbox.Text == "0")
            {
                ClientHelper.ShowErrorMessage("No quantity to return.");
                return;
            }
            else if (int.Parse(QtyTextbox.Text) > QtyLimit)
            {
                QtyTextbox.Text = QtyLimit.ToString();
                ClientHelper.ShowErrorMessage("Return quantity is greater than original quantity.");
                QtyTextbox.Focus();

                return;
            }
            else if (!UserInfo.IsAdmin && !string.IsNullOrWhiteSpace(UnitPriceTextbox.Text) &&
                _oldUnitPrice != UnitPriceTextbox.Text.ToDecimal())
            {
                ApprovalForm f = new ApprovalForm();
                f.ApprovalDone += new ApprovalDoneEventHandler(f_ApprovalDone);
                f.Show();
            }
            else
            {
                AddSalesReturn();
            }
        }

        private void AddSalesReturn()
        {
            ComputeTotalDetailAmount();

            SalesReturnDetailModel model = new SalesReturnDetailModel()
            {
                AutoPart = AutoPartTextbox.Text,
                PartDetailId = (int)PartNumberDropdown.SelectedValue,
                Id = 0,
                InvoiceNumber = InvoiceDropdown.SelectedItem.ToString(),
                PartNumber = PartNumberDropdown.Text,
                Quantity = int.Parse(QtyTextbox.Text),
                UnitPrice = UnitPriceTextbox.Text.ToDecimal(),
                TotalAmount = TotalAmountTextbox.Text.ToDecimal()
            };

            list.Insert(0, model);
            BindDetails();

            ClearDetails();
            ComputeTotalCredit();
            InvoiceDropdown.Focus();
        }

        void f_ApprovalDone(object sender, ApprovalEventArgs e)
        {
            if (e.ApproverId == 0)
                ClientHelper.ShowErrorMessage("Invalid approver.");
            else
                AddSalesReturn();
        }

        private void BindDetails()
        {
            salesReturnDetailModelBindingSource.DataSource = null;
            salesReturnDetailModelBindingSource.DataSource = list;
        }

        #endregion

        #region Clear

        private void ClearDetailButton_Click(object sender, EventArgs e)
        {
            ClearDetails();
        }

        private void ClearDetails()
        {
            InvoiceDropdown.SelectedIndex = -1;
            InvoiceDropdown.ComboBox.SelectedIndex = -1;
            PartNumberDropdown.SelectedIndex = -1;
            PartNumberDropdown.ComboBox.SelectedIndex = -1;

            ClearItem();
        }

        //Clears inputable item in details
        private void ClearItem()
        {
            AutoPartTextbox.Clear();
            QtyTextbox.Text = "0";
            QtyLimit = 0;
            UnitPriceTextbox.Text = "0.00";
            TotalAmountTextbox.Text = "0.00";
        }

        #endregion

        #region Compute

        private void QtyTextbox_Leave(object sender, EventArgs e)
        {
            ComputeTotalDetailAmount();
        }

        private void UnitPriceTextbox_Leave(object sender, EventArgs e)
        {
            ComputeTotalDetailAmount();
        }

        private void ComputeTotalDetailAmount()
        {
            int qty = int.Parse(QtyTextbox.Text);
            decimal price = decimal.Parse(UnitPriceTextbox.Text);

            decimal total = qty * price;
            TotalAmountTextbox.Text = total.ToString("N2");
        }

        private void ComputeTotalCredit()
        {
            decimal total = list.Sum(a => a.TotalAmount);
            TotalTextbox.Text = total.ToString("N2");
        }

        #endregion

        #region Delete

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            ComputeTotalCredit();
            ClearDetails();
        }

        #endregion

        #endregion

        #region Save
        int? approver = null;
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
                ClientHelper.ShowRequiredMessage("Memo number, Return Items");
            else if (hasDuplicate)
                ClientHelper.ShowDuplicateMessage("Memo number");
            else
            {
                SaveReturn();
            }
        }
 
        private void SaveReturn()
        {
            ToggleButtons(false);
            LoadImage.Visible = true;

            SalesReturnColumnModel model = new SalesReturnColumnModel()
            {
                Id = ReturnId,
                MemoNumber = MemoTextbox.Text,
                CustomerId = (int)CustomerDropdown.SelectedValue,
                IsDeleted = false,
                ReturnDate = ReturnDatePicker.Value,
                TotalCreditAmount = list.Sum(a => a.TotalAmount),
                Remarks = RemarksTextbox.Text,
                Details = list,
                RecordedByUser = UserInfo.UserId,
                ApprovedByUser = approver
            };

            worker.RunWorkerAsync(model);
        }

        private void ToggleButtons(bool enabled)
        {
            ExportButton.Enabled = enabled;
            SaveButton.Enabled = enabled;
            ClearButton.Enabled = enabled;
        }

        #endregion

        #region Clear

        private void ClearAll()
        {
            if (ReturnId == 0)
            {
                //MemoTextbox.Clear();
                ReturnDatePicker.Value = DateTime.Now;
                CustomerDropdown.SelectedIndex = -1;
                CustomerDropdown.ComboBox.SelectedIndex = -1;
                RemarksTextbox.Clear();

                ClearList();

                TotalTextbox.Text = "0.00";
                MemoTextbox.Focus();
            }
            else
                LoadSalesReturnDetails();

            ClearDetails();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        #endregion

        #region Validation

        private void MemoTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MemoTextbox.Text))
                e.Cancel = true;
        }

        private void TotalTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (TotalTextbox.Text == "0.00")
                e.Cancel = true;
        }

        #region Check Duplicate

        private bool hasDuplicate = false;
        private void MemoTextbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MemoTextbox.Text))
                hasDuplicate = DuplicateChecker.CodeHasDuplicate(CodeType.SalesReturn, MemoTextbox.Text.Trim(), ReturnId);

            if (hasDuplicate)
                MemoTextbox.StateCommon.Content.Color1 = Color.Red;
            else
                MemoTextbox.StateCommon.Content.Color1 = Color.Black;
        }

        #endregion

        #endregion

        private SalesReturnExportObject CreateExportObject()
        {
            SalesReturnExportObject obj = new SalesReturnExportObject()
            {
                Code = MemoTextbox.Text,
                Customer = CustomerDropdown.Text,
                Items = list
            };

            return obj;
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (list.Count > 0)
            {
                ToggleButtons(false);
                LoadImage.Visible = true;

                SalesReturnExportObject exportObject = CreateExportObject();
                exportWorker.RunWorkerAsync(exportObject);

                
            }
            else
                ClientHelper.ShowErrorMessage("No items to export.");
        }
    }

    public delegate void SalesReturnUpdatedEventHandler(object sender, EventArgs e);
}