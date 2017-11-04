CREATE TABLE [dbo].[SalesReturnDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SRId] [int] NULL,
	[PartDetailId] [int] NULL,
	[Quantity] [int] NULL,
	[UnitPrice] [decimal](18, 2) NULL,
	[InvoiceNumber] [varchar](70) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[Balance] [decimal](18, 2) NULL,
 CONSTRAINT [PK_SalesReturnDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_SalesReturnDetail_AutoPartDetail] FOREIGN KEY([PartDetailId])
REFERENCES [dbo].[AutoPartDetail] ([Id]),
 CONSTRAINT [FK_SalesReturnDetail_SalesReturn] FOREIGN KEY([SRId])
REFERENCES [dbo].[SalesReturn] ([Id])
)


