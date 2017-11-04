CREATE TABLE [dbo].[PurchaseRequisition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SaleId] [int] NULL,
	[PurchaseId] [int] NULL,
	[PRNumber] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_PurchaseRequisition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_PurchaseRequisition_Purchase] FOREIGN KEY([PurchaseId])
REFERENCES [dbo].[Purchase] ([Id]),
 CONSTRAINT [FK_PurchaseRequisition_Sale] FOREIGN KEY([SaleId])
REFERENCES [dbo].[Sale] ([Id])
)


