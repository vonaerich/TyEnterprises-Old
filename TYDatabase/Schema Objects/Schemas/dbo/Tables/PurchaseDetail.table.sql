CREATE TABLE [dbo].[PurchaseDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseId] [int] NULL,
	[PartDetailId] [int] NULL,
	[Quantity] [int] NULL,
	[Unit] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Price] [decimal](18, 2) NULL,
	[DiscountPercent] [decimal](18, 2) NULL,
	[DiscountPercent2] [decimal](18, 2) NULL,
	[DiscountPercent3] [decimal](18, 2) NULL,
	[TotalDiscount] [decimal](18, 2) NULL,
	[TotalAmount] [decimal](18, 2) NULL,
 CONSTRAINT [PK_PurchaseDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_PurchaseDetail_AutoPartDetail] FOREIGN KEY([PartDetailId])
REFERENCES [dbo].[AutoPartDetail] ([Id]),
 CONSTRAINT [FK_PurchaseDetail_Purchase] FOREIGN KEY([PurchaseId])
REFERENCES [dbo].[Purchase] ([Id])
)


