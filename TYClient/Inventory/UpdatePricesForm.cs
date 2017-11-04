using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Controllers;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Inventory
{
    public partial class UpdatePricesForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        private readonly IAutoPartController autoPartController;

        public int AutoPartDetailId { get; set; }

        public UpdatePricesForm()
        {
            this.autoPartController = IOC.Container.GetInstance<AutoPartController>();

            InitializeComponent();
        }

        private void UpdatePricesForm_Load(object sender, EventArgs e)
        {
            LoadDetails();
        }

        private void LoadDetails()
        {
            if (this.AutoPartDetailId != 0)
            {
                var autoPartDetail = this.autoPartController.FetchAutoPartDetailById(this.AutoPartDetailId);
                PurchasePriceTextbox.Text = autoPartDetail.BuyingPrice.HasValue ? autoPartDetail.BuyingPrice.Value.ToString("N2") : "0.00";
                SellingPrice1Textbox.Text = autoPartDetail.SellingPrice1.HasValue ? autoPartDetail.SellingPrice1.Value.ToString("N2") : "0.00";
                SellingPrice2Textbox.Text = autoPartDetail.SellingPrice2.HasValue ? autoPartDetail.SellingPrice2.Value.ToString("N2") : "0.00";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            decimal buyPrice = 0, sellPrice1 = 0, sellPrice2 = 0;
            
            if (!string.IsNullOrWhiteSpace(PurchasePriceTextbox.Text))
                buyPrice = decimal.Parse(PurchasePriceTextbox.Text);
            
            if (!string.IsNullOrWhiteSpace(SellingPrice1Textbox.Text))
                sellPrice1 = decimal.Parse(SellingPrice1Textbox.Text);
            
            if (!string.IsNullOrWhiteSpace(SellingPrice2Textbox.Text))
                sellPrice2 = decimal.Parse(SellingPrice2Textbox.Text);

            this.autoPartController.UpdatePartPrices(this.AutoPartDetailId,
                buyPrice, sellPrice1, sellPrice2);

            ClientHelper.ShowSuccessMessage("Prices updated successfully.");
            this.Close();
        }
    }
}