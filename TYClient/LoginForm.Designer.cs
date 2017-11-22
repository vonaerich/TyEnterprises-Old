namespace TY.SPIMS.Client
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.MySkin = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
            this.kryptonPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.kryptonGroupBox1 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.LoadImage = new System.Windows.Forms.PictureBox();
            this.LoginButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.PasswordTextbox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.UsernameTextbox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
            this.kryptonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1.Panel)).BeginInit();
            this.kryptonGroupBox1.Panel.SuspendLayout();
            this.kryptonGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).BeginInit();
            this.SuspendLayout();
            // 
            // MySkin
            // 
            this.MySkin.GlobalPaletteMode = ComponentFactory.Krypton.Toolkit.PaletteModeManager.SparkleBlue;
            // 
            // kryptonPanel
            // 
            this.kryptonPanel.Controls.Add(this.kryptonGroupBox1);
            this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel.Name = "kryptonPanel";
            this.kryptonPanel.Padding = new System.Windows.Forms.Padding(10, 5, 10, 10);
            this.kryptonPanel.Size = new System.Drawing.Size(309, 141);
            this.kryptonPanel.TabIndex = 0;
            // 
            // kryptonGroupBox1
            // 
            this.kryptonGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonGroupBox1.Location = new System.Drawing.Point(10, 5);
            this.kryptonGroupBox1.Name = "kryptonGroupBox1";
            // 
            // kryptonGroupBox1.Panel
            // 
            this.kryptonGroupBox1.Panel.Controls.Add(this.LoadImage);
            this.kryptonGroupBox1.Panel.Controls.Add(this.LoginButton);
            this.kryptonGroupBox1.Panel.Controls.Add(this.PasswordTextbox);
            this.kryptonGroupBox1.Panel.Controls.Add(this.UsernameTextbox);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel2);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel1);
            this.kryptonGroupBox1.Size = new System.Drawing.Size(289, 126);
            this.kryptonGroupBox1.TabIndex = 0;
            this.kryptonGroupBox1.Text = "User Login";
            this.kryptonGroupBox1.Values.Heading = "User Login";
            // 
            // LoadImage
            // 
            this.LoadImage.BackColor = System.Drawing.Color.Transparent;
            this.LoadImage.Image = global::TY.SPIMS.Client.Properties.Resources.ajax_loader;
            this.LoadImage.Location = new System.Drawing.Point(144, 70);
            this.LoadImage.Name = "LoadImage";
            this.LoadImage.Size = new System.Drawing.Size(16, 16);
            this.LoadImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.LoadImage.TabIndex = 3;
            this.LoadImage.TabStop = false;
            this.LoadImage.Visible = false;
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(166, 66);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(90, 25);
            this.LoginButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginButton.TabIndex = 2;
            this.LoginButton.Values.Text = "Login";
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // PasswordTextbox
            // 
            this.PasswordTextbox.AlwaysActive = false;
            this.PasswordTextbox.InputControlStyle = ComponentFactory.Krypton.Toolkit.InputControlStyle.Ribbon;
            this.PasswordTextbox.Location = new System.Drawing.Point(104, 36);
            this.PasswordTextbox.Name = "PasswordTextbox";
            this.PasswordTextbox.PasswordChar = '●';
            this.PasswordTextbox.Size = new System.Drawing.Size(152, 24);
            this.PasswordTextbox.StateCommon.Content.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PasswordTextbox.TabIndex = 1;
            this.PasswordTextbox.UseSystemPasswordChar = true;
            // 
            // UsernameTextbox
            // 
            this.UsernameTextbox.AlwaysActive = false;
            this.UsernameTextbox.InputControlStyle = ComponentFactory.Krypton.Toolkit.InputControlStyle.Ribbon;
            this.UsernameTextbox.Location = new System.Drawing.Point(104, 6);
            this.UsernameTextbox.Name = "UsernameTextbox";
            this.UsernameTextbox.Size = new System.Drawing.Size(152, 24);
            this.UsernameTextbox.StateCommon.Content.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UsernameTextbox.TabIndex = 0;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalPanel;
            this.kryptonLabel2.Location = new System.Drawing.Point(18, 36);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(76, 22);
            this.kryptonLabel2.StateCommon.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = "Password";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalPanel;
            this.kryptonLabel1.Location = new System.Drawing.Point(18, 6);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(80, 22);
            this.kryptonLabel1.StateCommon.ShortText.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "Username";
            // 
            // LoginForm
            // 
            this.AcceptButton = this.LoginButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 141);
            this.Controls.Add(this.kryptonPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HeaderStyle = ComponentFactory.Krypton.Toolkit.HeaderStyle.Secondary;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
            this.kryptonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1.Panel)).EndInit();
            this.kryptonGroupBox1.Panel.ResumeLayout(false);
            this.kryptonGroupBox1.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).EndInit();
            this.kryptonGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonManager MySkin;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox1;
        private System.Windows.Forms.PictureBox LoadImage;
        private ComponentFactory.Krypton.Toolkit.KryptonButton LoginButton;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox PasswordTextbox;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox UsernameTextbox;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
    }
}

