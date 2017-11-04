CREATE TABLE [dbo].[PurchasePayment](
	[PurchaseId] [int] NULL,
	[PaymentId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseReturnDetailId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
 CONSTRAINT [PK_PurchasePayment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_PurchasePayment_PaymentDetail] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[PaymentDetail] ([Id]),
 CONSTRAINT [FK_PurchasePayment_Purchase] FOREIGN KEY([PurchaseId])
REFERENCES [dbo].[Purchase] ([Id]),
 CONSTRAINT [FK_PurchasePayment_PurchaseReturnDetail] FOREIGN KEY([PurchaseReturnDetailId])
REFERENCES [dbo].[PurchaseReturnDetail] ([Id])
)


