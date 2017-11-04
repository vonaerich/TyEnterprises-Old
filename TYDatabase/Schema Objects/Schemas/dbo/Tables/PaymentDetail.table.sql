CREATE TABLE [dbo].[PaymentDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VoucherNumber] [varchar](70) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PaymentDate] [datetime] NULL,
	[TotalCashPayment] [decimal](18, 2) NULL,
	[TotalCheckPayment] [decimal](18, 2) NULL,
	[RecordedBy] [int] NULL,
	[ApprovedBy] [int] NULL,
	[IsDeleted] [bit] NULL,
	[WitholdingTax] [decimal](18, 2) NULL,
	[GovtForm] [bit] NULL,
	[TotalAmountDue] [decimal](18, 2) NULL,
	[Discount] [decimal](18, 2) NULL,
	[MiscAmount] [decimal](18, 2) NULL,
	[Misc] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CustomerId] [int] NULL,
	[Remarks] [varchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_CustomerPayment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_PaymentDetail_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id]),
 CONSTRAINT [FK_PaymentDetail_InventoryUser] FOREIGN KEY([RecordedBy])
REFERENCES [dbo].[InventoryUser] ([Id]),
 CONSTRAINT [FK_PaymentDetail_InventoryUser1] FOREIGN KEY([ApprovedBy])
REFERENCES [dbo].[InventoryUser] ([Id])
)


