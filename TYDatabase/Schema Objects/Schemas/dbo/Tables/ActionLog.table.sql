CREATE TABLE [dbo].[ActionLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Action] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ActionDate] [datetime] NULL,
	[ActionUserId] [int] NULL,
 CONSTRAINT [PK_ActionLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_ActionLog_InventoryUser] FOREIGN KEY([ActionUserId])
REFERENCES [dbo].[InventoryUser] ([Id])
)


