using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Controllers;
using System.IO;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Inventory
{
    public partial class ViewItemForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        private readonly IAutoPartController autoPartController;

        public int AutoPartDetailId { get; set; }
        BackgroundWorker pictureWorker = new BackgroundWorker();

        public ViewItemForm()
        {
            this.autoPartController = IOC.Container.GetInstance<AutoPartController>();

            InitializeComponent();

            pictureWorker.DoWork += new DoWorkEventHandler(pictureWorker_DoWork);
            pictureWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(pictureWorker_RunWorkerCompleted);
        }

        #region Worker

        void pictureWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                ItemPictureBox.ImageLocation = e.Result.ToString();
            }
        }

        void pictureWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                string serverIP = TY.SPIMS.Utilities.Helper.GetServerIP();
                string serverPath = string.Format(@"\\{0}\{1}\", serverIP, AutoPartController._pictureFolderName);
                string picPath = Path.Combine(serverPath, e.Argument.ToString());

                if(File.Exists(picPath))
                    e.Result = picPath;
            }
        }

        #endregion

        private void ViewItem_Load(object sender, EventArgs e)
        {
            LoadItemDetails();
            CheckUserAccess();

            if (UserInfo.IsVisitor == false)
                ItemPictureBox.Visible = false;
        }

        private void CheckUserAccess()
        {
            if (UserInfo.IsVisitor)
            {
                EditButton.Visible = false;
                BuyingGroup.Visible = false;
            }
        }

        private void LoadItemDetails()
        {
            if (AutoPartDetailId != 0)
            {
                var part = this.autoPartController.FetchAutoPartDetailById(AutoPartDetailId);
                if (part != null)
                {
                    AutoPartLabel.Text = part.AutoPart.PartName;
                    DescriptionLabel.Text = part.Description;
                    PartNumberLabel.Text = part.PartNumber;

                    if (part.AutoPartAltPartNumber.Count > 0)
                    {
                        StringBuilder s = new StringBuilder();
                        foreach (var p in part.AutoPartAltPartNumber)
                            s.AppendFormat("{0}{1}", p.AltPartNumber, Environment.NewLine);

                        AltPartNumberLabel.Text = s.ToString();
                    }
                    else
                        AltPartNumberLabel.Text = "-";
                    
                    BrandLabel.Text = part.Brand.BrandName;
                    MakeLabel.Text = !string.IsNullOrWhiteSpace(part.Make) ?
                        part.Make : "-";
                    ModelLabel.Text = !string.IsNullOrWhiteSpace(part.Model) ?
                        part.Model : "-";
                    SizeLabel.Text = !string.IsNullOrWhiteSpace(part.Size) ?
                        part.Size : "-";

                    SellingPriceLabel.Text = part.SellingPrice1.HasValue ?
                        part.SellingPrice1.Value.ToString("Php #,##0.00") : "Php 0.00";
                    SellingPrice2Label.Text = part.SellingPrice2.HasValue ?
                        part.SellingPrice2.Value.ToString("Php #,##0.00") : "Php 0.00";
                    BuyingLabel.Text = part.BuyingPrice.HasValue ?
                        part.BuyingPrice.Value.ToString("Php #,##0.00") : "Php 0.00";
                    QuantityLabel.Text = string.Format("{0} {1}",
                        part.Quantity.HasValue ? part.Quantity.Value.ToString() : "0",
                        part.Unit);

                    if (!string.IsNullOrWhiteSpace(part.Picture))
                    {
                        pictureWorker.RunWorkerAsync(part.Picture);
                    }
                }
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            ClientHelper.IsEdit = true;
            this.Close();
        }
    }
}