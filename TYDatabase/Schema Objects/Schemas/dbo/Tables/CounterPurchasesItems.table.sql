CREATE TABLE [dbo].[CounterPurchasesItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CounterPurchasesId] [int] NULL,
	[PurchaseId] [int] NULL,
	[PurchaseReturnDetailId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
 CONSTRAINT [PK_CounterPurchasesItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_CounterPurchasesItems_CounterPurchases] FOREIGN KEY([CounterPurchasesId])
REFERENCES [dbo].[CounterPurchases] ([Id]),
 CONSTRAINT [FK_CounterPurchasesItems_Purchase] FOREIGN KEY([PurchaseId])
REFERENCES [dbo].[Purchase] ([Id]),
 CONSTRAINT [FK_CounterPurchasesItems_PurchaseReturnDetail] FOREIGN KEY([PurchaseReturnDetailId])
REFERENCES [dbo].[PurchaseReturnDetail] ([Id])
)


