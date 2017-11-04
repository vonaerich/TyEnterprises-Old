CREATE TABLE [dbo].[PaymentCheck](
	[PaymentId] [int] NOT NULL,
	[CheckId] [int] NOT NULL,
 CONSTRAINT [PK_PaymentCheck] PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC,
	[CheckId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON),
 CONSTRAINT [FK_PaymentCheck_Check] FOREIGN KEY([CheckId])
REFERENCES [dbo].[Check] ([Id]),
 CONSTRAINT [FK_PaymentCheck_PaymentDetail] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[PaymentDetail] ([Id])
)


