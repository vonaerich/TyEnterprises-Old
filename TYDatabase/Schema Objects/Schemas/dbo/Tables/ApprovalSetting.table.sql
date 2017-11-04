CREATE TABLE [dbo].[ApprovalSetting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SalesAdd] [bit] NULL,
	[PurchaseAdd] [bit] NULL,
	[InventoryAdd] [bit] NULL,
	[SalesReturnAdd] [bit] NULL,
	[PurchaseReturnAdd] [bit] NULL,
	[PaymentAdd] [bit] NULL,
	[SalesEdit] [bit] NULL,
	[SalesDelete] [bit] NULL,
	[PurchaseEdit] [bit] NULL,
	[PurchaseDelete] [bit] NULL,
	[InventoryEdit] [bit] NULL,
	[InventoryDelete] [bit] NULL,
	[SalesReturnEdit] [bit] NULL,
	[SalesReturnDelete] [bit] NULL,
	[PurchaseReturnEdit] [bit] NULL,
	[PurchaseReturnDelete] [bit] NULL,
	[PaymentEdit] [bit] NULL,
	[PaymentDelete] [bit] NULL,
 CONSTRAINT [PK_ApprovalSetting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)


