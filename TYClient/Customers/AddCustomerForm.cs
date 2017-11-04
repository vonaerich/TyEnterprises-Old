using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TY.SPIMS.Client.Helper;
using TY.SPIMS.Controllers;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Customers
{
    public partial class AddCustomerForm : InputDetailsForm
    {
        private readonly ICustomerController customerController;

        public int CustomerId { get; set; }

        #region Events

        public event CustomerUpdatedEventHandler CustomerUpdated;

        protected virtual void OnCustomerUpdated(EventArgs e)
        {
            CustomerUpdatedEventHandler handler = CustomerUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        public AddCustomerForm()
        {
            this.customerController = IOC.Container.GetInstance<CustomerController>();

            InitializeComponent();
            this.AutoValidate = AutoValidate.Disable;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
                ClientHelper.ShowRequiredMessage("Customer Code, Company Name");
            else if (hasDuplicate)
                ClientHelper.ShowDuplicateMessage("Customer Code");
            else
            {
                CustomerColumnModel model = new CustomerColumnModel()
                {
                    Id = CustomerId,
                    Address = AddressTextbox.Text.Trim(),
                    Agent = AgentTextbox.Text.Trim(),
                    CompanyName = CompanyNameTextbox.Text.Trim(),
                    ContactPerson = ContactPersonTextbox.Text.Trim(),
                    CustomerCode = CodeTextbox.Text.Trim(),
                    FaxNumber = FaxTextbox.Text.Trim(),
                    PaymentTerms = int.Parse(PaymentTextbox.Text),
                    TIN = TINTextbox.Text.Trim(),
                    PhoneNumber = PhoneTextbox.Text.Trim(),
                    IsDeleted = false
                };

                if (CustomerId == 0)
                {
                    this.customerController.InsertCustomer(model);
                    ClientHelper.ShowSuccessMessage("Customer successfully added.");
                }
                else
                {
                    this.customerController.UpdateCustomer(model);
                    ClientHelper.ShowSuccessMessage("Customer successfully updated.");
                }

                ClearForm();
                CodeTextbox.Focus();

                if(CustomerUpdated != null)
                    CustomerUpdated(sender, e);
            }
        }

        #region Validate

        private void CodeTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CodeTextbox.Text))
                e.Cancel = true;
        }

        private void CompanyNameTextbox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CompanyNameTextbox.Text))
                e.Cancel = true;
        }

        #region Check Duplicate
        private bool hasDuplicate = false;
        private void CodeTextbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CodeTextbox.Text))
                hasDuplicate = DuplicateChecker.CodeHasDuplicate(CodeType.Customer, CodeTextbox.Text.Trim(), CustomerId);

            if (hasDuplicate)
                CodeTextbox.StateCommon.Content.Color1 = Color.Red;
            else
                CodeTextbox.StateCommon.Content.Color1 = Color.Black;
        }
        #endregion

        #endregion

        #region Clear

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            if (CustomerId == 0)
            {
                IdTextbox.Text = "0";
                AddressTextbox.Clear();
                AgentTextbox.Clear();
                CompanyNameTextbox.Clear();
                ContactPersonTextbox.Clear();
                CodeTextbox.Clear();
                FaxTextbox.Clear();
                PaymentTextbox.Text = "0";
                TINTextbox.Clear();
                PhoneTextbox.Clear();
            }
            else
                LoadCustomerDetails();
        }

        #endregion

        #region Load

        private void AddCustomerForm_Load(object sender, EventArgs e)
        {
            if (CustomerId != 0)
                LoadCustomerDetails();
        }

        public void LoadCustomerDetails()
        {
            if (CustomerId != 0)
            {
                Customer c = this.customerController.FetchCustomerById(CustomerId);

                IdTextbox.Text = c.Id.ToString();
                CodeTextbox.Text = c.CustomerCode;
                CompanyNameTextbox.Text = c.CompanyName;
                AddressTextbox.Text = c.Address;
                PhoneTextbox.Text = c.PhoneNumber;
                FaxTextbox.Text = c.FaxNumber;
                ContactPersonTextbox.Text = c.ContactPerson;
                TINTextbox.Text = c.TIN;
                PaymentTextbox.Text = c.PaymentTerms.ToString();
                AgentTextbox.Text = c.Agent;
            }
            else
                ClearForm();
        }

        #endregion
    }

    public delegate void CustomerUpdatedEventHandler(object sender, EventArgs e);
}