namespace TY.SPIMS.Client.Inventory
{
    partial class UpdatePricesForm
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
            this.kryptonHeaderGroup1 = new ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.SellingPrice2Textbox = new TY.SPIMS.Client.Controls.CurrencyTextbox();
            this.SellingPrice1Textbox = new TY.SPIMS.Client.Controls.CurrencyTextbox();
            this.PurchasePriceTextbox = new TY.SPIMS.Client.Controls.CurrencyTextbox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.SaveButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonHeaderGroup1)).BeginInit();
            this.kryptonHeaderGroup1.Panel.SuspendLayout();
            this.kryptonHeaderGroup1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonHeaderGroup1
            // 
            this.kryptonHeaderGroup1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonHeaderGroup1.GroupBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient;
            this.kryptonHeaderGroup1.HeaderStylePrimary = ComponentFactory.Krypton.Toolkit.HeaderStyle.Secondary;
            this.kryptonHeaderGroup1.HeaderVisibleSecondary = false;
            this.kryptonHeaderGroup1.Location = new System.Drawing.Point(5, 5);
            this.kryptonHeaderGroup1.Name = "kryptonHeaderGroup1";
            // 
            // kryptonHeaderGroup1.Panel
            // 
            this.kryptonHeaderGroup1.Panel.Controls.Add(this.tableLayoutPanel1);
            this.kryptonHeaderGroup1.Size = new System.Drawing.Size(293, 160);
            this.kryptonHeaderGroup1.TabIndex = 0;
            this.kryptonHeaderGroup1.ValuesPrimary.Heading = "Enter new prices";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.SellingPrice2Textbox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.SellingPrice1Textbox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.PurchasePriceTextbox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.kryptonLabel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.kryptonLabel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.kryptonLabel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(291, 135);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // SellingPrice2Textbox
            // 
            this.SellingPrice2Textbox.AlwaysActive = false;
            this.SellingPrice2Textbox.AutoCompleteValues = null;
            this.SellingPrice2Textbox.BackColor = System.Drawing.Color.Transparent;
            this.SellingPrice2Textbox.BackgroundColor = System.Drawing.Color.White;
            this.SellingPrice2Textbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SellingPrice2Textbox.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SellingPrice2Textbox.Location = new System.Drawing.Point(117, 70);
            this.SellingPrice2Textbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SellingPrice2Textbox.Name = "SellingPrice2Textbox";
            this.SellingPrice2Textbox.ReadOnly = false;
            this.SellingPrice2Textbox.Size = new System.Drawing.Size(120, 25);
            this.SellingPrice2Textbox.TabIndex = 2;
            // 
            // SellingPrice1Textbox
            // 
            this.SellingPrice1Textbox.AlwaysActive = false;
            this.SellingPrice1Textbox.AutoCompleteValues = null;
            this.SellingPrice1Textbox.BackColor = System.Drawing.Color.Transparent;
            this.SellingPrice1Textbox.BackgroundColor = System.Drawing.Color.White;
            this.SellingPrice1Textbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SellingPrice1Textbox.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SellingPrice1Textbox.Location = new System.Drawing.Point(117, 37);
            this.SellingPrice1Textbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SellingPrice1Textbox.Name = "SellingPrice1Textbox";
            this.SellingPrice1Textbox.ReadOnly = false;
            this.SellingPrice1Textbox.Size = new System.Drawing.Size(120, 25);
            this.SellingPrice1Textbox.TabIndex = 1;
            // 
            // PurchasePriceTextbox
            // 
            this.PurchasePriceTextbox.AlwaysActive = false;
            this.PurchasePriceTextbox.AutoCompleteValues = null;
            this.PurchasePriceTextbox.BackColor = System.Drawing.Color.Transparent;
            this.PurchasePriceTextbox.BackgroundColor = System.Drawing.Color.White;
            this.PurchasePriceTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PurchasePriceTextbox.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PurchasePriceTextbox.Location = new System.Drawing.Point(117, 4);
            this.PurchasePriceTextbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PurchasePriceTextbox.Name = "PurchasePriceTextbox";
            this.PurchasePriceTextbox.ReadOnly = false;
            this.PurchasePriceTextbox.Size = new System.Drawing.Size(120, 25);
            this.PurchasePriceTextbox.TabIndex = 0;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonLabel3.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalPanel;
            this.kryptonLabel3.Location = new System.Drawing.Point(3, 36);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(107, 27);
            this.kryptonLabel3.StateCommon.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel3.StateCommon.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.kryptonLabel3.TabIndex = 23;
            this.kryptonLabel3.Values.Text = "Selling Price 1";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonLabel2.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalPanel;
            this.kryptonLabel2.Location = new System.Drawing.Point(3, 69);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(107, 27);
            this.kryptonLabel2.StateCommon.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel2.StateCommon.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.kryptonLabel2.TabIndex = 22;
            this.kryptonLabel2.Values.Text = "Selling Price 2";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonLabel1.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalPanel;
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 3);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(107, 27);
            this.kryptonLabel1.StateCommon.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel1.StateCommon.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.kryptonLabel1.TabIndex = 21;
            this.kryptonLabel1.Values.Text = "Buying Price";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.SaveButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(113, 99);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(96, 28);
            this.flowLayoutPanel1.TabIndex = 27;
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(3, 0);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(90, 25);
            this.SaveButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveButton.TabIndex = 3;
            this.SaveButton.Values.Text = "&Save";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // UpdatePricesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 170);
            this.Controls.Add(this.kryptonHeaderGroup1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "UpdatePricesForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update Prices";
            this.Load += new System.EventHandler(this.UpdatePricesForm_Load);
            this.kryptonHeaderGroup1.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonHeaderGroup1)).EndInit();
            this.kryptonHeaderGroup1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup kryptonHeaderGroup1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private Controls.CurrencyTextbox SellingPrice2Textbox;
        private Controls.CurrencyTextbox SellingPrice1Textbox;
        private Controls.CurrencyTextbox PurchasePriceTextbox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton SaveButton;
    }
}

