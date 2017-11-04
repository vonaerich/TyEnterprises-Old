CREATE TABLE [dbo].[AutoPartAltPartNumber](
	[DetailId] [int] NOT NULL,
	[AltPartNumber] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_AutoPartAltPartNumber] PRIMARY KEY CLUSTERED 
(
	[DetailId] ASC,
	[AltPartNumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_AutoPartAltPartNumber_AutoPartDetail] FOREIGN KEY([DetailId])
REFERENCES [dbo].[AutoPartDetail] ([Id])
)


