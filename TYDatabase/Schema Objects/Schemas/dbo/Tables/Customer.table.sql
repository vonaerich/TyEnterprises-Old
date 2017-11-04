CREATE TABLE [dbo].[Customer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerCode] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CompanyName] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ContactPerson] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Address] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PhoneNumber] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FaxNumber] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TIN] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Agent] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PaymentTerms] [int] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)


