using System;
using System.ComponentModel;
using System.Windows.Forms;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.POCOs;
using ComponentFactory.Krypton.Toolkit;

namespace TY.SPIMS.Client.Payment
{
    public partial class AddCheckForm : KryptonForm
    {
        #region Events

        public event CheckAddedEventHandler CheckAdded;

        protected void OnCheckAdded(CheckAddedEventArgs e)
        {
            CheckAddedEventHandler handler = CheckAdded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        public AddCheckForm()
        {
            InitializeComponent();

            this.AutoValidate = AutoValidate.Disable;
        }

        #region Load

        private void AddCheckForm_Load(object sender, EventArgs e)
        {
            LoadClearingDate();
        }

        private void LoadClearingDate()
        {
            ClearingDatePicker.Value = DateTime.Today.AddDays(3);
        }

        #endregion

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
                ClientHelper.ShowRequiredMessage("Check Number, Amount");
            else
            {
                CheckColumnModel model = new CheckColumnModel()
                {
                    CheckNumber = CheckNumberTextbox.Text,
                    Amount = decimal.Parse(AmountTextbox.Text),
                    Bank = BankTextbox.Text,
                    Branch = BranchTextbox.Text,
                    CheckDate = CheckDatePicker.Value,
                    ClearingDate = ClearingDatePicker.Value,
                    IsDeleted = false
                };

                CheckAdded(sender, new CheckAddedEventArgs() { CheckModel = model });
                this.Close();
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            CheckNumberTextbox.Clear();
            BankTextbox.Clear();
            BranchTextbox.Clear();
            CheckDatePicker.Value = DateTime.Today;
            ClearingDatePicker.Value = DateTime.Today;
            AmountTextbox.Text = "0.00";
        }

        #region DatePicker

        private void CheckDatePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = CheckDatePicker.Value;
            ClearingDatePicker.Value = selectedDate.AddDays(3);
        }

        #endregion

        #region Validation

        private void CheckNumberTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CheckNumberTextbox.Text))
                e.Cancel = true;
        }

        private void AmountTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (AmountTextbox.Text == "0.00")
                e.Cancel = true;
        }

        #endregion

        
        
    }

    public delegate void CheckAddedEventHandler(object sender, CheckAddedEventArgs e);

    public class CheckAddedEventArgs : EventArgs
    {
        public CheckColumnModel CheckModel { get; set; }
    }
}