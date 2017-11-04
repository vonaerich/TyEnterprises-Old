using System;
using System.Windows.Forms;
using SimpleInjector;
using SimpleInjector.Extensions.LifetimeScoping;
using TY.SPIMS.Controllers;
using TY.SPIMS.Controllers.Interfaces;
using TY.SPIMS.Entities;

namespace TY.SPIMS.Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Bootstrap();
            Application.Run(new LoginForm());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ClientHelper.LogException(e.Exception);
            ClientHelper.ShowErrorMessage("An error has occurred. Please contact your administrator.");
        }

        private static void Bootstrap()
        {
            // Create the container as usual.
            IOC.Container = new Container();
            IOC.Container.Options.DefaultScopedLifestyle = new LifetimeScopeLifestyle();

            // Register your types, for instance:
            IOC.Container.Register<IUnitOfWork, UnitOfWork>(Lifestyle.Singleton);
            IOC.Container.Register<IActionLogController, ActionLogController>();
            IOC.Container.Register<IAutoPartController, AutoPartController>();
            IOC.Container.Register<IBrandController, BrandController>();
            IOC.Container.Register<ICheckController, CheckController>();
            IOC.Container.Register<ICustomerController, CustomerController>();
            IOC.Container.Register<IInventoryUserController, InventoryUserController>();
            IOC.Container.Register<IPaymentDetailController, PaymentDetailController>();
            IOC.Container.Register<IPurchaseController, PurchaseController>();
            IOC.Container.Register<IPurchaseCounterController, PurchaseCounterController>();
            IOC.Container.Register<IPurchaseReturnController, PurchaseReturnController>();
            IOC.Container.Register<ISaleController, SaleController>();
            IOC.Container.Register<ISalesCounterController, SalesCounterController>();
            IOC.Container.Register<ISalesReturnController, SalesReturnController>();

            // Optionally verify the container.
            IOC.Container.Verify();
        }
    }
}