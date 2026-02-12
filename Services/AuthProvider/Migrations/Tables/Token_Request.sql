/* ************************************************************* */
/* ******** Create the table to store the Token Request ******** */
/* ************************************************************* */
CREATE TABLE [dbo].[Token_Request](
	[Token_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Expires_DT] [datetime] NOT NULL,
	[Temporal_TT] [datetime] NOT NULL,
PRIMARY KEY ( [Token_ID] ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- This sets when the token expires in the future.
ALTER TABLE [dbo].[Token_Request] ADD  CONSTRAINT [DF_TOKEN_REQUEST_EXPIRES_DT]  DEFAULT (DATEADD (hh, 8, GETDATE())) FOR [Expires_DT]
GO

-- Set the temporal token expiration in the past, it has to be touched to become active.
ALTER TABLE [dbo].[Token_Request] ADD  CONSTRAINT [DF_TOKEN_REQUEST_TEMPORAL_TT]  DEFAULT (getdate() - 30) FOR [Temporal_TT]
GO