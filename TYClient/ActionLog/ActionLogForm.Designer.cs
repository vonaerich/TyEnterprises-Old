namespace TY.SPIMS.Client.ActionLog
{
    partial class ActionLogForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.MySkin = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
            this.kryptonPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.kryptonHeaderGroup1 = new ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup();
            this.dataGridView1 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.actionDisplayModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.FilterButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.DateToPicker = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.DateFromPicker = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.actionDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.actionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
            this.kryptonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonHeaderGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonHeaderGroup1.Panel)).BeginInit();
            this.kryptonHeaderGroup1.Panel.SuspendLayout();
            this.kryptonHeaderGroup1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionDisplayModelBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MySkin
            // 
            this.MySkin.GlobalPaletteMode = global::TY.SPIMS.Client.Properties.Settings.Default.PaletteMode;
            // 
            // kryptonPanel
            // 
            this.kryptonPanel.Controls.Add(this.kryptonHeaderGroup1);
            this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel.Name = "kryptonPanel";
            this.kryptonPanel.Padding = new System.Windows.Forms.Padding(10);
            this.kryptonPanel.Size = new System.Drawing.Size(777, 520);
            this.kryptonPanel.TabIndex = 0;
            // 
            // kryptonHeaderGroup1
            // 
            this.kryptonHeaderGroup1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonHeaderGroup1.HeaderVisibleSecondary = false;
            this.kryptonHeaderGroup1.Location = new System.Drawing.Point(10, 10);
            this.kryptonHeaderGroup1.Name = "kryptonHeaderGroup1";
            // 
            // kryptonHeaderGroup1.Panel
            // 
            this.kryptonHeaderGroup1.Panel.Controls.Add(this.dataGridView1);
            this.kryptonHeaderGroup1.Panel.Controls.Add(this.kryptonPanel1);
            this.kryptonHeaderGroup1.Size = new System.Drawing.Size(757, 500);
            this.kryptonHeaderGroup1.TabIndex = 0;
            this.kryptonHeaderGroup1.ValuesPrimary.Heading = "View Action Log";
            this.kryptonHeaderGroup1.ValuesPrimary.Image = null;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.actionDateDataGridViewTextBoxColumn,
            this.actionDataGridViewTextBoxColumn,
            this.userDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.actionDisplayModelBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.GridStyles.Style = ComponentFactory.Krypton.Toolkit.DataGridViewStyle.Mixed;
            this.dataGridView1.GridStyles.StyleBackground = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.ControlClient;
            this.dataGridView1.HideOuterBorders = true;
            this.dataGridView1.Location = new System.Drawing.Point(0, 35);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 10F);
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(755, 431);
            this.dataGridView1.TabIndex = 1;
            // 
            // actionDisplayModelBindingSource
            // 
            this.actionDisplayModelBindingSource.DataSource = typeof(TY.SPIMS.POCOs.ActionDisplayModel);
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.AutoSize = true;
            this.kryptonPanel1.Controls.Add(this.FilterButton);
            this.kryptonPanel1.Controls.Add(this.DateToPicker);
            this.kryptonPanel1.Controls.Add(this.DateFromPicker);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.kryptonPanel1.Size = new System.Drawing.Size(755, 35);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // FilterButton
            // 
            this.FilterButton.Location = new System.Drawing.Point(358, 5);
            this.FilterButton.Name = "FilterButton";
            this.FilterButton.Size = new System.Drawing.Size(90, 25);
            this.FilterButton.TabIndex = 4;
            this.FilterButton.Values.Text = "Filter";
            this.FilterButton.Click += new System.EventHandler(this.FilterButton_Click);
            // 
            // DateToPicker
            // 
            this.DateToPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DateToPicker.Location = new System.Drawing.Point(247, 7);
            this.DateToPicker.Name = "DateToPicker";
            this.DateToPicker.Size = new System.Drawing.Size(105, 21);
            this.DateToPicker.TabIndex = 3;
            // 
            // DateFromPicker
            // 
            this.DateFromPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DateFromPicker.Location = new System.Drawing.Point(77, 7);
            this.DateFromPicker.Name = "DateFromPicker";
            this.DateFromPicker.Size = new System.Drawing.Size(105, 21);
            this.DateFromPicker.TabIndex = 2;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalPanel;
            this.kryptonLabel2.Location = new System.Drawing.Point(188, 10);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(53, 20);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = "Date To";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalPanel;
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 10);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(68, 20);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "Date From";
            // 
            // actionDateDataGridViewTextBoxColumn
            // 
            this.actionDateDataGridViewTextBoxColumn.DataPropertyName = "ActionDate";
            dataGridViewCellStyle2.Format = "G";
            dataGridViewCellStyle2.NullValue = null;
            this.actionDateDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.actionDateDataGridViewTextBoxColumn.HeaderText = "Date";
            this.actionDateDataGridViewTextBoxColumn.Name = "actionDateDataGridViewTextBoxColumn";
            this.actionDateDataGridViewTextBoxColumn.ReadOnly = true;
            this.actionDateDataGridViewTextBoxColumn.Width = 200;
            // 
            // actionDataGridViewTextBoxColumn
            // 
            this.actionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.actionDataGridViewTextBoxColumn.DataPropertyName = "Action";
            this.actionDataGridViewTextBoxColumn.HeaderText = "Action";
            this.actionDataGridViewTextBoxColumn.Name = "actionDataGridViewTextBoxColumn";
            this.actionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // userDataGridViewTextBoxColumn
            // 
            this.userDataGridViewTextBoxColumn.DataPropertyName = "User";
            this.userDataGridViewTextBoxColumn.HeaderText = "User";
            this.userDataGridViewTextBoxColumn.Name = "userDataGridViewTextBoxColumn";
            this.userDataGridViewTextBoxColumn.ReadOnly = true;
            this.userDataGridViewTextBoxColumn.Width = 150;
            // 
            // ActionLogForm
            // 
            this.AcceptButton = this.FilterButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 520);
            this.Controls.Add(this.kryptonPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ActionLogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View Action Logs";
            this.Load += new System.EventHandler(this.ActionLogForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
            this.kryptonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonHeaderGroup1.Panel)).EndInit();
            this.kryptonHeaderGroup1.Panel.ResumeLayout(false);
            this.kryptonHeaderGroup1.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonHeaderGroup1)).EndInit();
            this.kryptonHeaderGroup1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionDisplayModelBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonManager MySkin;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup kryptonHeaderGroup1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView dataGridView1;
        private System.Windows.Forms.BindingSource actionDisplayModelBindingSource;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton FilterButton;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker DateToPicker;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker DateFromPicker;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn actionDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn actionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn userDataGridViewTextBoxColumn;
    }
}

