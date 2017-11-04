CREATE VIEW [dbo].[PurchasesView] (	[Id],
	[InvoiceNumber],
	[CustomerId],
	[Supplier],
	[RecordedBy],
	[PurchaseDate],
	[Type],
	[TypeName],
	[TotalAmount],
	[IsPaid],
	[IsDeleted],
	[InvoiceDiscount],
	[Balance])
 AS 
SELECT     dbo.Purchase.Id, dbo.Purchase.PONumber AS InvoiceNumber, dbo.Customer.Id AS CustomerId, dbo.Customer.CompanyName AS Supplier, 
                      dbo.InventoryUser.Firstname + ' ' + dbo.InventoryUser.Lastname AS RecordedBy, dbo.Purchase.PurchaseDate, dbo.Purchase.Type, 
                      CASE WHEN dbo.Purchase.Type = 0 THEN '01-Cash Invoice' WHEN dbo.Purchase.Type = 1 THEN '02-Delivery Receipt' WHEN dbo.Purchase.Type = 2 THEN '03-Charge Invoice'
                       WHEN dbo.Purchase.Type = 3 THEN '04-Cash No Invoice' END AS TypeName, dbo.Purchase.TotalAmount, dbo.Purchase.IsPaid, dbo.Purchase.IsDeleted, 
                      dbo.Purchase.InvoiceDiscount, dbo.Purchase.Balance
FROM         dbo.Purchase LEFT OUTER JOIN
                      dbo.Customer ON dbo.Purchase.CustomerId = dbo.Customer.Id LEFT OUTER JOIN
                      dbo.InventoryUser ON dbo.Purchase.RecordedBy = dbo.InventoryUser.Id


