CREATE TABLE [dbo].[AccessRights](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserType] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AccessSales] [bit] NULL,
	[AccessPurchase] [bit] NULL,
	[AccessInventory] [bit] NULL,
	[AccessCustomers] [bit] NULL,
	[AccessSuppliers] [bit] NULL,
	[AccessUsers] [bit] NULL,
	[AccessSettings] [bit] NULL,
	[AccessLogs] [bit] NULL,
	[AccessChecks] [bit] NULL,
	[AccessPayment] [bit] NULL,
	[AccessSalesReturn] [bit] NULL,
	[AccessPurchaseReturn] [bit] NULL,
	[AccessReports] [bit] NULL,
	[AccessBrands] [bit] NULL,
 CONSTRAINT [PK_AccessRights] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)


