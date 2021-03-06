USE [master]
GO
/****** Object:  User [NT SERVICE\ReportServer]    Script Date: 10/14/2016 8:34:53 AM ******/
CREATE USER [NT SERVICE\ReportServer] FOR LOGIN [NT SERVICE\ReportServer] WITH DEFAULT_SCHEMA=[NT SERVICE\ReportServer]
GO
/****** Object:  User [##MS_PolicyEventProcessingLogin##]    Script Date: 10/14/2016 8:34:53 AM ******/
CREATE USER [##MS_PolicyEventProcessingLogin##] FOR LOGIN [##MS_PolicyEventProcessingLogin##] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [##MS_AgentSigningCertificate##]    Script Date: 10/14/2016 8:34:53 AM ******/
CREATE USER [##MS_AgentSigningCertificate##] FOR LOGIN [##MS_AgentSigningCertificate##]
GO
/****** Object:  DatabaseRole [RSExecRole]    Script Date: 10/14/2016 8:34:53 AM ******/
CREATE ROLE [RSExecRole]
GO
ALTER ROLE [RSExecRole] ADD MEMBER [NT SERVICE\ReportServer]
GO
/****** Object:  Schema [NT SERVICE\ReportServer]    Script Date: 10/14/2016 8:34:53 AM ******/
CREATE SCHEMA [NT SERVICE\ReportServer]
GO
/****** Object:  Schema [RSExecRole]    Script Date: 10/14/2016 8:34:53 AM ******/
CREATE SCHEMA [RSExecRole]
GO
/****** Object:  Table [dbo].[BackendLibraryBlob]    Script Date: 10/14/2016 8:34:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BackendLibraryBlob](
	[BackendLibraryHash] [char](64) NOT NULL,
	[Version] [varchar](64) NOT NULL,
 CONSTRAINT [PK_BackendLibraryBlob] PRIMARY KEY CLUSTERED 
(
	[BackendLibraryHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CorpusBlob]    Script Date: 10/14/2016 8:34:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CorpusBlob](
	[CorpusHash] [char](64) NOT NULL,
 CONSTRAINT [PK_CorpusBlob] PRIMARY KEY CLUSTERED 
(
	[CorpusHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[File]    Script Date: 10/14/2016 8:34:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[File](
	[FileID] [bigint] NOT NULL,
	[FileHash] [char](64) NOT NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[DownloadDate] [nvarchar](max) NULL,
	[DownloadURL] [nvarchar](max) NULL,
 CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FileBlob]    Script Date: 10/14/2016 8:34:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FileBlob](
	[FileHash] [char](64) NOT NULL,
	[Contents] [varchar](max) NOT NULL,
 CONSTRAINT [PK_FileBlob] PRIMARY KEY CLUSTERED 
(
	[FileHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MerkleEdge]    Script Date: 10/14/2016 8:34:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MerkleEdge](
	[ParentHash] [char](64) NOT NULL,
	[ChildHash] [char](64) NOT NULL,
 CONSTRAINT [PK_MerkleEdge] PRIMARY KEY CLUSTERED 
(
	[ParentHash] ASC,
	[ChildHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MerkleNode]    Script Date: 10/14/2016 8:34:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MerkleNode](
	[MerkleNodeHash] [char](64) NOT NULL,
	[Type] [varchar](50) NOT NULL,
	[Pinned] [bit] NOT NULL,
 CONSTRAINT [PK_MerkleNode] PRIMARY KEY CLUSTERED 
(
	[MerkleNodeHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NeuralNetBlob]    Script Date: 10/14/2016 8:34:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NeuralNetBlob](
	[NeuralNetHash] [char](64) NOT NULL,
	[CachedNeuralNet] [varchar](max) NOT NULL,
	[Status] [varchar](64) NOT NULL,
	[DateRequested] [datetime] NOT NULL,
	[DateCompleted] [datetime] NOT NULL,
 CONSTRAINT [PK_NeuralNetBlob] PRIMARY KEY CLUSTERED 
(
	[NeuralNetHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NeuralNetParameterBlob]    Script Date: 10/14/2016 8:34:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NeuralNetParameterBlob](
	[NNParameterHash] [char](64) NOT NULL,
	[Name] [varchar](512) NOT NULL,
	[Value] [varchar](max) NOT NULL,
 CONSTRAINT [PK_NeuralNetParameterBlob] PRIMARY KEY CLUSTERED 
(
	[NNParameterHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ResultsBlob]    Script Date: 10/14/2016 8:34:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ResultsBlob](
	[ResultsHash] [char](64) NOT NULL,
	[Contents] [varchar](max) NULL,
 CONSTRAINT [PK_ResultsBlob] PRIMARY KEY CLUSTERED 
(
	[ResultsHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_MerkleEdge]    Script Date: 10/14/2016 8:34:53 AM ******/
CREATE NONCLUSTERED INDEX [IX_MerkleEdge] ON [dbo].[MerkleEdge]
(
	[ParentHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BackendLibraryBlob]  WITH CHECK ADD  CONSTRAINT [FK_BackendLibraryBlob_MerkleNode] FOREIGN KEY([BackendLibraryHash])
REFERENCES [dbo].[MerkleNode] ([MerkleNodeHash])
GO
ALTER TABLE [dbo].[BackendLibraryBlob] CHECK CONSTRAINT [FK_BackendLibraryBlob_MerkleNode]
GO
ALTER TABLE [dbo].[CorpusBlob]  WITH CHECK ADD  CONSTRAINT [FK_CorpusBlob_MerkleNode] FOREIGN KEY([CorpusHash])
REFERENCES [dbo].[MerkleNode] ([MerkleNodeHash])
GO
ALTER TABLE [dbo].[CorpusBlob] CHECK CONSTRAINT [FK_CorpusBlob_MerkleNode]
GO
ALTER TABLE [dbo].[FileBlob]  WITH CHECK ADD  CONSTRAINT [FK_FileBlob_MerkleNode] FOREIGN KEY([FileHash])
REFERENCES [dbo].[MerkleNode] ([MerkleNodeHash])
GO
ALTER TABLE [dbo].[FileBlob] CHECK CONSTRAINT [FK_FileBlob_MerkleNode]
GO
ALTER TABLE [dbo].[MerkleEdge]  WITH CHECK ADD  CONSTRAINT [FK_MerkleNode_MerkleEdge] FOREIGN KEY([ParentHash])
REFERENCES [dbo].[MerkleNode] ([MerkleNodeHash])
GO
ALTER TABLE [dbo].[MerkleEdge] CHECK CONSTRAINT [FK_MerkleNode_MerkleEdge]
GO
ALTER TABLE [dbo].[NeuralNetBlob]  WITH CHECK ADD  CONSTRAINT [FK_NeuralNetBlob_MerkleNode] FOREIGN KEY([NeuralNetHash])
REFERENCES [dbo].[MerkleNode] ([MerkleNodeHash])
GO
ALTER TABLE [dbo].[NeuralNetBlob] CHECK CONSTRAINT [FK_NeuralNetBlob_MerkleNode]
GO
ALTER TABLE [dbo].[NeuralNetParameterBlob]  WITH CHECK ADD  CONSTRAINT [FK_NeuralNetParameterBlob_MerkleNode] FOREIGN KEY([NNParameterHash])
REFERENCES [dbo].[MerkleNode] ([MerkleNodeHash])
GO
ALTER TABLE [dbo].[NeuralNetParameterBlob] CHECK CONSTRAINT [FK_NeuralNetParameterBlob_MerkleNode]
GO
ALTER TABLE [dbo].[ResultsBlob]  WITH CHECK ADD  CONSTRAINT [FK_ResultsBlob_MerkleNode] FOREIGN KEY([ResultsHash])
REFERENCES [dbo].[MerkleNode] ([MerkleNodeHash])
GO
ALTER TABLE [dbo].[ResultsBlob] CHECK CONSTRAINT [FK_ResultsBlob_MerkleNode]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Inheritance' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BackendLibraryBlob', @level2type=N'CONSTRAINT',@level2name=N'FK_BackendLibraryBlob_MerkleNode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Inheritance' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CorpusBlob', @level2type=N'CONSTRAINT',@level2name=N'FK_CorpusBlob_MerkleNode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Inheritance' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FileBlob', @level2type=N'CONSTRAINT',@level2name=N'FK_FileBlob_MerkleNode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Inheritance' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NeuralNetBlob', @level2type=N'CONSTRAINT',@level2name=N'FK_NeuralNetBlob_MerkleNode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Inheritance' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NeuralNetParameterBlob', @level2type=N'CONSTRAINT',@level2name=N'FK_NeuralNetParameterBlob_MerkleNode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Inheritance' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ResultsBlob', @level2type=N'CONSTRAINT',@level2name=N'FK_ResultsBlob_MerkleNode'
GO
