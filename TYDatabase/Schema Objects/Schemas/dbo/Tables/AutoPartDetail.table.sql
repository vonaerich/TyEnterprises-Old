CREATE TABLE [dbo].[AutoPartDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AutoPartId] [int] NULL,
	[PartNumber] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AltPartNumber] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Model] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Make] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Size] [varchar](70) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[BrandId] [int] NULL,
	[Quantity] [int] NULL,
	[Unit] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SellingPrice1] [decimal](18, 2) NULL,
	[SellingPrice2] [decimal](18, 2) NULL,
	[BuyingPrice] [decimal](18, 2) NULL,
	[ReorderLimit] [int] NULL,
	[Picture] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsDeleted] [bit] NULL,
	[Description] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ReorderDate] [datetime] NULL,
 CONSTRAINT [PK_AutoPartDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_AutoPartDetail_AutoPart] FOREIGN KEY([AutoPartId])
REFERENCES [dbo].[AutoPart] ([Id]),
 CONSTRAINT [FK_AutoPartDetail_Brand] FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brand] ([Id])
)


