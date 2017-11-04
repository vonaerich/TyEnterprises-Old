CREATE TABLE [dbo].[CounterSales](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CounterNumber] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Date] [datetime] NULL,
	[Remarks] [varchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Total] [decimal](18, 2) NULL,
	[Discount] [decimal](18, 2) NULL,
	[WitholdingTax] [decimal](18, 2) NULL,
	[IsDeleted] [bit] NULL,
	[CustomerId] [int] NULL,
 CONSTRAINT [PK_CounterSales] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_CounterSales_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
)


