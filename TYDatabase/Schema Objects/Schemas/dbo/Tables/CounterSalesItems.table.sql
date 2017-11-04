CREATE TABLE [dbo].[CounterSalesItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CounterSalesId] [int] NULL,
	[SaleId] [int] NULL,
	[SalesReturnDetailId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
 CONSTRAINT [PK_CounterSalesItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_CounterSalesItems_CounterSales] FOREIGN KEY([CounterSalesId])
REFERENCES [dbo].[CounterSales] ([Id]),
 CONSTRAINT [FK_CounterSalesItems_Sale] FOREIGN KEY([SaleId])
REFERENCES [dbo].[Sale] ([Id]),
 CONSTRAINT [FK_CounterSalesItems_SalesReturnDetail] FOREIGN KEY([SalesReturnDetailId])
REFERENCES [dbo].[SalesReturnDetail] ([Id])
)


