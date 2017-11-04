using System;
using System.Drawing;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Brands
{
    public partial class AddBrandForm : InputDetailsForm
    {
        private readonly IBrandController brandController;

        public int BrandId { get; set; }
        public bool FromAddItem { get; set; }

        public event BrandUpdatedEventHandler BrandUpdated;

        protected virtual void OnBrandUpdated(EventArgs e)
        {
            BrandUpdatedEventHandler handler = BrandUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public AddBrandForm()
        {
            this.brandController = IOC.Container.GetInstance<BrandController>();

            InitializeComponent();
        }

        #region Load

        private void AddBrandForm_Load(object sender, EventArgs e)
        {
            if (BrandId != 0)
                LoadBrandDetails();
        }

        public void LoadBrandDetails()
        {
            if (BrandId != 0)
            {
                Brand b = this.brandController.FetchBrandById(BrandId);

                IdTextbox.Text = b.Id.ToString();
                BrandTextbox.Text = b.BrandName;
            }
            else
                ClearForm();
        }

        #endregion

        #region Save

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveBrand();
        }

        private void SaveBrand()
        {
            if (string.IsNullOrWhiteSpace(BrandTextbox.Text))
            {
                ClientHelper.ShowRequiredMessage("Brand name");
                return;
            }

            if (hasDuplicate)
            {
                ClientHelper.ShowDuplicateMessage("Brand name");
                return;
            }

            if (IdTextbox.Text == "0")
            {
                this.brandController.InsertBrand(
                    new BrandColumnModel()
                    {
                        BrandName = BrandTextbox.Text.Trim(),
                        IsDeleted = false
                    });
                ClientHelper.ShowSuccessMessage("Brand added successfully.");
            }
            else
            {
                this.brandController.UpdateBrand(
                    new BrandColumnModel()
                    {
                        Id = BrandId,
                        BrandName = BrandTextbox.Text.Trim(),
                        IsDeleted = false
                    });
                ClientHelper.ShowSuccessMessage("Brand updated successfully.");
            }

            if (BrandUpdated != null)
                BrandUpdated(new object(), new EventArgs());
            ClearForm();

            if (FromAddItem)
                this.Close();
        }

        #endregion

        #region Clear

        private void ClearForm()
        {
            if (BrandId == 0)
            {
                IdTextbox.Text = "0";
                BrandTextbox.Clear();
            }
            else
            {
                IdTextbox.Text = BrandId.ToString();
                LoadBrandDetails();
            }
        }

        #endregion

        #region Duplicate

        private bool hasDuplicate = false;
        private void BrandTextbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(BrandTextbox.Text))
                hasDuplicate = DuplicateChecker.CodeHasDuplicate(CodeType.Brand, BrandTextbox.Text.Trim(), BrandId);

            if (hasDuplicate)
                BrandTextbox.ForeColor = Color.Red;
            else
                BrandTextbox.ForeColor = Color.Black;
        }

        #endregion
    }

    public delegate void BrandUpdatedEventHandler(object sender, EventArgs e);
}