CREATE TABLE [dbo].[SalesPayment](
	[SalesId] [int] NULL,
	[PaymentId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SalesReturnDetailId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
 CONSTRAINT [PK_SalesPayment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_SalesPayment_PaymentDetail] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[PaymentDetail] ([Id]),
 CONSTRAINT [FK_SalesPayment_Sale] FOREIGN KEY([SalesId])
REFERENCES [dbo].[Sale] ([Id]),
 CONSTRAINT [FK_SalesPayment_SalesReturnDetail] FOREIGN KEY([SalesReturnDetailId])
REFERENCES [dbo].[SalesReturnDetail] ([Id])
)


