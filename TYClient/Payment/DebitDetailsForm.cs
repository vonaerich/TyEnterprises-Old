using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TY.SPIMS.Controllers;
using TY.SPIMS.Utilities;
using TY.SPIMS.Entities;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.POCOs;

namespace TY.SPIMS.Client.Payment
{
    public partial class DebitDetailsForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public int SupplierId { get; set; }
        public int PaymentId { get; set; }

        #region Events

        public event DebitSelectedEventHandler DebitSelected;
        protected void OnDebitSelected(DebitSelectedEventArgs args)
        {
            DebitSelectedEventHandler handler = DebitSelected;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        public DebitDetailsForm()
        {
            InitializeComponent();
        }

        private void DebitDetailsForm_Load(object sender, EventArgs e)
        {
            LoadSupplierDebit();

            if (PaymentId != 0)
                LoadPaymentDebit();
        }

        List<CustomerDebitDisplayModel> paymentDebits = new List<CustomerDebitDisplayModel>();
        private void LoadPaymentDebit()
        {
            PaymentDetail detail = PaymentDetailController.Instance.FetchPaymentDetailById(PaymentId);

            //Compute Credit
            //if (detail.CustomerDebit.Any())
            //{
            //    paymentDebits = detail.CustomerDebit.Select(a => new CustomerDebitDisplayModel()
            //    {
            //        Id = a.Id,
            //        Amount = a.Amount.HasValue ? a.Amount.Value : 0m,
            //        Debited = a.Debited.HasValue ? a.Debited.Value : false,
            //        Customer = a.Customer.CompanyName,
            //        Reference = a.Reference,
            //        Type = a.Type
            //    }).ToList();

            //    if (debits == null)
            //        debits = new SortableBindingList<CustomerDebitDisplayModel>(paymentDebits);
            //    else
            //    {
            //        foreach (var c in paymentDebits)
            //            debits.Add(c);
            //    }
            //}

            //if (debits.Any(a => a.Reference == detail.VoucherNumber))
            //    debits.Remove(debits.FirstOrDefault(a => a.Reference == detail.VoucherNumber));

            //customerDebitDisplayModelBindingSource.DataSource = null;
            //customerDebitDisplayModelBindingSource.DataSource = debits;
        }

        SortableBindingList<CustomerDebitDisplayModel> debits = null;
        private void LoadSupplierDebit()
        {
            if (SupplierId != 0)
            {
                CustomerDebitFilterModel filter = new CustomerDebitFilterModel() { CustomerId = SupplierId, Debited = false };
                debits = CustomerDebitController.Instance.FetchCustomerDebitWithSearch(filter);

                customerDebitDisplayModelBindingSource.DataSource = debits;
            }
            else
                customerDebitDisplayModelBindingSource.DataSource = null;
        }

        SortableBindingList<DebitDetailModel> selectedDebits = new SortableBindingList<DebitDetailModel>();
        int id = 0;
        decimal _origAmount = 0;
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                id = (int)selectedRow.Cells["IdCol"].Value;
                _origAmount = (decimal)selectedRow.Cells["AmountCol"].Value;
                AmountTextbox.Value = _origAmount;
                ReferenceTextbox.Text = selectedRow.Cells["ReferenceCol"].Value.ToString();
            }
        }

        private void ComputeTotal()
        {
            TotalTextbox.Text = selectedDebits.Sum(a => a.Amount).ToString("N2");
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DebitSelectedEventArgs args = new DebitSelectedEventArgs()
            {
                TotalDebit = TotalTextbox.Text.ToDecimal(),
                DebitSelected = selectedDebits.ToList()
            };

            DebitSelected(sender, args);
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
            //            int id = (int)row.Cells["IdCol"].Value;
            //            if (paymentDebits.Any(a => a.Id == id))
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

            if (!selectedDebits.Any(a => a.Reference == ReferenceTextbox.Text))
            {
                var newDebit = new DebitDetailModel()
                {
                    Id = this.id,
                    Reference = ReferenceTextbox.Text,
                    Amount = AmountTextbox.Value
                };

                selectedDebits.Add(newDebit);
            }
            else
            {
                var oldDebit = selectedDebits.FirstOrDefault(a => a.Reference == ReferenceTextbox.Text);
                oldDebit.Amount = AmountTextbox.Value;
            }

            BindSelectedDebits();
            ComputeTotal();
        }

        private void BindSelectedDebits()
        {
            debitDetailModelBindingSource.DataSource = null;
            debitDetailModelBindingSource.DataSource = selectedDebits;
        }
    }

    #region Delegate

    public delegate void DebitSelectedEventHandler(object sender, DebitSelectedEventArgs args);

    public class DebitSelectedEventArgs
    {
        public List<DebitDetailModel> DebitSelected { get; set; }
        public decimal TotalDebit { get; set; }
    }

    #endregion
}