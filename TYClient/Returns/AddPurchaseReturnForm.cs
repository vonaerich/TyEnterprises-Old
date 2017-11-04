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
    public partial class AddPurchaseReturnForm : InputDetailsForm
    {
        private readonly ICustomerController customerController;
        private readonly IPurchaseController purchaseController;
        private readonly IPurchaseReturnController purchaseReturnController;

        public int ReturnId { get; set; }

        #region Props
        private int QtyLimit = 0; //for storing the total qty of an invoice item
        #endregion

        #region Event

        public event PurchaseReturnUpdatedEventHandler PurchaseReturnUpdated;

        protected void OnPurchaseReturnUpdated(EventArgs e)
        {
            PurchaseReturnUpdatedEventHandler handler = PurchaseReturnUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        BackgroundWorker worker = new BackgroundWorker();
        BackgroundWorker exportWorker = new BackgroundWorker();
        public AddPurchaseReturnForm()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.purchaseController = IOC.Container.GetInstance<PurchaseController>();
            this.purchaseReturnController = IOC.Container.GetInstance<PurchaseReturnController>();

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
                PurchaseReturnExportObject exportObject = (PurchaseReturnExportObject)e.Argument;
                IExportStrategy strategy = new PurchaseReturnExportStrategy(exportObject);

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
            if (PurchaseReturnUpdated != null)
                PurchaseReturnUpdated(new object(), new EventArgs());

            ToggleButtons(true);
            LoadImage.Visible = false;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                PurchaseReturnColumnModel model = (PurchaseReturnColumnModel)e.Argument;

                if (model.Id == 0)
                    this.purchaseReturnController.InsertPurchaseReturn(model);
                else
                    this.purchaseReturnController.UpdatePurchaseReturn(model);
            }
        }

        #endregion

        #region Load

        private void AddPurchaseReturnForm_Load(object sender, EventArgs e)
        {
            LoadSupplierDropdown();
            PrepareForm();
        }

        private void PrepareForm()
        {
            if (ReturnId != 0)
            {
                LoadPurchaseReturnDetails();
                MemoTextbox.ReadOnly = true;
                SupplierDropdown.Enabled = false;
            }
            else
            {
                ClearAll();
                LoadMemoNumber();
                MemoTextbox.ReadOnly = false;
                SupplierDropdown.Enabled = true;
            }

            CheckIfEditAllowed();
        }

        private void LoadMemoNumber()
        {
            var generator = new CodeGenerator(new PurchaseReturnCodeGenerator());
            string memoNumber = generator.GenerateCode();

            MemoTextbox.Text = memoNumber;
        }

        private void CheckIfEditAllowed()
        {
            //CustomerDebit c = CustomerDebitController.Instance.FetchCustomerDebitByReference(MemoTextbox.Text);
            bool editAllowed = true;

            //if (c != null)
            //{
            //    if (c.Debited.Value)
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

        private void SupplierDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearList();
            LoadPONumbers();
        }

        private void ClearList()
        {
            list.Clear();
            purchaseReturnDetailModelBindingSource.DataSource = null;
        }

        public void LoadPurchaseReturnDetails()
        {
            PurchaseReturn sReturn = this.purchaseReturnController.FetchPurchaseReturnById(ReturnId);

            if (sReturn != null)
            {
                MemoTextbox.Text = sReturn.MemoNumber;
                ReturnDatePicker.Value = sReturn.ReturnDate.HasValue ? sReturn.ReturnDate.Value : DateTime.Today;
                SupplierDropdown.SelectedValue = sReturn.CustomerId;
                RemarksTextbox.Text = sReturn.Remarks;

                list = this.purchaseReturnController.FetchPurchaseReturnDetails(ReturnId).ToList();
                if (list.Any())
                {
                    BindDetails();
                    ComputeTotalDebit();
                }
            }
            else
                PrepareForm();
        }

        private void LoadPONumbers()
        {
            if (SupplierDropdown.SelectedIndex != -1)
            {
                int supplierId = (int)SupplierDropdown.SelectedValue;
                purchaseDisplayModelBindingSource.DataSource = this.purchaseController.FetchAllPONumbersByCustomer(supplierId);
                PODropdown.SelectedIndex = -1;
            }
            else
                purchaseDisplayModelBindingSource.DataSource = null;
        }

        private void LoadSupplierDropdown()
        {
            customerDisplayModelBindingSource.DataSource = this.customerController.FetchAllCustomers();
            SupplierDropdown.SelectedIndex = -1;
        }

        List<PurchaseDetailViewModel> purchaseDetails = null;
        private void PODropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            purchaseDetails = null;

            if (PODropdown.SelectedIndex != -1)
            {
                string poNumber = PODropdown.SelectedItem.ToString();
                purchaseDetails = this.purchaseController.FetchPurchaseDetailsPerPOAndSupplier(poNumber, (int)SupplierDropdown.SelectedValue);
            }

            purchaseDetailViewModelBindingSource.DataSource = purchaseDetails;
            PartNumberDropdown.SelectedIndex = -1;
            ClearItem();
        }

        private decimal _oldUnitPrice;
        private void PartNumberDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PartNumberDropdown.SelectedIndex != -1 && purchaseDetails != null)
            {
                _oldUnitPrice = 0m;
                int id = (int)PartNumberDropdown.SelectedValue;

                PurchaseDetailViewModel details = purchaseDetails.FirstOrDefault(a => a.AutoPartDetailId == id);
                if (details != null)
                {
                    //Get Previously returned value of this invoice number
                    int supplierId = (int)SupplierDropdown.SelectedValue;
                    string poNumber = PODropdown.SelectedItem.ToString();
                    int qtyReturned = this.purchaseReturnController.FetchQuantityReturned(supplierId, poNumber, id, ReturnId);

                    //Get quantities in grid
                    int qtyInGrid = 0;
                    if (list.Any())
                    {
                        qtyInGrid = list.Where(a => a.PartDetailId == id && a.PONumber == poNumber).Sum(a => a.Quantity);
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

        List<PurchaseReturnDetailModel> list = new List<PurchaseReturnDetailModel>();
        private void AddButton_Click(object sender, EventArgs e)
        {
            if (PODropdown.SelectedIndex == -1 || PartNumberDropdown.SelectedIndex == -1 || string.IsNullOrWhiteSpace(UnitPriceTextbox.Text))
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
                AddPurchaseReturn();
            }
        }

        void f_ApprovalDone(object sender, ApprovalEventArgs e)
        {
            if (e.ApproverId == 0)
                ClientHelper.ShowErrorMessage("Invalid approver.");
            else
                AddPurchaseReturn();
        }

        private void AddPurchaseReturn()
        {
            ComputeTotalDetailAmount();

            PurchaseReturnDetailModel model = new PurchaseReturnDetailModel()
            {
                AutoPart = AutoPartTextbox.Text,
                PartDetailId = (int)PartNumberDropdown.SelectedValue,
                Id = 0,
                PONumber = PODropdown.SelectedItem.ToString(),
                PartNumber = PartNumberDropdown.Text,
                Quantity = int.Parse(QtyTextbox.Text),
                UnitPrice = UnitPriceTextbox.Text.ToDecimal(),
                TotalAmount = TotalAmountTextbox.Text.ToDecimal()
            };

            list.Insert(0, model);
            BindDetails();

            ClearDetails();
            ComputeTotalDebit();
            PODropdown.Focus();
        }

        private void BindDetails()
        {
            purchaseReturnDetailModelBindingSource.DataSource = null;
            purchaseReturnDetailModelBindingSource.DataSource = list;
        }

        #endregion

        #region Clear

        private void ClearDetailButton_Click(object sender, EventArgs e)
        {
            ClearDetails();
        }

        private void ClearDetails()
        {
            PODropdown.SelectedIndex = -1;
            PartNumberDropdown.SelectedIndex = -1;

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

        private void ComputeTotalDebit()
        {
            decimal total = list.Sum(a => a.TotalAmount);
            TotalTextbox.Text = total.ToString("N2");
        }

        #endregion

        #region Delete

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            ComputeTotalDebit();
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
                //Approval
                bool needsApproval = false;
                SystemSettings.RefreshApproval();
                if (ReturnId != 0)
                    needsApproval = SystemSettings.Approval["PurchaseReturnEdit"];
                else
                    needsApproval = SystemSettings.Approval["PurchaseReturnAdd"];

                if (!needsApproval)
                    SaveReturn();
                else
                {
                    ApprovalForm form = new ApprovalForm();
                    form.ApprovalDone += new ApprovalDoneEventHandler(form_ApprovalDone);
                    form.Show();
                }
            }
        }

        void form_ApprovalDone(object sender, ApprovalEventArgs e)
        {
            if (e.ApproverId == 0)
                ClientHelper.ShowErrorMessage("Invalid approver.");
            else
            {
                approver = e.ApproverId;
                SaveReturn();
            }
        }

        private void SaveReturn()
        {
            LoadImage.Visible = true;
            ToggleButtons(false);

            PurchaseReturnColumnModel model = new PurchaseReturnColumnModel()
            {
                Id = ReturnId,
                MemoNumber = MemoTextbox.Text,
                CustomerId = (int)SupplierDropdown.SelectedValue,
                IsDeleted = false,
                ReturnDate = ReturnDatePicker.Value,
                TotalDebitAmount = list.Sum(a => a.TotalAmount),
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
                SupplierDropdown.SelectedIndex = -1;
                SupplierDropdown.ComboBox.SelectedIndex = -1;
                RemarksTextbox.Clear();

                

                TotalTextbox.Text = "0.00";
                MemoTextbox.Focus();
            }
            else
                LoadPurchaseReturnDetails();

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
                hasDuplicate = DuplicateChecker.CodeHasDuplicate(CodeType.PurchaseReturn, MemoTextbox.Text.Trim(), ReturnId);

            if (hasDuplicate)
                MemoTextbox.StateCommon.Content.Color1 = Color.Red;
            else
                MemoTextbox.StateCommon.Content.Color1 = Color.Black;
        }

        #endregion

        #endregion

        private PurchaseReturnExportObject CreateExportObject()
        {
            PurchaseReturnExportObject obj = new PurchaseReturnExportObject()
            {
                Code = MemoTextbox.Text,
                Supplier = SupplierDropdown.Text,
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

                PurchaseReturnExportObject exportObject = CreateExportObject();
                exportWorker.RunWorkerAsync(exportObject);
            }
            else
                ClientHelper.ShowErrorMessage("No items to export.");
        }
    }

    public delegate void PurchaseReturnUpdatedEventHandler(object sender, EventArgs e);
}