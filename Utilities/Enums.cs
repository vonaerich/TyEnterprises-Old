using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TY.SPIMS.Utilities
{
    public enum NumericSearchType
    {
        All, Equal, GreaterThan, LessThan
    }

    public enum SaleType
    {
        All, CashInvoice, Cash, CashCharge, SalesOrder, ChargeInvoice, NoInvoice
    }

    public enum PurchaseType
    {
        All, CashInvoice, DeliveryReceipt, ChargeInvoice, CashNoInvoice
    }

    public enum DateSearchType
    {
        All, SpecificDate, DateRange
    }

    public enum CustomerCreditType
    {
        Return, OverPayment
    }

    public enum PaidType
    { 
        None, Paid, NotPaid 
    }

    public enum DeliveredType
    {
        None, Delivered, NotDelivered
    }

    public enum CheckClearedType
    {
        None, Cleared, NotCleared
    }

    public enum PaymentType
    {
        Sales, Purchase
    }

    public enum CodeType
    {
        Brand, Customer, Supplier, User, Item, Sale, 
        Purchase, SalesReturn, PurchaseReturn, SalesPayment, PurchasePayment
    }

    public enum IssuerType
    { All, Customer, Us}

    public enum ReturnStatusType
    { All, Used, Unused }

    public enum PaymentRevertType
    { Sale, SalesReturn, Purchase, PurchaseReturn }

    class Enums
    {
    }
}
