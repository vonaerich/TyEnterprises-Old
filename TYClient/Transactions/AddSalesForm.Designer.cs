using TY.SPIMS.Client.DetailModel;

namespace TY.SPIMS.Client.Transactions
{
    partial class AddSalesForm
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
            this.kryptonHeaderGroup1 = new ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.addItemControl1 = new TY.SPIMS.Client.Controls.AddItemControl();
            this.kryptonPanel5 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.kryptonPanel4 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.ErrorPanel = new System.Windows.Forms.Panel();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.LoadImage = new System.Windows.Forms.PictureBox();
            this.ClearButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.SaveButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonPanel2 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.kryptonHeaderGroup2 = new ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.POButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.PRButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.POTextbox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.PRTextbox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.CustomerDropdown = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.customerDisplayModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.SaleDate = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.InvoiceNumberTextbox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.CommentTextbox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.TypeDropdown = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.AddCommentButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.salesDetailViewModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonHeaderGroup1)).BeginInit();
            this.kryptonHeaderGroup1.Panel.SuspendLayout();
            this.kryptonHeaderGroup1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel4)).BeginInit();
            this.kryptonPanel4.SuspendLayout();
            this.ErrorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonHeaderGroup2)).BeginInit();
            this.kryptonHeaderGroup2.Panel.SuspendLayout();
            this.kryptonHeaderGroup2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CustomerDropdown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customerDisplayModelBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TypeDropdown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesDetailViewModelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonHeaderGroup1
            // 
            this.kryptonHeaderGroup1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonHeaderGroup1.Location = new System.Drawing.Point(5, 5);
            this.kryptonHeaderGroup1.Name = "kryptonHeaderGroup1";
            // 
            // kryptonHeaderGroup1.Panel
            // 
            this.kryptonHeaderGroup1.Panel.Controls.Add(this.kryptonPanel1);
            this.kryptonHeaderGroup1.Size = new System.Drawing.Size(1214, 668);
            this.kryptonHeaderGroup1.TabIndex = 0;
            this.kryptonHeaderGroup1.ValuesPrimary.Heading = "Sales Information";
            this.kryptonHeaderGroup1.ValuesSecondary.Heading = "Fields in red are required.";
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.addItemControl1);
            this.kryptonPanel1.Controls.Add(this.kryptonPanel5);
            this.kryptonPanel1.Controls.Add(this.kryptonPanel4);
            this.kryptonPanel1.Controls.Add(this.kryptonPanel2);
            this.kryptonPanel1.Controls.Add(this.kryptonHeaderGroup2);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.kryptonPanel1.Size = new System.Drawing.Size(1212, 615);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // addItemControl1
            // 
            this.addItemControl1.AutoSize = true;
            this.addItemControl1.CustomerName = null;
            this.addItemControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addItemControl1.HasVat = false;
            this.addItemControl1.InvoiceDiscount = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.addItemControl1.InvoiceDiscountPercent = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.addItemControl1.ItemList = null;
            this.addItemControl1.Location = new System.Drawing.Point(10, 151);
            this.addItemControl1.Name = "addItemControl1";
            this.addItemControl1.NotifyAdmin = false;
            this.addItemControl1.Size = new System.Drawing.Size(1192, 424);
            this.addItemControl1.TabIndex = 4;
            this.addItemControl1.TotalAmount = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.addItemControl1.Type = TY.SPIMS.Client.Controls.TransactionType.Sale;
            this.addItemControl1.Vat = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.addItemControl1.VatableSales = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // kryptonPanel5
            // 
            this.kryptonPanel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonPanel5.Location = new System.Drawing.Point(10, 575);
            this.kryptonPanel5.Name = "kryptonPanel5";
            this.kryptonPanel5.Size = new System.Drawing.Size(1192, 5);
            this.kryptonPanel5.TabIndex = 3;
            // 
            // kryptonPanel4
            // 
            this.kryptonPanel4.AutoSize = true;
            this.kryptonPanel4.Controls.Add(this.ErrorPanel);
            this.kryptonPanel4.Controls.Add(this.LoadImage);
            this.kryptonPanel4.Controls.Add(this.ClearButton);
            this.kryptonPanel4.Controls.Add(this.SaveButton);
            this.kryptonPanel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonPanel4.Location = new System.Drawing.Point(10, 580);
            this.kryptonPanel4.Name = "kryptonPanel4";
            this.kryptonPanel4.Size = new System.Drawing.Size(1192, 30);
            this.kryptonPanel4.TabIndex = 7;
            // 
            // ErrorPanel
            // 
            this.ErrorPanel.AutoSize = true;
            this.ErrorPanel.BackColor = System.Drawing.Color.Transparent;
            this.ErrorPanel.Controls.Add(this.kryptonLabel5);
            this.ErrorPanel.Location = new System.Drawing.Point(4, 0);
            this.ErrorPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ErrorPanel.Name = "ErrorPanel";
            this.ErrorPanel.Size = new System.Drawing.Size(615, 30);
            this.ErrorPanel.TabIndex = 24;
            this.ErrorPanel.Visible = false;
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalPanel;
            this.kryptonLabel5.Location = new System.Drawing.Point(3, 4);
            this.kryptonLabel5.Margin = new System.Windows.Forms.Padding(0);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(612, 22);
            this.kryptonLabel5.StateCommon.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel5.TabIndex = 0;
            this.kryptonLabel5.Values.Text = "* Unable to edit. This invoice is already paid or has recorded a return of mercha" +
    "ndise. *";
            // 
            // LoadImage
            // 
            this.LoadImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LoadImage.BackColor = System.Drawing.Color.Transparent;
            this.LoadImage.Image = global::TY.SPIMS.Client.Properties.Resources.ajax_loader;
            this.LoadImage.Location = new System.Drawing.Point(981, 7);
            this.LoadImage.Name = "LoadImage";
            this.LoadImage.Size = new System.Drawing.Size(16, 16);
            this.LoadImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.LoadImage.TabIndex = 23;
            this.LoadImage.TabStop = false;
            this.LoadImage.Visible = false;
            // 
            // ClearButton
            // 
            this.ClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearButton.Location = new System.Drawing.Point(1099, 2);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(90, 25);
            this.ClearButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClearButton.TabIndex = 1;
            this.ClearButton.Values.Text = "Cl&ear";
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveButton.Location = new System.Drawing.Point(1003, 2);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(90, 25);
            this.SaveButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveButton.TabIndex = 0;
            this.SaveButton.Values.Text = "&Save";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // kryptonPanel2
            // 
            this.kryptonPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonPanel2.Location = new System.Drawing.Point(10, 146);
            this.kryptonPanel2.Name = "kryptonPanel2";
            this.kryptonPanel2.Size = new System.Drawing.Size(1192, 5);
            this.kryptonPanel2.TabIndex = 1;
            // 
            // kryptonHeaderGroup2
            // 
            this.kryptonHeaderGroup2.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonHeaderGroup2.GroupBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.ControlRibbon;
            this.kryptonHeaderGroup2.HeaderStylePrimary = ComponentFactory.Krypton.Toolkit.HeaderStyle.Secondary;
            this.kryptonHeaderGroup2.HeaderVisibleSecondary = false;
            this.kryptonHeaderGroup2.Location = new System.Drawing.Point(10, 5);
            this.kryptonHeaderGroup2.Name = "kryptonHeaderGroup2";
            // 
            // kryptonHeaderGroup2.Panel
            // 
            this.kryptonHeaderGroup2.Panel.Controls.Add(this.tableLayoutPanel1);
            this.kryptonHeaderGroup2.Size = new System.Drawing.Size(1192, 141);
            this.kryptonHeaderGroup2.TabIndex = 0;
            this.kryptonHeaderGroup2.ValuesPrimary.Description = "Step 1: Fill out invoice details";
            this.kryptonHeaderGroup2.ValuesPrimary.Heading = "Invoice Details";
            this.kryptonHeaderGroup2.ValuesPrimary.Image = null;
            this.kryptonHeaderGroup2.ValuesSecondary.Heading = "";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 445F));
            this.tableLayoutPanel1.Controls.Add(this.POButton, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.PRButton, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.POTextbox, 5, 2);
            this.tableLayoutPanel1.Controls.Add(this.PRTextbox, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.kryptonLabel3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.kryptonLabel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.kryptonLabel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.CustomerDropdown, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.kryptonLabel4, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.SaleDate, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.InvoiceNumberTextbox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.CommentTextbox, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.TypeDropdown, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.AddCommentButton, 2, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1190, 118);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // POButton
            // 
            this.POButton.AutoSize = true;
            this.POButton.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.ButtonSpec;
            this.POButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.POButton.Location = new System.Drawing.Point(709, 65);
            this.POButton.Name = "POButton";
            this.POButton.Size = new System.Drawing.Size(133, 50);
            this.POButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.POButton.StateCommon.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.POButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.POButton.TabIndex = 21;
            this.POButton.TabStop = false;
            this.POButton.Values.Text = "Purchase &Order";
            this.POButton.Click += new System.EventHandler(this.POButton_Click);
            // 
            // PRButton
            // 
            this.PRButton.AutoSize = true;
            this.PRButton.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.ButtonSpec;
            this.PRButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PRButton.Location = new System.Drawing.Point(709, 3);
            this.PRButton.Name = "PRButton";
            this.PRButton.Size = new System.Drawing.Size(133, 25);
            this.PRButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PRButton.StateCommon.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.PRButton.TabIndex = 20;
            this.PRButton.TabStop = false;
            this.PRButton.Values.Text = "Purchase &Request";
            this.PRButton.Click += new System.EventHandler(this.PRButton_Click);
            // 
            // POTextbox
            // 
            this.POTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.POTextbox.Location = new System.Drawing.Point(848, 65);
            this.POTextbox.Multiline = true;
            this.POTextbox.Name = "POTextbox";
            this.POTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.POTextbox.Size = new System.Drawing.Size(185, 48);
            this.POTextbox.StateCommon.Content.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.POTextbox.TabIndex = 19;
            this.POTextbox.TabStop = false;
            // 
            // PRTextbox
            // 
            this.PRTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.PRTextbox.Location = new System.Drawing.Point(848, 3);
            this.PRTextbox.Multiline = true;
            this.PRTextbox.Name = "PRTextbox";
            this.tableLayoutPanel1.SetRowSpan(this.PRTextbox, 2);
            this.PRTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PRTextbox.Size = new System.Drawing.Size(185, 54);
            this.PRTextbox.StateCommon.Content.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PRTextbox.TabIndex = 18;
            this.PRTextbox.TabStop = false;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonLabel3.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalPanel;
            this.kryptonLabel3.Location = new System.Drawing.Point(435, 3);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(77, 25);
            this.kryptonLabel3.StateCommon.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel3.StateCommon.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.kryptonLabel3.TabIndex = 4;
            this.kryptonLabel3.Values.Text = "Type";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonLabel1.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalPanel;
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 34);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(120, 25);
            this.kryptonLabel1.StateCommon.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel1.StateCommon.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "Invoice Number";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonLabel2.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalPanel;
            this.kryptonLabel2.Location = new System.Drawing.Point(3, 3);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(120, 25);
            this.kryptonLabel2.StateCommon.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel2.StateCommon.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = "Customer";
            // 
            // CustomerDropdown
            // 
            this.CustomerDropdown.AlwaysActive = false;
            this.CustomerDropdown.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.CustomerDropdown.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CustomerDropdown.DataSource = this.customerDisplayModelBindingSource;
            this.CustomerDropdown.DisplayMember = "CompanyName";
            this.CustomerDropdown.DropDownWidth = 500;
            this.CustomerDropdown.Location = new System.Drawing.Point(129, 3);
            this.CustomerDropdown.Name = "CustomerDropdown";
            this.CustomerDropdown.Size = new System.Drawing.Size(300, 24);
            this.CustomerDropdown.StateCommon.ComboBox.Back.Color1 = System.Drawing.Color.SeaShell;
            this.CustomerDropdown.StateCommon.ComboBox.Content.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CustomerDropdown.StateCommon.Item.Content.ShortText.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CustomerDropdown.StateCommon.Item.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.CustomerDropdown.TabIndex = 0;
            this.CustomerDropdown.ValueMember = "Id";
            this.CustomerDropdown.SelectedIndexChanged += new System.EventHandler(this.CustomerDropdown_SelectedIndexChanged);
            this.CustomerDropdown.Validating += new System.ComponentModel.CancelEventHandler(this.CustomerDropdown_Validating);
            // 
            // customerDisplayModelBindingSource
            // 
            this.customerDisplayModelBindingSource.DataSource = typeof(TY.SPIMS.POCOs.CustomerDisplayModel);
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonLabel4.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalPanel;
            this.kryptonLabel4.Location = new System.Drawing.Point(435, 34);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(77, 25);
            this.kryptonLabel4.StateCommon.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel4.StateCommon.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.kryptonLabel4.TabIndex = 5;
            this.kryptonLabel4.Values.Text = "Date";
            // 
            // SaleDate
            // 
            this.SaleDate.AlwaysActive = false;
            this.SaleDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.SaleDate.Location = new System.Drawing.Point(518, 34);
            this.SaleDate.Name = "SaleDate";
            this.SaleDate.Size = new System.Drawing.Size(185, 25);
            this.SaleDate.StateCommon.Content.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaleDate.TabIndex = 3;
            // 
            // InvoiceNumberTextbox
            // 
            this.InvoiceNumberTextbox.AlwaysActive = false;
            this.InvoiceNumberTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.InvoiceNumberTextbox.Location = new System.Drawing.Point(129, 34);
            this.InvoiceNumberTextbox.Name = "InvoiceNumberTextbox";
            this.InvoiceNumberTextbox.Size = new System.Drawing.Size(300, 23);
            this.InvoiceNumberTextbox.StateCommon.Back.Color1 = System.Drawing.Color.SeaShell;
            this.InvoiceNumberTextbox.StateCommon.Content.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InvoiceNumberTextbox.TabIndex = 1;
            this.InvoiceNumberTextbox.TextChanged += new System.EventHandler(this.InvoiceNumberTextbox_TextChanged);
            this.InvoiceNumberTextbox.Validating += new System.ComponentModel.CancelEventHandler(this.InvoiceNumberTextbox_Validating);
            // 
            // CommentTextbox
            // 
            this.CommentTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.CommentTextbox.Location = new System.Drawing.Point(518, 65);
            this.CommentTextbox.Multiline = true;
            this.CommentTextbox.Name = "CommentTextbox";
            this.CommentTextbox.Size = new System.Drawing.Size(185, 48);
            this.CommentTextbox.StateCommon.Content.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommentTextbox.TabIndex = 10;
            this.CommentTextbox.TabStop = false;
            // 
            // TypeDropdown
            // 
            this.TypeDropdown.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.TypeDropdown.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.TypeDropdown.DropDownWidth = 155;
            this.TypeDropdown.Items.AddRange(new object[] {
            "01-Cash Invoice",
            "02-Cash/Petty/SOR",
            "03-Cash/Charge Invoice",
            "04-Sales Order Slip",
            "05-Charge Invoice",
            "06-No Invoice"});
            this.TypeDropdown.Location = new System.Drawing.Point(518, 3);
            this.TypeDropdown.Name = "TypeDropdown";
            this.TypeDropdown.Size = new System.Drawing.Size(185, 25);
            this.TypeDropdown.StateCommon.ComboBox.Content.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TypeDropdown.StateCommon.Item.Content.ShortText.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TypeDropdown.TabIndex = 2;
            this.TypeDropdown.SelectedIndexChanged += new System.EventHandler(this.TypeDropdown_SelectedIndexChanged);
            // 
            // AddCommentButton
            // 
            this.AddCommentButton.AutoSize = true;
            this.AddCommentButton.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.ButtonSpec;
            this.AddCommentButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddCommentButton.Location = new System.Drawing.Point(435, 65);
            this.AddCommentButton.Name = "AddCommentButton";
            this.AddCommentButton.Size = new System.Drawing.Size(77, 50);
            this.AddCommentButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddCommentButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.AddCommentButton.TabIndex = 15;
            this.AddCommentButton.TabStop = false;
            this.AddCommentButton.Values.Text = "Comme&nt\r\n(Alt + N)";
            this.AddCommentButton.Click += new System.EventHandler(this.AddCommentButton_Click);
            // 
            // salesDetailViewModelBindingSource
            // 
            this.salesDetailViewModelBindingSource.DataSource = typeof(TY.SPIMS.POCOs.SalesDetailViewModel);
            // 
            // AddSalesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TY.SPIMS.Client.Properties.Resources.patternstripe;
            this.ClientSize = new System.Drawing.Size(1224, 678);
            this.Controls.Add(this.kryptonHeaderGroup1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddSalesForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sales Details";
            this.Load += new System.EventHandler(this.AddSalesForm_Load);
            this.kryptonHeaderGroup1.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonHeaderGroup1)).EndInit();
            this.kryptonHeaderGroup1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel4)).EndInit();
            this.kryptonPanel4.ResumeLayout(false);
            this.kryptonPanel4.PerformLayout();
            this.ErrorPanel.ResumeLayout(false);
            this.ErrorPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).EndInit();
            this.kryptonHeaderGroup2.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonHeaderGroup2)).EndInit();
            this.kryptonHeaderGroup2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CustomerDropdown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customerDisplayModelBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TypeDropdown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesDetailViewModelBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup kryptonHeaderGroup1;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel2;
        private ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup kryptonHeaderGroup2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox CustomerDropdown;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker SaleDate;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox InvoiceNumberTextbox;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel5;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel4;
        private ComponentFactory.Krypton.Toolkit.KryptonButton ClearButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton SaveButton;
        private System.Windows.Forms.BindingSource salesDetailViewModelBindingSource;
        private System.Windows.Forms.PictureBox LoadImage;
        private System.Windows.Forms.Panel ErrorPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private System.Windows.Forms.BindingSource customerDisplayModelBindingSource;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox CommentTextbox;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox TypeDropdown;
        private ComponentFactory.Krypton.Toolkit.KryptonButton AddCommentButton;
        private Controls.AddItemControl addItemControl1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox POTextbox;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox PRTextbox;
        private ComponentFactory.Krypton.Toolkit.KryptonButton POButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton PRButton;
    }
}

