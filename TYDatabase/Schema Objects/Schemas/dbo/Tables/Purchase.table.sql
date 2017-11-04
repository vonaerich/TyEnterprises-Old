CREATE TABLE [dbo].[Purchase](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NULL,
	[PONumber] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PurchaseDate] [datetime] NULL,
	[Type] [int] NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[RecordedBy] [int] NULL,
	[ApprovedBy] [int] NULL,
	[IsPaid] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[VatableSale] [decimal](18, 2) NULL,
	[Vat] [decimal](18, 2) NULL,
	[Comment] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PurchaseOption] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[InvoiceDiscount] [decimal](18, 2) NULL,
	[InvoiceDiscountPercent] [decimal](18, 2) NULL,
	[Balance] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Purchase] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_Purchase_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id]),
 CONSTRAINT [FK_Purchase_InventoryUser] FOREIGN KEY([RecordedBy])
REFERENCES [dbo].[InventoryUser] ([Id]),
 CONSTRAINT [FK_Purchase_InventoryUser1] FOREIGN KEY([ApprovedBy])
REFERENCES [dbo].[InventoryUser] ([Id])
)


