CREATE TABLE [dbo].[SalesReturn](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MemoNumber] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CustomerId] [int] NULL,
	[ReturnDate] [datetime] NULL,
	[AmountReturn] [decimal](18, 2) NULL,
	[Adjustment] [decimal](18, 2) NULL,
	[TotalCreditAmount] [decimal](18, 2) NULL,
	[Remarks] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[RecordedBy] [int] NULL,
	[ApprovedBy] [int] NULL,
	[IsDeleted] [bit] NULL,
	[IsUsed] [bit] NULL,
	[AmountUsed] [decimal](18, 2) NULL,
 CONSTRAINT [PK_SalesReturn] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_SalesReturn_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id]),
 CONSTRAINT [FK_SalesReturn_InventoryUser] FOREIGN KEY([RecordedBy])
REFERENCES [dbo].[InventoryUser] ([Id]),
 CONSTRAINT [FK_SalesReturn_InventoryUser1] FOREIGN KEY([ApprovedBy])
REFERENCES [dbo].[InventoryUser] ([Id])
)


