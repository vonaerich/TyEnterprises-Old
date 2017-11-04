using System;
using TY.SPIMS.Controllers;
using TY.SPIMS.Controllers.Interfaces;

namespace TY.SPIMS.Client.Approval
{
    public partial class ApprovalForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        private readonly IInventoryUserController inventoryUserController;

        #region Event

        public event ApprovalDoneEventHandler ApprovalDone;
        protected void OnApprovalDone(ApprovalEventArgs e)
        {
            ApprovalDoneEventHandler handler = ApprovalDone;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        public ApprovalForm()
        {
            this.inventoryUserController = IOC.Container.GetInstance<InventoryUserController>();

            InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextbox.Text) ||
                string.IsNullOrWhiteSpace(PasswordTextbox.Text))
                return;

            try
            {
                var uname = UsernameTextbox.Text.Trim();
                var pw = PasswordTextbox.Text.Trim();

                var user = this.inventoryUserController.LoginUser(uname, pw);

                var userId = 0;
                if(user != null)
                {
                    var isApprover = user.IsApprover != null && user.IsApprover.Value;
                    var isAdmin = user.IsAdmin != null && user.IsAdmin.Value;
                    if (isApprover || isAdmin)
                        userId = user.Id;
                }

                if(ApprovalDone != null)
                    ApprovalDone(sender, new ApprovalEventArgs() { ApproverId = userId });

                this.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public delegate void ApprovalDoneEventHandler(object sender, ApprovalEventArgs e);

    public class ApprovalEventArgs
    {
        public int ApproverId { get; set; }
    }
}