using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.Client.Inventory;
using TY.SPIMS.Client.DetailModel;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Controls
{
    public partial class AddItemControl : UserControl
    {
        private readonly IAutoPartController autoPartController;

        #region Constants
        private const string ZeroCurrency = "0.00";
        private const string Zero = "0";
        #endregion

        #region Props
        
        public TransactionType Type { get; set; }

        private List<IAutoPartDetailModel> _itemList;
        public List<IAutoPartDetailModel> ItemList
        {
            get 
            { 
                return this._itemList; 
            }
            set
            {
                this._itemList = value;
                BindList();
                ComputeTotalAll();
            }
        }
        
        private bool _hasVat;
        public bool HasVat 
        {
            get 
            { 
                return _hasVat; 
            }
            set 
            {
                _hasVat = value;
                ComputeTotalAll(); 
            } 
        }

        public bool IsDetailLeftHidden 
        { 
            set 
            {
                if (value) { AddDetailBox.Hide(); } 
            } 
        }

        private decimal _vatableSales;
        public decimal VatableSales 
        { 
            get { return _vatableSales; }
            set 
            { 
                _vatableSales = value;
                VatableTextbox.Text = _vatableSales.ToString("N2");
            }
 
        }

        private decimal _vat;
        public decimal Vat 
        {
            get { return _vat; }
            set
            {
                _vat = value;
                VatTextbox.Text = _vat.ToString("N2");
            } 
        }

        private decimal _totalAmount;
        public decimal TotalAmount 
        {
            get { return _totalAmount; }
            set
            {
                _totalAmount = value;
                InvoiceTotalTextbox.Text = _totalAmount.ToString("N2");
            }
        }

        private decimal _invoiceDiscount;
        public decimal InvoiceDiscount 
        {
            get { return _invoiceDiscount; }
            set
            {
                _invoiceDiscount = value;
                InvoiceDiscountTextbox.Text = _invoiceDiscount.ToString("N2");
            }
        }

        private decimal _invoiceDiscountPercent;
        public decimal InvoiceDiscountPercent
        {
            get { return _invoiceDiscountPercent; }
            set
            {
                _invoiceDiscountPercent = value;
                InvoiceDiscountPercentTextbox.Text = _invoiceDiscountPercent.ToString("N2");
            }
        }

        public bool NotifyAdmin { get; set; }
        public string CustomerName { get; set; }

        #endregion

        #region Private Properties

        private int _partNumberId;
        private int PartNumberId
        { 
            get { return this._partNumberId; }
            set 
            {
                this._partNumberId = value;
                //Load the details of this part number
                LoadAutoPartDetail(this._partNumberId);
            }
        }

        private decimal _buyingPrice;
        private decimal _minimumPrice;
        private int _qtyLeft;
        private bool _hasForm;

        #endregion

        public AddItemControl()
        {
            this.autoPartController = IOC.Container.GetInstance<AutoPartController>();

            InitializeComponent();
        }

        #region Load

        public void LoadControl()
        {
            this._itemList = new List<IAutoPartDetailModel>();
            LoadPartNumbers();
            ToggleTransactionControls();
        }

        private void ToggleTransactionControls()
        {
            if (this.Type == TransactionType.Sale)
            {
                VatableLabel.Text = "Vatable Sale";
                TotalLabel.Text = "Total Sales";

                AddItemButton.Visible = false;
            }
            else if (this.Type == TransactionType.Purchase)
            {
                VatableLabel.Text = "Vatable Purchases";
                TotalLabel.Text = "Total Purchases";
            }
        }

        private void LoadAutoPartDetail(int partNumber)
        {
            if (partNumber == 0)
                SetDetailsToNothing();
            else
            {
                var autoPartDetail = this.autoPartController.FetchAutoPartDetailById(partNumber);
                
                if (autoPartDetail != null)
                {
                    PartNumberSearchTextbox.Text = autoPartDetail.PartNumber;
                    UnitTextbox.Text = autoPartDetail.Unit;
                    UnitPriceTextbox.Text = this.Type == TransactionType.Sale ?
                        GetSalesPrice(autoPartDetail.SellingPrice1, autoPartDetail.SellingPrice2) :
                        GetBuyingPrice(autoPartDetail.BuyingPrice);
                    AutoPartLabel.Text = autoPartDetail.AutoPart.PartName;
                    BrandLabel.Text = autoPartDetail.Brand.BrandName;
                    ModelLabel.Text = autoPartDetail.Model;
                    MakeLabel.Text = autoPartDetail.Make;

                    this._buyingPrice = autoPartDetail.BuyingPrice != null ? 
                        autoPartDetail.BuyingPrice.Value :
                        0;

                    if (autoPartDetail.SellingPrice1 != null && autoPartDetail.SellingPrice1.Value != 0 
                        && autoPartDetail.SellingPrice2 != null && autoPartDetail.SellingPrice2.Value !=0)
                    {
                        this._minimumPrice = autoPartDetail.SellingPrice1.Value < autoPartDetail.SellingPrice2.Value ?
                            autoPartDetail.SellingPrice1.Value : autoPartDetail.SellingPrice2.Value;
                    }
                    else
                    {
                        if (autoPartDetail.SellingPrice1 != null && autoPartDetail.SellingPrice1.Value != 0)
                        { this._minimumPrice = autoPartDetail.SellingPrice1.Value; }
                        else if (autoPartDetail.SellingPrice2 != null && autoPartDetail.SellingPrice2.Value != 0)
                        { this._minimumPrice = autoPartDetail.SellingPrice2.Value; }
                    }
                }
            }
        }

        private void LoadPartNumbers()
        {
            var partNumbers = this.autoPartController
                .FetchAllPartNumbersWithAlternateNumbers();

            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
            collection.AddRange(partNumbers.ToArray());
            PartNumberSearchTextbox.AutoCompleteCustomSource = collection;
        }

        #endregion

        #region Add Item To List

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (this.Type == TransactionType.Sale)
                AddSaleItem();
            else if (this.Type == TransactionType.Purchase)
                AddPurchaseItem();

            BindList();
            ComputeTotalAll();

            this.PartNumberId = 0;
            if(!this._hasForm)
                PartNumberSearchTextbox.Focus();
        }

        private void AddSaleItem()
        {
            int qty = int.Parse(QtyTextbox.Text);
            decimal sellingPrice = UnitPriceTextbox.Text.ToDecimal();
            //bool hasLowerSellingPrice = false;

            //If no part number is selected, do nothing
            if (this.PartNumberId == 0)
            {
                ClientHelper.ShowErrorMessage("Part number does not exist.");
                return;
            }
            else if (qty > this._qtyLeft)
            {
                ClientHelper.ShowErrorMessage("Insufficient quantity.");
                return;
            }
            else if (sellingPrice < this._buyingPrice)
            {
                ClientHelper.ShowErrorMessage("Selling price cannot be less than purchase price.");
                return;
            }
            else if (sellingPrice < this._minimumPrice)
            {
                if (ClientHelper.ShowConfirmMessage("Price is lower than selling price. Administrators will be notified.\nDo you wish to continue?") ==
                     System.Windows.Forms.DialogResult.Yes)
                    this.NotifyAdmin = true;
                else
                    return;
            }

            IAutoPartDetailModel model = new SalesDetailModel()
            {
                AutoPartDetailId = this.PartNumberId,
                Name = string.Format("{0} - {1}\n{2} / {3} / {4}",
                                AutoPartLabel.Text, PartNumberSearchTextbox.Text,
                                BrandLabel.Text, ModelLabel.Text, MakeLabel.Text),
                Quantity = qty,
                Unit = UnitTextbox.Text,
                UnitPrice = sellingPrice,
                DiscountedPrice = DiscountTextbox.Text.ToDecimal(),
                DiscountedPrice2 = DiscountTextbox2.Text.ToDecimal(),
                DiscountedPrice3 = DiscountTextbox3.Text.ToDecimal(),
                DiscountPercent = DPercentTextbox.Text.ToDecimal(),
                DiscountPercent2 = DPercentTextbox2.Text.ToDecimal(),
                DiscountPercent3 = DPercentTextbox3.Text.ToDecimal(),
                DiscountPercents = string.Format("Less: {0}% + {1}% + {2}%",
                                    DPercentTextbox.Text == ZeroCurrency ? "0" : DPercentTextbox.Text,
                                    DPercentTextbox2.Text == ZeroCurrency ? "0" : DPercentTextbox2.Text,
                                    DPercentTextbox3.Text == ZeroCurrency ? "0" : DPercentTextbox3.Text),
                TotalDiscount = TotalDiscountTextbox.Text.ToDecimal(),
                TotalAmount = TotalAmountTextbox.Text.ToDecimal()
            };

            this.ItemList.Insert(0, model);
        }

        private void AddPurchaseItem()
        {
            int qty = int.Parse(QtyTextbox.Text);
            decimal buyingPrice = UnitPriceTextbox.Text.ToDecimal();
            decimal discountedPrice = DiscountTextbox3.Text.ToDecimal();

            //If no part number is selected, do nothing
            if (this.PartNumberId == 0)
            {
                ClientHelper.ShowErrorMessage("Part number does not exist.");
                return;
            }
            else if (discountedPrice > this._buyingPrice)
            {
                if (ClientHelper.ShowConfirmMessage("Do you want to update this auto part's prices?") ==
                     System.Windows.Forms.DialogResult.Yes)
                {
                    UpdatePricesForm form = new UpdatePricesForm();
                    form.AutoPartDetailId = this.PartNumberId;
                    form.Show();
                    form.Focus();
                    form.FormClosed += (s, a) =>
                    {
                        this._hasForm = false;
                        this.PartNumberSearchTextbox.Focus();
                    };
                    this._hasForm = true;
                }
                this.NotifyAdmin = true;
            }

            IAutoPartDetailModel model = new PurchaseDetailModel()
            {
                AutoPartDetailId = this.PartNumberId,
                Name = string.Format("{0} - {1}\n{2} / {3} / {4}",
                                AutoPartLabel.Text, PartNumberSearchTextbox.Text,
                                BrandLabel.Text, ModelLabel.Text, MakeLabel.Text),
                Quantity = qty,
                Unit = UnitTextbox.Text,
                UnitPrice = buyingPrice,
                DiscountedPrice = DiscountTextbox.Text.ToDecimal(),
                DiscountedPrice2 = DiscountTextbox2.Text.ToDecimal(),
                DiscountedPrice3 = DiscountTextbox3.Text.ToDecimal(),
                DiscountPercent = DPercentTextbox.Text.ToDecimal(),
                DiscountPercent2 = DPercentTextbox2.Text.ToDecimal(),
                DiscountPercent3 = DPercentTextbox3.Text.ToDecimal(),
                DiscountPercents = string.Format("Less: {0}% + {1}% + {2}%",
                                    DPercentTextbox.Text == ZeroCurrency ? "0" : DPercentTextbox.Text,
                                    DPercentTextbox2.Text == ZeroCurrency ? "0" : DPercentTextbox2.Text,
                                    DPercentTextbox3.Text == ZeroCurrency ? "0" : DPercentTextbox3.Text),
                TotalDiscount = TotalDiscountTextbox.Text.ToDecimal(),
                TotalAmount = TotalAmountTextbox.Text.ToDecimal()
            };

            this.ItemList.Insert(0, model);
        }

        private void BindList()
        {
            iAutoPartDetailModelBindingSource.DataSource = null;
            iAutoPartDetailModelBindingSource.DataSource = this.ItemList;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == dataGridView1.Columns["TotalDiscountColumn"].Index) && e.Value != null)
            {
                DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                object val = dataGridView1.Rows[e.RowIndex].Cells["DiscountPercentsColumn"].Value;
                cell.ToolTipText = val != null ? val.ToString() : string.Empty;
            }
        }
        
        #endregion

        #region Delete

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            ComputeTotalAll();
        }

        #endregion

        #region Textbox Events

        private void FindItemButton_Click(object sender, EventArgs e)
        {
            SearchItemForm form = new SearchItemForm();
            form.Owner = this.ParentForm;
            form.ItemSelected += this.ItemSelected;
            form.Show();
        }

        private void PartNumberSearchTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if(!string.IsNullOrWhiteSpace(PartNumberSearchTextbox.Text))
                {
                    DuplicateItemForm form = new DuplicateItemForm();
                    form.AltPartNumber = PartNumberSearchTextbox.Text;
                    form.ItemSelected += this.ItemSelected;
                    form.Show();
                }
            }
        }

        //decimal minimumPrice = 0m;
        void ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            ClearDetails();

            this.PartNumberId = e.AutoPartDetailId;

            if (this.Type == TransactionType.Sale)
            {
                this._qtyLeft = e.QtyLeft;
                this._qtyLeft -= this.ItemList.Where(a => a.AutoPartDetailId == e.AutoPartDetailId)
                    .Sum(a => a.Quantity);
            }

            QtyTextbox.Value = 1;

            SelectedPartNumberTextbox.Text = e.PartNumber;
            SelectedPartNumberTextbox.Focus();
        }

        #region For Computation

        private void SelectedPartNumberTextbox_TextChanged(object sender, EventArgs e)
        {
            ComputeTotalAmount();
        }

        private void QtyTextbox_Leave(object sender, EventArgs e)
        {
            ComputeTotalAmount();
        }

        private void UnitPriceTextbox_Leave(object sender, EventArgs e)
        {
            ComputeTotalAmount();
        }

        private void DPercentTextbox_Leave(object sender, EventArgs e)
        {
            ComputeTotalAmount();
        }

        private void DiscountTextbox_Leave(object sender, EventArgs e)
        {
            ComputeTotalAmount();
        }

        private void DPercentTextbox2_Leave(object sender, EventArgs e)
        {
            ComputeTotalAmount();
        }

        private void DiscountTextbox2_Leave(object sender, EventArgs e)
        {
            ComputeTotalAmount();
        }

        private void DPercentTextbox3_Leave(object sender, EventArgs e)
        {
            ComputeTotalAmount();
        }

        private void DiscountTextbox3_Leave(object sender, EventArgs e)
        {
            ComputeTotalAmount();
        }

        private void TotalDiscountTextbox_Leave(object sender, EventArgs e)
        {
            ComputeTotalAmountWithDiscount();
        }

        #endregion

        #endregion

        #region Computation

        private void ComputeTotalAmountWithDiscount()
        {
            decimal totalDiscount = 0m;
            int qty = 0;
            decimal unitPrice = 0m;

            if (!string.IsNullOrWhiteSpace(QtyTextbox.Text))
                qty = int.Parse(QtyTextbox.Text);

            if (!string.IsNullOrWhiteSpace(UnitPriceTextbox.Text))
                unitPrice = UnitPriceTextbox.Text.ToDecimal();

            if (!string.IsNullOrWhiteSpace(TotalDiscountTextbox.Text))
                totalDiscount = TotalDiscountTextbox.Text.ToDecimal();

            decimal netPrice = unitPrice - totalDiscount;
            DiscountTextbox3.Text = netPrice.ToString("N2");

            decimal totalPrice = qty * netPrice;
            TotalAmountTextbox.Text = totalPrice.ToString("N2");
        }

        private void ComputeTotalAmount()
        {
            int qty = 0;
            decimal unitPrice = 0m;
            decimal discountPercent = 0m;
            decimal discountPrice = 0m, discountPrice2 = 0m, discountPrice3 = 0m;
            decimal totalDiscount = 0m;

            if (!string.IsNullOrWhiteSpace(QtyTextbox.Text))
                qty = int.Parse(QtyTextbox.Text);
            else
                QtyTextbox.Text = Zero;

            if (!string.IsNullOrWhiteSpace(UnitPriceTextbox.Text))
                unitPrice = UnitPriceTextbox.Text.ToDecimal();
            else
                UnitPriceTextbox.Text = ZeroCurrency;

            if (!string.IsNullOrWhiteSpace(DPercentTextbox.Text))
            {
                discountPercent = DPercentTextbox.Text.ToDecimal();

                discountPrice = unitPrice * (100 - discountPercent) / 100;
                DiscountTextbox.Text = discountPrice.ToString("N2");
            }
            else
                DPercentTextbox.Text = ZeroCurrency;

            if (!string.IsNullOrWhiteSpace(DPercentTextbox2.Text))
            {
                decimal discountPercent2 = DPercentTextbox2.Text.ToDecimal();

                discountPrice2 = discountPrice * (100 - discountPercent2) / 100;
                DiscountTextbox2.Text = discountPrice2.ToString("N2");
            }
            else
                DPercentTextbox2.Text = ZeroCurrency;

            if (!string.IsNullOrWhiteSpace(DPercentTextbox3.Text))
            {
                decimal discountPercent3 = DPercentTextbox3.Text.ToDecimal();

                discountPrice3 = discountPrice2 * (100 - discountPercent3) / 100;
                DiscountTextbox3.Text = discountPrice3.ToString("N2");
            }
            else
                DPercentTextbox3.Text = ZeroCurrency;

            if (!string.IsNullOrWhiteSpace(DiscountTextbox3.Text) && discountPrice3 == 0)
                discountPrice3 = DiscountTextbox3.Text.ToDecimal();

            totalDiscount = unitPrice - discountPrice3;
            TotalDiscountTextbox.Text = totalDiscount.ToString("N2");

            decimal totalPrice = qty * (unitPrice - totalDiscount);
            TotalAmountTextbox.Text = totalPrice.ToString("N2");
        }

        private void ComputeTotalAll()
        {
            if (this.ItemList != null)
            {
                _invoiceDiscount = InvoiceDiscountTextbox.Value;
                decimal totalAmount = this.ItemList.Sum(a => a.TotalAmount) - this.InvoiceDiscount;
                decimal vat = 0;
                decimal vatable = totalAmount;

                if (HasVat)
                {
                    vatable = totalAmount / 1.12m;
                    vat = totalAmount - vatable;
                }

                this.VatableSales = vatable;
                this.Vat = vat;
                this.TotalAmount = totalAmount;
            }
        }

        #endregion
        
        #region Clear

        public void ClearAll()
        {
            ClearDetails();

            this.ItemList.Clear();
            BindList();

            ClearTotals();
            this.NotifyAdmin = false;
        }

        private void ClearTotals()
        {
            VatableTextbox.Text = ZeroCurrency;
            VatTextbox.Text = ZeroCurrency;
            InvoiceTotalTextbox.Text = ZeroCurrency;
            InvoiceDiscountTextbox.Text = ZeroCurrency;
            InvoiceDiscountPercentTextbox.Text = ZeroCurrency;
        }

        private void ClearDetailButton_Click(object sender, EventArgs e)
        {
            ClearDetails();
        }

        private void ClearDetails()
        {
            this.PartNumberId = 0;

            PartNumberSearchTextbox.ReadOnly = false;
            PartNumberSearchTextbox.Focus();
        }

        private void SetDetailsToNothing()
        {
            //this._partNumber = string.Empty;
            SelectedPartNumberTextbox.Clear();
            PartNumberSearchTextbox.Clear();
            QtyTextbox.Text = Zero;
            UnitTextbox.Clear();
            UnitPriceTextbox.Text = ZeroCurrency;
            DiscountTextbox.Text = ZeroCurrency;
            DPercentTextbox.Text = ZeroCurrency;
            DiscountTextbox2.Text = ZeroCurrency;
            DPercentTextbox2.Text = ZeroCurrency;
            DiscountTextbox3.Text = ZeroCurrency;
            DPercentTextbox3.Text = ZeroCurrency;
            TotalAmountTextbox.Text = ZeroCurrency;
            AutoPartLabel.Text = "-";
            BrandLabel.Text = "-";
            ModelLabel.Text = "-";
            MakeLabel.Text = "-";
            TotalDiscountTextbox.Text = ZeroCurrency;
        }

        #endregion

        #region Helper Methods

        private string GetBuyingPrice(decimal? buyingPrice)
        {
            if (buyingPrice != null)
                return buyingPrice.Value.ToString("0.00");
            else
                return ZeroCurrency;
        }

        private string GetSalesPrice(decimal? sellingPrice1, decimal? sellingPrice2)
        {
            if (sellingPrice1 != null || sellingPrice2 != null)
            {
                if (sellingPrice1 != null)
                    return sellingPrice1.Value.ToString("0.00");
                else if (sellingPrice2 != null)
                    return sellingPrice2.Value.ToString("0.00");
                else
                    return ZeroCurrency;
            }
            else
                return ZeroCurrency;

        }

        #endregion

        private void InvoiceDiscountPercentTextbox_ValueChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(InvoiceDiscountPercentTextbox.Text))
            {
                InvoiceDiscountPercentTextbox.Value = 0;
                return;
            }

            _invoiceDiscountPercent = InvoiceDiscountPercentTextbox.Value;
            decimal totalAmount = this.ItemList.Sum(a => a.TotalAmount);
            decimal discount = totalAmount * (InvoiceDiscountPercentTextbox.Value / 100);

            InvoiceDiscountTextbox.Value = discount;
        }

        private void InvoiceDiscountTextbox_ValueChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InvoiceDiscountTextbox.Text))
                InvoiceDiscountTextbox.Value = 0;

            ComputeTotalAll();
        }

        private void InvoiceDiscountLabel_Click(object sender, EventArgs e)
        {
            InvoiceDiscountPercentTextbox.Focus();
        }

        private void SelectedPartNumberTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.PartNumberId != 0)
            {
                if (e.KeyCode == Keys.F11)
                {
                    RecentPurchasesForm form = new RecentPurchasesForm();
                    form.AutoPartDetailId = this.PartNumberId;
                    form.SupplierName = this.CustomerName;
                    form.Show();
                }
                else if (e.KeyCode == Keys.F12)
                {
                    RecentSalesForm form = new RecentSalesForm();
                    form.AutoPartDetailId = this.PartNumberId;
                    form.CustomerName = this.CustomerName;
                    form.Show();
                }
            }
        }

        private void AddItemButton_Click(object sender, EventArgs e)
        {
            AddItemForm form = new AddItemForm();
            form.FormClosed += (s, a) => { LoadPartNumbers(); };
            form.Show();
        }
    }

    public enum TransactionType
    {
        Sale,
        Purchase
    }
}
