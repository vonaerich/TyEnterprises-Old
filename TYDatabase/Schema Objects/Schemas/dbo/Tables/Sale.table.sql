CREATE TABLE [dbo].[Sale](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NULL,
	[InvoiceNumber] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Date] [datetime] NULL,
	[Type] [int] NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[IsPaid] [bit] NULL,
	[RecordedBy] [int] NULL,
	[ApprovedBy] [int] NULL,
	[IsDeleted] [bit] NULL,
	[VatableSale] [decimal](18, 2) NULL,
	[Vat] [decimal](18, 2) NULL,
	[Comment] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[InvoiceDiscount] [decimal](18, 2) NULL,
	[InvoiceDiscountPercent] [decimal](18, 2) NULL,
	[Balance] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Sale] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_Sale_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id]),
 CONSTRAINT [FK_Sale_InventoryUser] FOREIGN KEY([RecordedBy])
REFERENCES [dbo].[InventoryUser] ([Id]),
 CONSTRAINT [FK_Sale_InventoryUser1] FOREIGN KEY([ApprovedBy])
REFERENCES [dbo].[InventoryUser] ([Id])
)


