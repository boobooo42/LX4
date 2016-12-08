-- Lexical Analyzer Database Schema
-- Version: 1

-- Metadata Tables
CREATE TABLE la.Info(
    Version integer NOT NULL
    );

-- Corpus Tables
CREATE TABLE la.Corpus (
    Id bigint NOT NULL IDENTITY,
    Name varchar(2048) NOT NULL,
    Description text NOT NULL,
    Locked bit DEFAULT '0',
    Hash char(64),
    CONSTRAINT PK_Corpus PRIMARY KEY (Id)
    );

CREATE TABLE la.CorpusContent (
    Id bigint NOT NULL IDENTITY,
    CorpusId bigint NOT NULL,
    Hash char(64) NOT NULL,
    Name varchar(2048) NOT NULL,
    Type varchar(64) NOT NULL,
    ScraperGuid varchar(2048) NULL,
    ScraperType varchar(2048) NULL,
    DownloadDate datetime2 NULL,
    DownloadURL varchar(2048) NULL,
	Long float NULL,
	Lat float NULL,
    CONSTRAINT PK_CorpusContent PRIMARY KEY (Id)
    );

-- Merkle Tree Tables
CREATE TABLE la.MerkleNode(
    Hash char(64) NOT NULL,
    Type varchar(50) NOT NULL,
    Pinned bit NOT NULL,
    CONSTRAINT PK_MerkleNode PRIMARY KEY (Hash)
    );

CREATE TABLE la.MerkleEdge(
    ParentHash char(64) NOT NULL,
    ChildHash char(64) NOT NULL,
    CONSTRAINT PK_MerkleEdge PRIMARY KEY (ParentHash, ChildHash)
    );

CREATE TABLE la.ContentBlob(
    Hash char(64) NOT NULL,
    Contents nvarchar(max) NOT NULL,
    CONSTRAINT PK_ContentHash PRIMARY KEY (Hash)
    );

CREATE TABLE la.CorpusBlob(
    Hash char(64) NOT NULL,
    CONSTRAINT PK_CorpusBlob PRIMARY KEY (Hash)
    );

CREATE TABLE la.NeuralNetBlob(
    Hash char(64) NOT NULL,
    CachedNeuralNet text NOT NULL,
    Status varchar(64) NOT NULL,
    DateRequested datetime NOT NULL,
    DateCompleted datetime NOT NULL,
    CONSTRAINT PK_NeuralNetBlob PRIMARY KEY (Hash)
    );

CREATE TABLE la.NeuralNetParameterBlob(
    Hash char(64) NOT NULL,
    Name varchar(512) NOT NULL,
    Value text NOT NULL,
    CONSTRAINT PK_NeuralNetParameterBlob PRIMARY KEY (Hash)
    );

CREATE TABLE la.ResultsBlob(
    Hash char(64) NOT NULL,
    Contents text NULL,
    CONSTRAINT PK_ResultsBlob PRIMARY KEY (Hash)
    );

CREATE TABLE la.LearningModelBlob(
    Hash char(64) NOT NULL,
    Version varchar(64) NOT NULL,
    CONSTRAINT PK_BackendLibraryBlob PRIMARY KEY (Hash)
    );


/*
-- TODO: Define a macro for maximum filename/url length
CREATE TABLE la.[File](
    FileID bigint NOT NULL,
    FileHash char(64) NOT NULL,
    FileName varchar(2048) NOT NULL,
    DownloadDate timestamp NULL,
    DownloadURL varchar(2048) NULL,
    CONSTRAINT [PK_File] PRIMARY KEY (FileID)
    )
*/


/* TODO: Enable this in the future, as this should speed things up
 * significantly */
/*
CREATE NONCLUSTERED INDEX [IX_MerkleEdge] ON [dbo].[MerkleEdge]
(
	[ParentHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
*/
