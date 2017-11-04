using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Client.Payment
{
    public partial class CreditDetailsForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public int CustomerId { get; set; }
        public int PaymentId { get; set; }

        #region Events

        public event CreditSelectedEventHandler CreditSelected;
        protected void OnCreditSelected(CreditSelectedEventArgs args)
        {
            CreditSelectedEventHandler handler = CreditSelected;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        public CreditDetailsForm()
        {
            InitializeComponent();
        }

        private void CreditDetailsForm_Load(object sender, EventArgs e)
        {
            LoadCustomerCredit();

            if (PaymentId != 0)
                LoadPaymentCredit();

        }

        List<CustomerCreditDisplayModel> paymentCredits = new List<CustomerCreditDisplayModel>();
        private void LoadPaymentCredit()
        {
            PaymentDetail detail = PaymentDetailController.Instance.FetchPaymentDetailById(PaymentId);

            //Compute Credit
            //if (detail.CustomerCredits.Any())
            //{
            //    paymentCredits = detail.CustomerCredits.Select(a => new CustomerCreditDisplayModel()
            //    {
            //        Id = a.Id,
            //        Amount = a.Amount.HasValue ? a.Amount.Value : 0m,
            //        Credited = a.Credited.HasValue ? a.Credited.Value : false,
            //        Customer = a.Customer.CompanyName,
            //        Reference = a.Reference,
            //        Type = a.Type
            //    }).ToList();

            //    if (credits == null)
            //        credits = new SortableBindingList<CustomerCreditDisplayModel>(paymentCredits);
            //    else
            //    {
            //        foreach (var c in paymentCredits)
            //            credits.Add(c);
            //    }
            //}

            if (credits.Any(a => a.Reference == detail.VoucherNumber))
                credits.Remove(credits.FirstOrDefault(a => a.Reference == detail.VoucherNumber));

            customerCreditDisplayModelBindingSource.DataSource = null;
            customerCreditDisplayModelBindingSource.DataSource = credits;
        }

        SortableBindingList<CustomerCreditDisplayModel> credits = null;
        private void LoadCustomerCredit()
        {
            if (CustomerId != 0)
            {
                CustomerCreditFilterModel filter = new CustomerCreditFilterModel() { CustomerId = CustomerId, Credited = false };
                credits = CustomerCreditController.Instance.FetchCustomerCreditWithSearch(filter);

                customerCreditDisplayModelBindingSource.DataSource = credits;
            }
            else
                customerCreditDisplayModelBindingSource.DataSource = null;
        }

        SortableBindingList<CreditDetailModel> selectedCredits = new SortableBindingList<CreditDetailModel>();
        int id = 0;
        decimal _origAmount = 0;
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                id = (int)selectedRow.Cells["IdColumn"].Value;
                _origAmount = (decimal)selectedRow.Cells["AmountColumn"].Value;
                AmountTextbox.Value = _origAmount;
                ReferenceTextbox.Text = selectedRow.Cells["ReferenceColumn"].Value.ToString();
            }
        }

        private void ComputeTotal()
        {
            TotalTextbox.Text = selectedCredits.Sum(a => a.Amount).ToString("N2");
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            CreditSelectedEventArgs args = new CreditSelectedEventArgs()
            {
                TotalCredit = TotalTextbox.Text.ToDecimal(),
                CreditSelected = selectedCredits.ToList()
            };

            CreditSelected(sender, args);
            this.Close();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //if (dataGridView1.Rows.Count != 0)
            //{
            //    dataGridView1.ClearSelection();

            //    //If adding a payment, select all credits
            //    if (PaymentId == 0)
            //        dataGridView1.SelectAll();
            //    else //If editing, select those part of the payment only
            //    {
            //        foreach (DataGridViewRow row in dataGridView1.Rows)
            //        {
            //            int id = (int)row.Cells["IdColumn"].Value;
            //            if (paymentCredits.Any(a => a.Id == id))
            //                row.Selected = true;
            //            else
            //                row.Selected = false;
            //        }
            //    }
            //}
            //else
            //    TotalTextbox.Text = "0.00";
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            decimal newAmount = AmountTextbox.Value;
            if (newAmount > _origAmount)
            {
                ClientHelper.ShowErrorMessage("Amount cannot be greater than original amount.");
                return;
            }

            if (!selectedCredits.Any(a => a.Reference == ReferenceTextbox.Text))
            {
                var newCredit = new CreditDetailModel()
                {
                    Id = this.id,
                    Reference = ReferenceTextbox.Text,
                    Amount = AmountTextbox.Value
                };

                selectedCredits.Add(newCredit);
            }
            else
            {
                var oldCredit = selectedCredits.FirstOrDefault(a => a.Reference == ReferenceTextbox.Text);
                oldCredit.Amount = AmountTextbox.Value;
            }

            BindSelectedCredits();
            ComputeTotal();
        }

        private void BindSelectedCredits()
        {
            creditDetailModelBindingSource.DataSource = null;
            creditDetailModelBindingSource.DataSource = selectedCredits;
        }

        private void kryptonDataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
           
        }
    }

    #region Delegate

    public delegate void CreditSelectedEventHandler(object sender, CreditSelectedEventArgs args);

    public class CreditSelectedEventArgs
    {
        public List<CreditDetailModel> CreditSelected { get; set; }
        public decimal TotalCredit { get; set; }
    }

    #endregion
}