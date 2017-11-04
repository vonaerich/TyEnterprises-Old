using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using TY.SPIMS.Client.Approval;
using TY.SPIMS.Client.Brands;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Inventory
{
    public partial class AddItemForm : InputDetailsForm
    {
        private readonly IAutoPartController autoPartController;
        private readonly IBrandController brandController;

        private int _origQty = 0;
        private string _origPartName = string.Empty;

        public int AutoPartId { get; set; }

        #region Events

        public event AutoPartUpdatedEventHandler AutoPartUpdated;

        protected virtual void OnAutoPartUpdated(EventArgs e)
        {
            AutoPartUpdatedEventHandler handler = AutoPartUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        private BackgroundWorker itemWorker = new BackgroundWorker();
        public AddItemForm()
        {
            this.autoPartController = IOC.Container.GetInstance<AutoPartController>();
            this.brandController = IOC.Container.GetInstance<BrandController>();

            InitializeComponent();
            this.AutoValidate = AutoValidate.Disable;

            itemWorker.DoWork += new DoWorkEventHandler(itemWorker_DoWork);
            itemWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(itemWorker_RunWorkerCompleted);
        }

        #region Worker

        void itemWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (AutoPartId == 0)
                ClientHelper.ShowSuccessMessage("Auto part successfully added.");
            else
                ClientHelper.ShowSuccessMessage("Auto part details successfully updated.");

            ClearForm();
            LoadPartNames();

            if (AutoPartUpdated != null)
                AutoPartUpdated(new object(), new EventArgs());

            AutoPartTextbox.Focus();

            LoadImage.Visible = false;

            SaveButton.Enabled = true;
            ClearButton.Enabled = true;
        }

        void itemWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            AutoPartColumnModel model = (AutoPartColumnModel)e.Argument;

            if (model.Id == 0)
                this.autoPartController.InsertAutoPart(model);
            else
                this.autoPartController.UpdateAutoPart(model);
        }

        #endregion

        #region Load

        private void AddItemForm_Load(object sender, EventArgs e)
        {
            LoadBrands();
            LoadPartNames();

            if (AutoPartId != 0)
                LoadPartDetails();
        }

        public void LoadPartDetails()
        {
            if (AutoPartId != 0)
            {
                AutoPartDetail d = this.autoPartController.FetchAutoPartDetailById(AutoPartId);
                AutoPartTextbox.Text = d.AutoPart.PartName;
                DescriptionTextbox.Text = d.Description;
                PartNumberTextbox.Text = d.PartNumber;
                //AltTextbox.Text = d.AltPartNumber;
                BrandDropdown.SelectedValue = d.BrandId;
                ModelTextbox.Text = d.Model;
                MakeTextbox.Text = d.Make;
                SizeTextbox.Text = d.Size;
                UnitTextbox.Text = d.Unit;
                PurchasePriceTextbox.Text = d.BuyingPrice.HasValue ? d.BuyingPrice.Value.ToString("N2") : "0.00";
                SellingPriceTextbox.Text = d.SellingPrice1.HasValue ? d.SellingPrice1.Value.ToString("N2") : "0.00";
                SellingPrice2Textbox.Text = d.SellingPrice2.HasValue ? d.SellingPrice2.Value.ToString("N2") : "0.00";
                InitialQtyTextbox.Text = d.Quantity.HasValue ? d.Quantity.Value.ToString() : "0";
                ReorderTextbox.Text = d.ReorderLimit.HasValue ? d.ReorderLimit.Value.ToString() : "0";
                PictureTextbox.Text = !string.IsNullOrWhiteSpace(d.Picture) ? d.Picture : string.Empty;

                if (d.AutoPartAltPartNumber.Count > 0)
                {
                    StringBuilder altNumbers = new StringBuilder();
                    foreach (var alt in d.AutoPartAltPartNumber)
                        altNumbers.AppendFormat("{0}{1}", alt.AltPartNumber, Environment.NewLine);

                    AltTextbox.Text = altNumbers.ToString();
                }

                _origQty = d.Quantity.HasValue ? d.Quantity.Value : 0;
                _origPartName = d.AutoPart.PartName;
            }
            else
            {
                ClearForm();
            }
        }

        private void LoadPartNames()
        {
            List<string> partNames = this.autoPartController.FetchAllPartNames();

            if (partNames.Count > 0)
            {
                AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                collection.AddRange(partNames.ToArray());

                AutoPartTextbox.AutoCompleteCustomSource = collection;
            }
        }

        private void LoadBrands()
        {
            brandBindingSource.DataSource = this.brandController.FetchAllBrands();
            BrandDropdown.SelectedIndex = -1;
        }

        #endregion

        int partId = 0;
        private void AutoPartTextbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AutoPartTextbox.Text))
            {
                AutoPart part = this.autoPartController.FetchAutoPartByName(AutoPartTextbox.Text.Trim());

                if (part != null)
                    partId = part.Id;
                else
                    partId = 0;
            }
        }

        #region Save

        int? approver = null;
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
                ClientHelper.ShowRequiredMessage("Auto Part, Part Number, Brand, Model");
            else
            {
                if (AutoPartId != 0)
                {
                    int newQty = (int)InitialQtyTextbox.Value;
                    if ((newQty != _origQty || string.Compare(_origPartName, AutoPartTextbox.Text) != 0) 
                        && !UserInfo.IsAdmin)
                    {
                        ApprovalForm form = new ApprovalForm();
                        form.ApprovalDone += new ApprovalDoneEventHandler(form_ApprovalDone);
                        form.Show();
                    }
                    else
                    {
                        SaveAutoPart();
                    }
                }
                else
                {
                    SaveAutoPart();
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
                SaveAutoPart();
            }
        }

        private void SaveAutoPart()
        {
            LoadImage.Visible = true;

            SaveButton.Enabled = false;
            ClearButton.Enabled = false;

            List<string> altNumbers = new List<string>();

            if (!string.IsNullOrEmpty(AltTextbox.Text))
            {
                string[] numbers = AltTextbox.Text.Trim().Split(new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
                altNumbers.AddRange(numbers);
            }

            AutoPartColumnModel model = new AutoPartColumnModel()
            {
                Id = AutoPartId, //DetailId
                PartId = partId, //AutoPartId
                //Description = DescriptionTextbox.Text.Trim(),
                IsDeleted = false,
                PartName = AutoPartTextbox.Text.Trim(),
                TotalQuantity = int.Parse(InitialQtyTextbox.Text),
                AutoPartDetail = new AutoPartDetailColumnModel()
                {
                    BrandId = (int)BrandDropdown.SelectedValue,
                    IsDeleted = false,
                    Make = MakeTextbox.Text.Trim(),
                    Model = ModelTextbox.Text.Trim(),
                    PartNumber = PartNumberTextbox.Text.Trim(),
                    //AltPartNumber = AltTextbox.Text.Trim(),
                    AltPartNumbers = altNumbers,
                    PurchasePrice = !string.IsNullOrWhiteSpace(PurchasePriceTextbox.Text) ?
                                    decimal.Parse(PurchasePriceTextbox.Text) : 0,
                    Quantity = !string.IsNullOrWhiteSpace(InitialQtyTextbox.Text) ?
                                int.Parse(InitialQtyTextbox.Text) : 0,
                    ReorderLimit = !string.IsNullOrWhiteSpace(ReorderTextbox.Text) ?
                                    int.Parse(ReorderTextbox.Text) : 0,
                    SellingPrice = !string.IsNullOrWhiteSpace(SellingPriceTextbox.Text) ?
                                    decimal.Parse(SellingPriceTextbox.Text) : 0,
                    SellingPrice2 = !string.IsNullOrWhiteSpace(SellingPrice2Textbox.Text) ?
                                    decimal.Parse(SellingPrice2Textbox.Text) : 0,
                    Unit = UnitTextbox.Text,
                    Size = SizeTextbox.Text,
                    Picture = PictureTextbox.Text,
                    Description = DescriptionTextbox.Text
                },
            };

            itemWorker.RunWorkerAsync(model);
        }

        #endregion

        #region Clear

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            if (AutoPartId == 0)
            {
                AutoPartTextbox.Clear();
                DescriptionTextbox.Clear();
                MakeTextbox.Clear();
                ModelTextbox.Clear();
                brandBindingSource.Position = -1;
                BrandDropdown.SelectedIndex = -1;
                SellingPriceTextbox.Text = "0.00";
                SellingPrice2Textbox.Text = "0.00";
                PurchasePriceTextbox.Text = "0.00";
                ReorderTextbox.Text = "0";
                InitialQtyTextbox.Text = "0";
                UnitTextbox.Clear();
                PartNumberTextbox.Clear();
                SizeTextbox.Clear();
                AltTextbox.Clear();
                PictureTextbox.Clear();
            }
            else
                LoadPartDetails();
        }

        private void ClearBrandButton_Click(object sender, EventArgs e)
        {
            brandBindingSource.Position = -1;
            BrandDropdown.SelectedIndex = -1;
        }

        #endregion

        #region Validate

        private void AutoPartTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AutoPartTextbox.Text))
                e.Cancel = true;
        }

        private void PartNumberTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PartNumberTextbox.Text))
                e.Cancel = true;
        }

        private void BrandDropdown_Validating(object sender, CancelEventArgs e)
        {
            if (BrandDropdown.SelectedIndex == -1)
                e.Cancel = true;
        }

        private void ModelTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ModelTextbox.Text))
                e.Cancel = true;
        }

        #region Check Duplicate

        //private bool hasDuplicate = false;
        //private void PartNumberTextbox_TextChanged(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(PartNumberTextbox.Text))
        //        hasDuplicate = DuplicateChecker.CodeHasDuplicate(CodeType.Item, PartNumberTextbox.Text.Trim(), AutoPartId);

        //    if (hasDuplicate)
        //        PartNumberTextbox.StateCommon.Content.Color1 = Color.Red;
        //    else
        //        PartNumberTextbox.StateCommon.Content.Color1 = Color.Black;
        //}

        #endregion

        #endregion

        #region Add Brand

        private void AddBrandLink_LinkClicked(object sender, EventArgs e)
        {
            AddBrandForm form = new AddBrandForm();
            form.Owner = this.Owner;
            form.FromAddItem = true;
            form.BrandUpdated += new BrandUpdatedEventHandler(form_BrandUpdated);
            form.Show();
        }

        void form_BrandUpdated(object sender, EventArgs e)
        {
            LoadBrands();
        }

        #endregion

        private void AddPicture_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.bmp;*.gif;*.png";
            dialog.Title = "Save an Image File";
            dialog.ShowDialog();

            PictureTextbox.Text = dialog.FileName;
        }

        private void LinkButton_LinkClicked(object sender, EventArgs e)
        {
            SearchItemForm form = new SearchItemForm();
            form.ItemSelected += (s, a) => 
            { 
                if(!AltTextbox.Text.Contains(a.PartNumber) && PartNumberTextbox.Text != a.PartNumber)
                    AltTextbox.AppendText(string.Format("{0}\r\n", a.PartNumber)); 
            };
            form.Show();
        }

       

    }

    public delegate void AutoPartUpdatedEventHandler(object sender, EventArgs e);
}