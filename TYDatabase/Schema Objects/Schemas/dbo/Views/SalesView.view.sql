CREATE VIEW [dbo].[SalesView] (	[Id],
	[CustomerId],
	[CompanyName],
	[InvoiceNumber],
	[Date],
	[Type],
	[TypeName],
	[TotalAmount],
	[IsDeleted],
	[IsPaid],
	[RecordedBy],
	[InvoiceDiscount],
	[Balance])
 AS 
SELECT     dbo.Sale.Id, dbo.Sale.CustomerId, dbo.Customer.CompanyName, dbo.Sale.InvoiceNumber, dbo.Sale.Date, dbo.Sale.Type, 
                      CASE WHEN dbo.Sale.Type = 0 THEN '01-Cash Invoice' WHEN dbo.Sale.Type = 1 THEN '02-Cash/Petty/SOR' WHEN dbo.Sale.Type = 2 THEN '03-Cash/Charge Invoice' WHEN
                       dbo.Sale.Type = 3 THEN '04-Sales Order Slip' WHEN dbo.Sale.Type = 4 THEN '05-Charge Invoice' WHEN dbo.Sale.Type = 5 THEN '06-No Invoice' END AS TypeName, 
                      dbo.Sale.TotalAmount, dbo.Sale.IsDeleted, dbo.Sale.IsPaid, dbo.InventoryUser.Firstname + ' ' + dbo.InventoryUser.Lastname AS RecordedBy, 
                      dbo.Sale.InvoiceDiscount, dbo.Sale.Balance
FROM         dbo.InventoryUser RIGHT OUTER JOIN
                      dbo.Sale ON dbo.InventoryUser.Id = dbo.Sale.RecordedBy LEFT OUTER JOIN
                      dbo.Customer ON dbo.Sale.CustomerId = dbo.Customer.Id


