namespace TY.SPIMS.Client.Inventory
{
    partial class RecentPurchasesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.purchaseDetailViewModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.dataGridView1 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.kryptonGroup1 = new ComponentFactory.Krypton.Toolkit.KryptonGroup();
            this.SupplierTextbox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.InvoiceNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Supplier = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PurchaseDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.autoPartDetailIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.autoPartNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.autoPartNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.quantityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unitDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unitPriceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.discountPercentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.discountPercent2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.discountPercent3DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.discountedPriceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.discountedPrice2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.discountedPrice3DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.discountPercentsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalDiscountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalAmountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.purchaseDetailViewModelBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroup1)).BeginInit();
            this.kryptonGroup1.Panel.SuspendLayout();
            this.kryptonGroup1.SuspendLayout();
            this.SuspendLayout();
            // 
            // purchaseDetailViewModelBindingSource
            // 
            this.purchaseDetailViewModelBindingSource.DataSource = typeof(TY.SPIMS.POCOs.PurchaseDetailViewModel);
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.dataGridView1);
            this.kryptonPanel1.Controls.Add(this.kryptonGroup1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Padding = new System.Windows.Forms.Padding(10);
            this.kryptonPanel1.Size = new System.Drawing.Size(784, 426);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.InvoiceNumber,
            this.Supplier,
            this.PurchaseDate,
            this.autoPartDetailIdDataGridViewTextBoxColumn,
            this.autoPartNumberDataGridViewTextBoxColumn,
            this.autoPartNameDataGridViewTextBoxColumn,
            this.quantityDataGridViewTextBoxColumn,
            this.unitDataGridViewTextBoxColumn,
            this.unitPriceDataGridViewTextBoxColumn,
            this.discountPercentDataGridViewTextBoxColumn,
            this.discountPercent2DataGridViewTextBoxColumn,
            this.discountPercent3DataGridViewTextBoxColumn,
            this.discountedPriceDataGridViewTextBoxColumn,
            this.discountedPrice2DataGridViewTextBoxColumn,
            this.discountedPrice3DataGridViewTextBoxColumn,
            this.discountPercentsDataGridViewTextBoxColumn,
            this.totalDiscountDataGridViewTextBoxColumn,
            this.totalAmountDataGridViewTextBoxColumn});
            this.dataGridView1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dataGridView1.DataSource = this.purchaseDetailViewModelBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.GridStyles.Style = ComponentFactory.Krypton.Toolkit.DataGridViewStyle.Mixed;
            this.dataGridView1.GridStyles.StyleBackground = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.ControlClient;
            this.dataGridView1.Location = new System.Drawing.Point(10, 48);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Tahoma", 10F);
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView1.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridView1.RowTemplate.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(764, 368);
            this.dataGridView1.StateCommon.BackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.ControlClient;
            this.dataGridView1.StateCommon.HeaderColumn.Content.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridView1.TabIndex = 6;
            // 
            // kryptonGroup1
            // 
            this.kryptonGroup1.AutoSize = true;
            this.kryptonGroup1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonGroup1.Location = new System.Drawing.Point(10, 10);
            this.kryptonGroup1.Name = "kryptonGroup1";
            // 
            // kryptonGroup1.Panel
            // 
            this.kryptonGroup1.Panel.Controls.Add(this.SupplierTextbox);
            this.kryptonGroup1.Panel.Controls.Add(this.kryptonLabel1);
            this.kryptonGroup1.Panel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.kryptonGroup1.Size = new System.Drawing.Size(764, 38);
            this.kryptonGroup1.TabIndex = 5;
            // 
            // SupplierTextbox
            // 
            this.SupplierTextbox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.SupplierTextbox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.SupplierTextbox.Location = new System.Drawing.Point(76, 6);
            this.SupplierTextbox.Name = "SupplierTextbox";
            this.SupplierTextbox.Size = new System.Drawing.Size(167, 24);
            this.SupplierTextbox.StateCommon.Content.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SupplierTextbox.TabIndex = 1;
            this.SupplierTextbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SupplierTextbox_KeyUp);
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 6);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(67, 22);
            this.kryptonLabel1.StateCommon.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "Supplier";
            // 
            // InvoiceNumber
            // 
            this.InvoiceNumber.DataPropertyName = "InvoiceNumber";
            this.InvoiceNumber.HeaderText = "Invoice No.";
            this.InvoiceNumber.MinimumWidth = 90;
            this.InvoiceNumber.Name = "InvoiceNumber";
            this.InvoiceNumber.ReadOnly = true;
            this.InvoiceNumber.Width = 112;
            // 
            // Supplier
            // 
            this.Supplier.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Supplier.DataPropertyName = "Supplier";
            this.Supplier.HeaderText = "Supplier";
            this.Supplier.MinimumWidth = 130;
            this.Supplier.Name = "Supplier";
            this.Supplier.ReadOnly = true;
            // 
            // PurchaseDate
            // 
            this.PurchaseDate.DataPropertyName = "PurchaseDate";
            dataGridViewCellStyle2.Format = "d";
            this.PurchaseDate.DefaultCellStyle = dataGridViewCellStyle2;
            this.PurchaseDate.HeaderText = "Date";
            this.PurchaseDate.Name = "PurchaseDate";
            this.PurchaseDate.ReadOnly = true;
            this.PurchaseDate.Width = 68;
            // 
            // autoPartDetailIdDataGridViewTextBoxColumn
            // 
            this.autoPartDetailIdDataGridViewTextBoxColumn.DataPropertyName = "AutoPartDetailId";
            this.autoPartDetailIdDataGridViewTextBoxColumn.HeaderText = "AutoPartDetailId";
            this.autoPartDetailIdDataGridViewTextBoxColumn.Name = "autoPartDetailIdDataGridViewTextBoxColumn";
            this.autoPartDetailIdDataGridViewTextBoxColumn.ReadOnly = true;
            this.autoPartDetailIdDataGridViewTextBoxColumn.Visible = false;
            this.autoPartDetailIdDataGridViewTextBoxColumn.Width = 142;
            // 
            // autoPartNumberDataGridViewTextBoxColumn
            // 
            this.autoPartNumberDataGridViewTextBoxColumn.DataPropertyName = "AutoPartNumber";
            this.autoPartNumberDataGridViewTextBoxColumn.HeaderText = "AutoPartNumber";
            this.autoPartNumberDataGridViewTextBoxColumn.Name = "autoPartNumberDataGridViewTextBoxColumn";
            this.autoPartNumberDataGridViewTextBoxColumn.ReadOnly = true;
            this.autoPartNumberDataGridViewTextBoxColumn.Visible = false;
            this.autoPartNumberDataGridViewTextBoxColumn.Width = 145;
            // 
            // autoPartNameDataGridViewTextBoxColumn
            // 
            this.autoPartNameDataGridViewTextBoxColumn.DataPropertyName = "AutoPartName";
            this.autoPartNameDataGridViewTextBoxColumn.HeaderText = "AutoPartName";
            this.autoPartNameDataGridViewTextBoxColumn.Name = "autoPartNameDataGridViewTextBoxColumn";
            this.autoPartNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.autoPartNameDataGridViewTextBoxColumn.Visible = false;
            this.autoPartNameDataGridViewTextBoxColumn.Width = 132;
            // 
            // quantityDataGridViewTextBoxColumn
            // 
            this.quantityDataGridViewTextBoxColumn.DataPropertyName = "Quantity";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.quantityDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.quantityDataGridViewTextBoxColumn.HeaderText = "Qty";
            this.quantityDataGridViewTextBoxColumn.MinimumWidth = 80;
            this.quantityDataGridViewTextBoxColumn.Name = "quantityDataGridViewTextBoxColumn";
            this.quantityDataGridViewTextBoxColumn.ReadOnly = true;
            this.quantityDataGridViewTextBoxColumn.Width = 80;
            // 
            // unitDataGridViewTextBoxColumn
            // 
            this.unitDataGridViewTextBoxColumn.DataPropertyName = "Unit";
            this.unitDataGridViewTextBoxColumn.HeaderText = "Unit";
            this.unitDataGridViewTextBoxColumn.Name = "unitDataGridViewTextBoxColumn";
            this.unitDataGridViewTextBoxColumn.ReadOnly = true;
            this.unitDataGridViewTextBoxColumn.Width = 62;
            // 
            // unitPriceDataGridViewTextBoxColumn
            // 
            this.unitPriceDataGridViewTextBoxColumn.DataPropertyName = "UnitPrice";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "Php #,##0.00";
            this.unitPriceDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.unitPriceDataGridViewTextBoxColumn.HeaderText = "Net Unit Price";
            this.unitPriceDataGridViewTextBoxColumn.MinimumWidth = 110;
            this.unitPriceDataGridViewTextBoxColumn.Name = "unitPriceDataGridViewTextBoxColumn";
            this.unitPriceDataGridViewTextBoxColumn.ReadOnly = true;
            this.unitPriceDataGridViewTextBoxColumn.Width = 125;
            // 
            // discountPercentDataGridViewTextBoxColumn
            // 
            this.discountPercentDataGridViewTextBoxColumn.DataPropertyName = "DiscountPercent";
            this.discountPercentDataGridViewTextBoxColumn.HeaderText = "DiscountPercent";
            this.discountPercentDataGridViewTextBoxColumn.Name = "discountPercentDataGridViewTextBoxColumn";
            this.discountPercentDataGridViewTextBoxColumn.ReadOnly = true;
            this.discountPercentDataGridViewTextBoxColumn.Visible = false;
            this.discountPercentDataGridViewTextBoxColumn.Width = 141;
            // 
            // discountPercent2DataGridViewTextBoxColumn
            // 
            this.discountPercent2DataGridViewTextBoxColumn.DataPropertyName = "DiscountPercent2";
            this.discountPercent2DataGridViewTextBoxColumn.HeaderText = "DiscountPercent2";
            this.discountPercent2DataGridViewTextBoxColumn.Name = "discountPercent2DataGridViewTextBoxColumn";
            this.discountPercent2DataGridViewTextBoxColumn.ReadOnly = true;
            this.discountPercent2DataGridViewTextBoxColumn.Visible = false;
            this.discountPercent2DataGridViewTextBoxColumn.Width = 149;
            // 
            // discountPercent3DataGridViewTextBoxColumn
            // 
            this.discountPercent3DataGridViewTextBoxColumn.DataPropertyName = "DiscountPercent3";
            this.discountPercent3DataGridViewTextBoxColumn.HeaderText = "DiscountPercent3";
            this.discountPercent3DataGridViewTextBoxColumn.Name = "discountPercent3DataGridViewTextBoxColumn";
            this.discountPercent3DataGridViewTextBoxColumn.ReadOnly = true;
            this.discountPercent3DataGridViewTextBoxColumn.Visible = false;
            this.discountPercent3DataGridViewTextBoxColumn.Width = 149;
            // 
            // discountedPriceDataGridViewTextBoxColumn
            // 
            this.discountedPriceDataGridViewTextBoxColumn.DataPropertyName = "DiscountedPrice";
            this.discountedPriceDataGridViewTextBoxColumn.HeaderText = "DiscountedPrice";
            this.discountedPriceDataGridViewTextBoxColumn.Name = "discountedPriceDataGridViewTextBoxColumn";
            this.discountedPriceDataGridViewTextBoxColumn.ReadOnly = true;
            this.discountedPriceDataGridViewTextBoxColumn.Visible = false;
            this.discountedPriceDataGridViewTextBoxColumn.Width = 138;
            // 
            // discountedPrice2DataGridViewTextBoxColumn
            // 
            this.discountedPrice2DataGridViewTextBoxColumn.DataPropertyName = "DiscountedPrice2";
            this.discountedPrice2DataGridViewTextBoxColumn.HeaderText = "DiscountedPrice2";
            this.discountedPrice2DataGridViewTextBoxColumn.Name = "discountedPrice2DataGridViewTextBoxColumn";
            this.discountedPrice2DataGridViewTextBoxColumn.ReadOnly = true;
            this.discountedPrice2DataGridViewTextBoxColumn.Visible = false;
            this.discountedPrice2DataGridViewTextBoxColumn.Width = 146;
            // 
            // discountedPrice3DataGridViewTextBoxColumn
            // 
            this.discountedPrice3DataGridViewTextBoxColumn.DataPropertyName = "DiscountedPrice3";
            this.discountedPrice3DataGridViewTextBoxColumn.HeaderText = "DiscountedPrice3";
            this.discountedPrice3DataGridViewTextBoxColumn.Name = "discountedPrice3DataGridViewTextBoxColumn";
            this.discountedPrice3DataGridViewTextBoxColumn.ReadOnly = true;
            this.discountedPrice3DataGridViewTextBoxColumn.Visible = false;
            this.discountedPrice3DataGridViewTextBoxColumn.Width = 146;
            // 
            // discountPercentsDataGridViewTextBoxColumn
            // 
            this.discountPercentsDataGridViewTextBoxColumn.DataPropertyName = "DiscountPercents";
            this.discountPercentsDataGridViewTextBoxColumn.HeaderText = "DiscountPercents";
            this.discountPercentsDataGridViewTextBoxColumn.Name = "discountPercentsDataGridViewTextBoxColumn";
            this.discountPercentsDataGridViewTextBoxColumn.ReadOnly = true;
            this.discountPercentsDataGridViewTextBoxColumn.Visible = false;
            this.discountPercentsDataGridViewTextBoxColumn.Width = 148;
            // 
            // totalDiscountDataGridViewTextBoxColumn
            // 
            this.totalDiscountDataGridViewTextBoxColumn.DataPropertyName = "TotalDiscount";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "Php #,##0.00";
            this.totalDiscountDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.totalDiscountDataGridViewTextBoxColumn.HeaderText = "Total Discount";
            this.totalDiscountDataGridViewTextBoxColumn.MinimumWidth = 110;
            this.totalDiscountDataGridViewTextBoxColumn.Name = "totalDiscountDataGridViewTextBoxColumn";
            this.totalDiscountDataGridViewTextBoxColumn.ReadOnly = true;
            this.totalDiscountDataGridViewTextBoxColumn.Visible = false;
            this.totalDiscountDataGridViewTextBoxColumn.Width = 130;
            // 
            // totalAmountDataGridViewTextBoxColumn
            // 
            this.totalAmountDataGridViewTextBoxColumn.DataPropertyName = "TotalAmount";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "Php #,##0.00";
            this.totalAmountDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.totalAmountDataGridViewTextBoxColumn.HeaderText = "Total Amount";
            this.totalAmountDataGridViewTextBoxColumn.MinimumWidth = 110;
            this.totalAmountDataGridViewTextBoxColumn.Name = "totalAmountDataGridViewTextBoxColumn";
            this.totalAmountDataGridViewTextBoxColumn.ReadOnly = true;
            this.totalAmountDataGridViewTextBoxColumn.Visible = false;
            this.totalAmountDataGridViewTextBoxColumn.Width = 126;
            // 
            // RecentPurchasesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TY.SPIMS.Client.Properties.Resources.patternstripe;
            this.ClientSize = new System.Drawing.Size(784, 426);
            this.Controls.Add(this.kryptonPanel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "RecentPurchasesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recent Purchases";
            this.Load += new System.EventHandler(this.RecentSalesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.purchaseDetailViewModelBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.kryptonGroup1.Panel.ResumeLayout(false);
            this.kryptonGroup1.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroup1)).EndInit();
            this.kryptonGroup1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource purchaseDetailViewModelBindingSource;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView dataGridView1;
        private ComponentFactory.Krypton.Toolkit.KryptonGroup kryptonGroup1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox SupplierTextbox;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvoiceNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn Supplier;
        private System.Windows.Forms.DataGridViewTextBoxColumn PurchaseDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn autoPartDetailIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn autoPartNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn autoPartNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn quantityDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn unitDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn unitPriceDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn discountPercentDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn discountPercent2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn discountPercent3DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn discountedPriceDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn discountedPrice2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn discountedPrice3DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn discountPercentsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalDiscountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalAmountDataGridViewTextBoxColumn;

    }
}

