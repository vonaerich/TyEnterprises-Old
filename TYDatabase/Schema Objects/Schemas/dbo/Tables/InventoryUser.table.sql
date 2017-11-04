CREATE TABLE [dbo].[InventoryUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Password] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Firstname] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Lastname] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsAdmin] [bit] NULL,
	[IsApprover] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[Theme] [bit] NULL,
	[IsVisitor] [bit] NULL,
 CONSTRAINT [PK_InventoryUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)


