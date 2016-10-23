-- Lexical Analyzer Database Schema
-- Version: 1

CREATE SCHEMA la;

-- Metadata Tables
CREATE TABLE la.Info(
    Version integer NOT NULL
    );

-- Merkle Tree Tables
CREATE TABLE la.MerkleNode(
    Hash char(32) NOT NULL,
    Type varchar(50) NOT NULL,
    Pinned bit NOT NULL,
    CONSTRAINT PK_MerkleNode PRIMARY KEY (Hash)
    );

CREATE TABLE la.MerkleEdge(
    ParentHash char(32) NOT NULL,
    ChildHash char(32) NOT NULL,
    CONSTRAINT PK_MerkleEdge PRIMARY KEY (ParentHash, ChildHash)
    );

CREATE TABLE la.ContentBlob(
    Hash char(32) NOT NULL,
    Contents text NOT NULL,
    CONSTRAINT PK_ContentHash PRIMARY KEY (Hash)
    );

CREATE TABLE la.CorpusBlob(
    Hash char(32) NOT NULL,
    CONSTRAINT PK_CorpusBlob PRIMARY KEY (Hash)
    );

CREATE TABLE la.NeuralNetBlob(
    Hash char(32) NOT NULL,
    CachedNeuralNet text NOT NULL,
    Status varchar(64) NOT NULL,
    DateRequested datetime NOT NULL,
    DateCompleted datetime NOT NULL,
    CONSTRAINT PK_NeuralNetBlob PRIMARY KEY (Hash)
    );

CREATE TABLE la.NeuralNetParameterBlob(
    Hash char(32) NOT NULL,
    Name varchar(512) NOT NULL,
    Value text NOT NULL,
    CONSTRAINT PK_NeuralNetParameterBlob PRIMARY KEY (Hash)
    );

CREATE TABLE la.ResultsBlob(
    Hash char(32) NOT NULL,
    Contents text NULL,
    CONSTRAINT PK_ResultsBlob PRIMARY KEY (Hash)
    );

CREATE TABLE la.LearningModelBlob(
    Hash char(32) NOT NULL,
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


/* Foreign key constraints */
ALTER TABLE la.LearningModelBlob
    WITH CHECK ADD CONSTRAINT FK_LearningModelBlob_MerkleNode
        FOREIGN KEY (Hash)
        REFERENCES la.MerkleNode (Hash);
ALTER TABLE la.LearningModelBlob
    CHECK CONSTRAINT FK_LearningModelBlob_MerkleNode;

ALTER TABLE la.CorpusBlob
    WITH CHECK ADD CONSTRAINT FK_CorpusBlob_MerkleNode
        FOREIGN KEY (Hash)
        REFERENCES la.MerkleNode (Hash);
ALTER TABLE la.CorpusBlob
    CHECK CONSTRAINT FK_CorpusBlob_MerkleNode;

ALTER TABLE la.ContentBlob
    WITH CHECK ADD CONSTRAINT FK_ContentBlob_MerkleNode
        FOREIGN KEY (Hash)
        REFERENCES la.MerkleNode (Hash);
ALTER TABLE la.ContentBlob
    CHECK CONSTRAINT [FK_ContentBlob_MerkleNode];

ALTER TABLE la.MerkleEdge
    WITH CHECK ADD CONSTRAINT FK_MerkleNode_MerkleEdge_Parent
        FOREIGN KEY (ParentHash)
        REFERENCES la.MerkleNode (Hash);
ALTER TABLE la.[MerkleEdge]
    CHECK CONSTRAINT FK_MerkleNode_MerkleEdge_Parent;

ALTER TABLE la.MerkleEdge
    WITH CHECK ADD CONSTRAINT FK_MerkleNode_MerkleEdge_Child
        FOREIGN KEY (ChildHash)
        REFERENCES la.MerkleNode (Hash);
ALTER TABLE la.[MerkleEdge]
    CHECK CONSTRAINT FK_MerkleNode_MerkleEdge_Child;

ALTER TABLE la.NeuralNetBlob
    WITH CHECK ADD CONSTRAINT FK_NeuralNetBlob_MerkleNode
        FOREIGN KEY (Hash)
        REFERENCES la.MerkleNode (Hash);
ALTER TABLE la.NeuralNetBlob
    CHECK CONSTRAINT FK_NeuralNetBlob_MerkleNode;

ALTER TABLE la.NeuralNetParameterBlob
    WITH CHECK ADD CONSTRAINT FK_NeuralNetParameterBlob_MerkleNode
    FOREIGN KEY (Hash)
    REFERENCES la.MerkleNode (Hash);
ALTER TABLE la.NeuralNetParameterBlob
    CHECK CONSTRAINT FK_NeuralNetParameterBlob_MerkleNode;

ALTER TABLE la.ResultsBlob
    WITH CHECK ADD CONSTRAINT FK_ResultsBlob_MerkleNode
    FOREIGN KEY (Hash)
    REFERENCES la.MerkleNode (Hash);
ALTER TABLE la.ResultsBlob
    CHECK CONSTRAINT FK_ResultsBlob_MerkleNode;
