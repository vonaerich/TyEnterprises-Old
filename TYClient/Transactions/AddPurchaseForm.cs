using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TY.SPIMS.Client.Controls;
using TY.SPIMS.Client.DetailModel;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Transactions
{
    public partial class AddPurchaseForm : InputDetailsForm
    {
        private readonly ICustomerController customerController;
        private readonly IPurchaseController purchaseController;

        public int PurchaseId { get; set; }

        #region Const

        private const string ZeroCurrency = "0.00";
        private const string Zero = "0";

        #endregion

        #region Event

        public event PurchaseUpdatedEventHandler PurchaseUpdated;

        protected void OnPurchaseUpdated(EventArgs e)
        {
            PurchaseUpdatedEventHandler handler = PurchaseUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        private BackgroundWorker purchaseWorker = new BackgroundWorker();

        public AddPurchaseForm()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.purchaseController = IOC.Container.GetInstance<PurchaseController>();

            InitializeComponent();

            purchaseWorker.DoWork += new DoWorkEventHandler(purchaseWorker_DoWork);
            purchaseWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(purchaseWorker_RunWorkerCompleted);

            this.AutoValidate = AutoValidate.Disable;
        }

        #region Worker

        void purchaseWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (PurchaseId == 0)
                ClientHelper.ShowSuccessMessage(string.Format("PO number {0} successfully added.", PONumberTextbox.Text));
            else
                ClientHelper.ShowSuccessMessage(string.Format("PO number {0} successfully updated.", PONumberTextbox.Text));

            ClearAll();

            SaveButton.Enabled = true;
            ClearButton.Enabled = true;

            LoadImage.Visible = false;
            //PONumberTextbox.Focus();
            SupplierDropdown.Focus();

            if (PurchaseUpdated != null)
                PurchaseUpdated(new object(), new EventArgs());
        }

        void purchaseWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            PurchaseColumnModel model = (PurchaseColumnModel)e.Argument;

            if (model.Id == 0)
                this.purchaseController.InsertPurchase(model);
            else
                this.purchaseController.UpdatePurchase(model);
        }

        #endregion

        #region Load

        private void AddPurchaseForm_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            addItemControl1.Type = TransactionType.Purchase;
            addItemControl1.LoadControl();

            if (PurchaseId != 0)
                LoadPurchaseDetails();
        }

        #region For Edit

        private void CheckIfEditAllowed()
        {
            if (this.purchaseController.ItemHasPaymentOrReturn(PurchaseId))
            {
                SaveButton.Visible = false;
                ClearButton.Visible = false;

                addItemControl1.IsDetailLeftHidden = true;
                ErrorPanel.Visible = true;
            }
            else
            {
                SaveButton.Visible = true;
                ClearButton.Visible = true;

                addItemControl1.IsDetailLeftHidden = false;
                ErrorPanel.Visible = false;
            }
        }

        public void LoadPurchaseDetails()
        {
            if (PurchaseId != 0)
            {
                Purchase p = this.purchaseController.FetchPurchaseById(PurchaseId);

                PONumberTextbox.Text = p.PONumber;
                SupplierDropdown.SelectedValue = p.CustomerId;
                PurchaseDate.Value = p.Date.HasValue ? p.Date.Value : DateTime.Now;

                TypeDropdown.SelectedIndex = p.Type.HasValue ? p.Type.Value : 0;

                if (p.PurchaseRequisition.Any())
                {
                    StringBuilder b = new StringBuilder();
                    foreach (var pr in p.PurchaseRequisition.OrderBy(a => a.PRNumber))
                    {
                        b.AppendLine(pr.PRNumber);
                    }
                    PRTextbox.Text = b.ToString();
                }

                if (p.PurchaseOrder.Any())
                {
                    StringBuilder b = new StringBuilder();
                    foreach (var po in p.PurchaseOrder.OrderBy(a => a.PONumber))
                    {
                        b.AppendLine(po.PONumber);
                    }
                    POTextbox.Text = b.ToString();
                }

                if (p.PurchaseOption == "Pick-Up")
                    PickupRB.Checked = true;
                else if(p.PurchaseOption == "Delivery")
                    DeliveryRB.Checked = true;

                CommentTextbox.Text = p.Comment;
                PONumberTextbox.ReadOnly = true;

                var details = this.purchaseController.FetchPurchaseDetails(PurchaseId).ToList();
                addItemControl1.ItemList = MapModelToItemList(details);
                addItemControl1.InvoiceDiscountPercent = p.InvoiceDiscountPercent.HasValue ? p.InvoiceDiscountPercent.Value : 0;
                addItemControl1.InvoiceDiscount = p.InvoiceDiscount.HasValue ? p.InvoiceDiscount.Value : 0;
                addItemControl1.NotifyAdmin = false;

                //CheckIfEditAllowed();
            }
            else
                ClearAll();
        }

        #endregion

        private void LoadSuppliers()
        {
            customerDisplayModelBindingSource.DataSource = this.customerController.FetchAllCustomers();
            SupplierDropdown.SelectedIndex = -1;
        }

        private void SupplierDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            string addressValue = "Address : N/A";
            string contactValue = "Contact Person : N/A";

            if (SupplierDropdown.SelectedIndex != -1)
            {
                int selectedId = (int)SupplierDropdown.SelectedValue;

                Customer c = this.customerController.FetchCustomerById(selectedId);
                addressValue = string.Format("Address : {0}", c.Address);
                contactValue = string.Format("Contact Person : {0}", c.ContactPerson);
                addItemControl1.CustomerName = c.CompanyName;
            }

            CheckDuplicate();
        }

        #endregion

        #region Save
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren() || addItemControl1.ItemList.Count == 0)
                ClientHelper.ShowRequiredMessage("Invoice Number, Supplier, Purchase Items");
            else if (hasDuplicate)
                ClientHelper.ShowDuplicateMessage("Invoice Number");
            else
                SavePurchase();
        }

        private void SavePurchase()
        {
            LoadImage.Visible = true;

            SaveButton.Enabled = false;
            ClearButton.Enabled = false;

            List<PurchaseDetailViewModel> details = new List<PurchaseDetailViewModel>();
            MapItemListToModel(details);

            PurchaseColumnModel model = new PurchaseColumnModel()
            {
                Id = PurchaseId,
                CustomerId = (int)SupplierDropdown.SelectedValue,
                PurchaseDate = PurchaseDate.Value,
                PONumber = PONumberTextbox.Text,
                IsDeleted = false,
                IsPaid = false,
                RecordedBy = UserInfo.UserId,
                InvoiceDiscount = addItemControl1.InvoiceDiscount,
                InvoiceDiscountPercent = addItemControl1.InvoiceDiscountPercent,
                VatableSale = addItemControl1.VatableSales,
                Vat = addItemControl1.Vat,
                TotalAmount = addItemControl1.TotalAmount,
                Type = TypeDropdown.SelectedIndex != -1 ? TypeDropdown.SelectedIndex : 0,
                Option = PickupRB.Checked ? "Pick-Up" : "Delivery",
                Comment = CommentTextbox.Text,
                PR = PRTextbox.Text.Trim(),
                PO = POTextbox.Text.Trim(),
                Details = details
            };

            if (addItemControl1.NotifyAdmin)
            {
                string message = string.Format("Invoice number {0} contains items bought at a higher price.",
                    PONumberTextbox.Text.Trim());
                TY.SPIMS.Utilities.Helper.AddNotification(message);
            }

            purchaseWorker.RunWorkerAsync(model);
        }

        private void MapItemListToModel(List<PurchaseDetailViewModel> details)
        {
            addItemControl1.ItemList.ForEach((a) =>
            {
                details.Add(new PurchaseDetailViewModel()
                {
                    AutoPartDetailId = a.AutoPartDetailId,
                    AutoPartName = a.Name,
                    AutoPartNumber = a.PartNumber,
                    Quantity = a.Quantity,
                    DiscountedPrice = a.DiscountedPrice,
                    DiscountedPrice2 = a.DiscountedPrice2,
                    DiscountedPrice3 = a.DiscountedPrice3,
                    DiscountPercent = a.DiscountPercent,
                    DiscountPercent2 = a.DiscountPercent2,
                    DiscountPercent3 = a.DiscountPercent3,
                    DiscountPercents = a.DiscountPercents,
                    TotalAmount = a.TotalAmount,
                    TotalDiscount = a.TotalDiscount,
                    Unit = a.Unit,
                    UnitPrice = a.UnitPrice
                });
            });
        }

        private List<IAutoPartDetailModel> MapModelToItemList(List<PurchaseDetailViewModel> model)
        {
            List<IAutoPartDetailModel> list = new List<IAutoPartDetailModel>();
            model.ForEach((a) =>
            {
                list.Add(new PurchaseDetailModel()
                {
                    AutoPartDetailId = a.AutoPartDetailId,
                    Name = a.AutoPartName,
                    PartNumber = a.AutoPartNumber,
                    Quantity = a.Quantity,
                    DiscountedPrice = a.DiscountedPrice,
                    DiscountedPrice2 = a.DiscountedPrice2,
                    DiscountedPrice3 = a.DiscountedPrice3,
                    DiscountPercent = a.DiscountPercent,
                    DiscountPercent2 = a.DiscountPercent2,
                    DiscountPercent3 = a.DiscountPercent3,
                    DiscountPercents = a.DiscountPercents,
                    TotalAmount = a.TotalAmount,
                    TotalDiscount = a.TotalDiscount,
                    Unit = a.Unit,
                    UnitPrice = a.UnitPrice
                });
            });

            return list;
        }

        #endregion

        #region Clear

        private void ClearAll()
        {
            if (PurchaseId == 0)
            {
                SupplierDropdown.ComboBox.SelectedIndex = -1;
                SupplierDropdown.SelectedIndex = -1;
                PurchaseDate.Value = DateTime.Today;
                PONumberTextbox.Clear();
                TypeDropdown.SelectedIndex = -1;
                TypeDropdown.ComboBox.SelectedIndex = -1;
                PickupRB.Checked = true;
                CommentTextbox.Clear();
                PRTextbox.Clear();
                POTextbox.Clear();

                addItemControl1.ClearAll();

                SetAsNew();
            }
            else
                LoadPurchaseDetails();
        }

        private void SetAsNew()
        {
            //AddDetailBox.Visible = true;
            ErrorPanel.Visible = false;

            PONumberTextbox.ReadOnly = false;
            SupplierDropdown.Focus();

            SaveButton.Visible = true;
            ClearButton.Visible = true;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        #endregion

        #region Validation

        private void PONumberTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PONumberTextbox.Text))
                e.Cancel = true;
        }

        private void SupplierDropdown_Validating(object sender, CancelEventArgs e)
        {
            if (SupplierDropdown.SelectedIndex == -1)
                e.Cancel = true;
        }

        #region Check Duplicate

        private bool hasDuplicate = false;
        private void PONumberTextbox_TextChanged(object sender, EventArgs e)
        {
            CheckDuplicate();
        }

        private void CheckDuplicate()
        {
            if (!string.IsNullOrWhiteSpace(PONumberTextbox.Text) && SupplierDropdown.SelectedIndex != -1)
                hasDuplicate = DuplicateChecker.CodeHasDuplicate(PONumberTextbox.Text.Trim(), PurchaseId, (int)SupplierDropdown.SelectedValue);

            if (hasDuplicate)
                PONumberTextbox.StateCommon.Content.Color1 = Color.Red;
            else
                PONumberTextbox.StateCommon.Content.Color1 = Color.Black;
        }

        #endregion

        #endregion

        private void AddCommentButton_Click(object sender, EventArgs e)
        {
            CommentTextbox.Focus();
        }

        private void TypeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            addItemControl1.HasVat = (TypeDropdown.SelectedIndex == 0
                || TypeDropdown.SelectedIndex == 2) ? true : false;
        }

        #region PR/PO

        private void PRButton_Click(object sender, EventArgs e)
        {
            PRTextbox.Focus();
        }

        private void POButton_Click(object sender, EventArgs e)
        {
            POTextbox.Focus();
        }

        #endregion
    }

    public delegate void PurchaseUpdatedEventHandler(object sender, EventArgs e);
}