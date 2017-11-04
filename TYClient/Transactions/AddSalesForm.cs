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
    public partial class AddSalesForm : InputDetailsForm
    {
        public int SaleId { get; set; }

        #region Const

        private const string ZeroCurrency = "0.00";
        private const string Zero = "0";

        #endregion

        #region Event

        public event SalesUpdatedEventHandler SalesUpdated;

        protected void OnSalesUpdated(EventArgs e)
        {
            SalesUpdatedEventHandler handler = SalesUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        private BackgroundWorker salesWorker = new BackgroundWorker();
        private readonly ISaleController saleController;
        private readonly ICustomerController customerController;

        public AddSalesForm()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();
            this.saleController = IOC.Container.GetInstance<SaleController>();

            InitializeComponent();

            salesWorker.DoWork += new DoWorkEventHandler(salesWorker_DoWork);
            salesWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(salesWorker_RunWorkerCompleted);

            this.AutoValidate = AutoValidate.Disable;
        }

        #region Worker

        void salesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (SaleId == 0)
                ClientHelper.ShowSuccessMessage(string.Format("Invoice number {0} successfully added.", InvoiceNumberTextbox.Text));
            else
                ClientHelper.ShowSuccessMessage(string.Format("Invoice number {0} successfully updated.", InvoiceNumberTextbox.Text));

            ClearAll();

            SaveButton.Enabled = true;
            ClearButton.Enabled = true;

            LoadImage.Visible = false;
            //InvoiceNumberTextbox.Focus();
            CustomerDropdown.Focus();

            if (SalesUpdated != null)
                SalesUpdated(new object(), new EventArgs());
        }

        void salesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SaleColumnModel model = (SaleColumnModel)e.Argument;

            if (model.Id == 0)
                this.saleController.InsertSale(model);
            else
                this.saleController.UpdateSale(model);
        }

        #endregion

        #region Load

        private void AddSalesForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            addItemControl1.Type = TransactionType.Sale;
            addItemControl1.LoadControl();

            if (SaleId != 0)
                LoadSaleDetails();
        }

        #region For Edit

        private void CheckIfEditAllowed()
        {
            if (this.saleController.ItemHasPaymentOrReturn(SaleId))
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

        public void LoadSaleDetails()
        {
            if (SaleId != 0)
            {
                Sale s = this.saleController.FetchSaleById(SaleId);

                InvoiceNumberTextbox.Text = s.InvoiceNumber;
                CustomerDropdown.SelectedValue = s.CustomerId;
                SaleDate.Value = s.Date.HasValue ? s.Date.Value : DateTime.Now;
                CommentTextbox.Text = s.Comment;
                if (s.Type.HasValue)
                    TypeDropdown.SelectedIndex = s.Type.Value;

                if (s.PurchaseRequisition.Any())
                {
                    StringBuilder b = new StringBuilder();
                    foreach (var pr in s.PurchaseRequisition.OrderBy(a => a.PRNumber))
                    {
                        b.AppendLine(pr.PRNumber);
                    }
                    PRTextbox.Text = b.ToString();
                }

                if (s.PurchaseOrder.Any())
                {
                    StringBuilder b = new StringBuilder();
                    foreach (var po in s.PurchaseOrder.OrderBy(a => a.PONumber))
                    {
                        b.AppendLine(po.PONumber);
                    }
                    POTextbox.Text = b.ToString();
                }

                InvoiceNumberTextbox.ReadOnly = true;

                var details = this.saleController.FetchSaleDetails(SaleId).ToList();
                addItemControl1.ItemList = MapModelToItemList(details);
                addItemControl1.InvoiceDiscountPercent = s.InvoiceDiscountPercent.HasValue ? s.InvoiceDiscountPercent.Value : 0;
                addItemControl1.InvoiceDiscount = s.InvoiceDiscount.HasValue ? s.InvoiceDiscount.Value : 0;
                addItemControl1.NotifyAdmin = false;

                //CheckIfEditAllowed();
            }
            else
                ClearAll();
        }

        #endregion

        private void LoadCustomers()
        {
            customerDisplayModelBindingSource.DataSource = this.customerController.FetchAllCustomers();
            CustomerDropdown.SelectedIndex = -1;
        }

        private void CustomerDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CustomerDropdown.SelectedIndex != -1)
            {
                int selectedId = (int)CustomerDropdown.SelectedValue;

                Customer c = this.customerController.FetchCustomerById(selectedId);
                addItemControl1.CustomerName = c.CompanyName;
            }
        }

        #endregion

        #region Save
        private void SaveButton_Click(object sender, EventArgs e)
        {
            //Validate
            if (!this.ValidateChildren() || addItemControl1.ItemList.Count == 0)
                ClientHelper.ShowRequiredMessage("Invoice Number, Customer, Sales Items");
            else if (hasDuplicate)
                ClientHelper.ShowDuplicateMessage("Invoice Number");
            else
                SaveSale();
        }

        private void SaveSale()
        {
            LoadImage.Visible = true;

            SaveButton.Enabled = false;
            ClearButton.Enabled = false;

            List<SalesDetailViewModel> details = new List<SalesDetailViewModel>();
            MapItemListToModel(details);

            SaleColumnModel model = new SaleColumnModel()
            {
                Id = SaleId,
                CustomerId = (int)CustomerDropdown.SelectedValue,
                Date = SaleDate.Value,
                InvoiceNumber = InvoiceNumberTextbox.Text.Trim(),
                IsDeleted = false,
                IsPaid = false,
                RecordedBy = UserInfo.UserId,
                VatableSale = addItemControl1.VatableSales,
                Vat = addItemControl1.Vat,
                TotalAmount = addItemControl1.TotalAmount,
                Type = TypeDropdown.SelectedIndex != -1 ? TypeDropdown.SelectedIndex : 0,
                InvoiceDiscount = addItemControl1.InvoiceDiscount,
                InvoiceDiscountPercent = addItemControl1.InvoiceDiscountPercent,
                Comment = CommentTextbox.Text.Trim(),
                PR = PRTextbox.Text.Trim(),
                PO = POTextbox.Text.Trim(),
                Details = details
            };

            if (addItemControl1.NotifyAdmin)
            {
                string message = string.Format("Invoice number {0} contains items sold at a lower price.",
                    InvoiceNumberTextbox.Text.Trim());
                TY.SPIMS.Utilities.Helper.AddNotification(message);
            }

            salesWorker.RunWorkerAsync(model);
        }

        private void MapItemListToModel(List<SalesDetailViewModel> details)
        {
            addItemControl1.ItemList.ForEach((a) =>
            {
                details.Add(new SalesDetailViewModel()
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

        private List<IAutoPartDetailModel> MapModelToItemList(List<SalesDetailViewModel> model)
        {
            List<IAutoPartDetailModel> list = new List<IAutoPartDetailModel>();
            model.ForEach((a) =>
            {
                list.Add(new SalesDetailModel()
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
            if (SaleId == 0)
            {
                CustomerDropdown.ComboBox.SelectedIndex = -1;
                CustomerDropdown.SelectedIndex = -1;
                SaleDate.Value = DateTime.Today;
                InvoiceNumberTextbox.Clear();
                //CashRB.Checked = true;
                TypeDropdown.SelectedIndex = -1;
                TypeDropdown.ComboBox.SelectedIndex = -1;
                CommentTextbox.Clear();

                PRTextbox.Clear();
                POTextbox.Clear();

                //list.Clear();
                addItemControl1.ClearAll();

                SetAsNew();
            }
            else
                LoadSaleDetails();
        }

        private void SetAsNew()
        {
            //AddDetailBox.Visible = true;
            ErrorPanel.Visible = false;

            InvoiceNumberTextbox.ReadOnly = false;
            CustomerDropdown.Focus();
            //InvoiceTotalTextbox.Focus();

            SaveButton.Visible = true;
            ClearButton.Visible = true;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        #endregion

        #region Validation

        private void InvoiceNumberTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InvoiceNumberTextbox.Text))
                e.Cancel = true;
        }

        private void CustomerDropdown_Validating(object sender, CancelEventArgs e)
        {
            if (CustomerDropdown.SelectedIndex == -1)
                e.Cancel = true;
        }

        #region Check Duplicate

        private bool hasDuplicate = false;
        private void InvoiceNumberTextbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InvoiceNumberTextbox.Text))
                hasDuplicate = DuplicateChecker.CodeHasDuplicate(CodeType.Sale, InvoiceNumberTextbox.Text.Trim(), SaleId);

            if (hasDuplicate)
                InvoiceNumberTextbox.StateCommon.Content.Color1 = Color.Red;
            else
                InvoiceNumberTextbox.StateCommon.Content.Color1 = Color.Black;
        }

        #endregion

        #endregion

        #region Comment

        private void AddCommentButton_Click(object sender, EventArgs e)
        {
            CommentTextbox.Focus();
        }

        #endregion

        private void TypeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            addItemControl1.HasVat = (TypeDropdown.SelectedIndex == 0
                || TypeDropdown.SelectedIndex == 2
                || TypeDropdown.SelectedIndex == 4) ? true : false;
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

    public delegate void SalesUpdatedEventHandler(object sender, EventArgs e);
}