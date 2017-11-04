CREATE TABLE [dbo].[Check](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CheckNumber] [varchar](70) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Bank] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Branch] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Amount] [decimal](18, 2) NULL,
	[CheckDate] [datetime] NULL,
	[ClearingDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Check] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)


