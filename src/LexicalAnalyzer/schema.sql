CREATE DATABASE lexical_analyzer;
GO
USE lexical_analyzer;
GO
CREATE SCHEMA la;
GO

-- FIXME: I'm not sure what this table is for
CREATE TABLE la.BackendLibraryBlob(
    BackendLibraryHash char(64) NOT NULL,
    Version varchar(64) NOT NULL,
    CONSTRAINT PK_BackendLibraryBlob PRIMARY KEY (BackendLibraryHash)
    );

CREATE TABLE la.CorpusBlob(
    CorpusHash char(64) NOT NULL,
    CONSTRAINT PK_CorpusBlob PRIMARY KEY (CorpusHash)
    );

-- TODO: Define a macro for maximum filename/url length
CREATE TABLE la.[File](
    FileID bigint NOT NULL,
    FileHash char(64) NOT NULL,
    FileName varchar(2048) NOT NULL,
    DownloadDate timestamp NULL,
    DownloadURL varchar(2048) NULL,
    CONSTRAINT [PK_File] PRIMARY KEY (FileID)
    );

CREATE TABLE la.FileBlob(
    FileHash char(64) NOT NULL,
    Contents varchar(max) NOT NULL,
    CONSTRAINT PK_FileBlob PRIMARY KEY (FileHash)
    );

CREATE TABLE la.MerkleEdge(
    ParentHash char(64) NOT NULL,
    ChildHash char(64) NOT NULL,
    CONSTRAINT PK_MerkleEdge PRIMARY KEY (ParentHash, ChildHash)
    );

CREATE TABLE la.MerkleNode(
    MerkleNodeHash char(64) NOT NULL,
    Type varchar(50) NOT NULL,
    Pinned bit NOT NULL,
    CONSTRAINT PK_MerkleNode PRIMARY KEY (MerkleNodeHash)
    );

CREATE TABLE la.NeuralNetBlob(
    NeuralNetHash char(64) NOT NULL,
    CachedNeuralNet varchar(max) NOT NULL,
    Status varchar(64) NOT NULL,
    DateRequested datetime NOT NULL,
    DateCompleted datetime NOT NULL,
    CONSTRAINT PK_NeuralNetBlob PRIMARY KEY (NeuralNetHash)
    );

CREATE TABLE la.NeuralNetParameterBlob(
    NNParameterHash char(64) NOT NULL,
    Name varchar(512) NOT NULL,
    Value varchar(max) NOT NULL,
    CONSTRAINT PK_NeuralNetParameterBlob PRIMARY KEY (NNParameterHash)
    );

CREATE TABLE la.ResultsBlob(
    ResultsHash char(64) NOT NULL,
    Contents varchar(max) NULL,
    CONSTRAINT PK_ResultsBlob PRIMARY KEY (ResultsHash)
    );


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
ALTER TABLE la.BackendLibraryBlob
    WITH CHECK ADD CONSTRAINT FK_BackendLibraryBlob_MerkleNode
        FOREIGN KEY (BackendLibraryHash)
        REFERENCES la.MerkleNode (MerkleNodeHash);
ALTER TABLE la.BackendLibraryBlob
    CHECK CONSTRAINT FK_BackendLibraryBlob_MerkleNode;

ALTER TABLE la.CorpusBlob
    WITH CHECK ADD CONSTRAINT FK_CorpusBlob_MerkleNode
        FOREIGN KEY (CorpusHash)
        REFERENCES la.MerkleNode (MerkleNodeHash);
ALTER TABLE la.CorpusBlob
    CHECK CONSTRAINT FK_CorpusBlob_MerkleNode;

ALTER TABLE la.FileBlob
    WITH CHECK ADD CONSTRAINT FK_FileBlob_MerkleNode
        FOREIGN KEY (FileHash)
        REFERENCES la.MerkleNode (MerkleNodeHash);
ALTER TABLE la.FileBlob
    CHECK CONSTRAINT [FK_FileBlob_MerkleNode];

ALTER TABLE la.MerkleEdge
    WITH CHECK ADD CONSTRAINT FK_MerkleNode_MerkleEdge
        FOREIGN KEY (ParentHash)
        REFERENCES la.MerkleNode (MerkleNodeHash);
ALTER TABLE la.[MerkleEdge]
    CHECK CONSTRAINT FK_MerkleNode_MerkleEdge;

ALTER TABLE la.NeuralNetBlob
    WITH CHECK ADD CONSTRAINT FK_NeuralNetBlob_MerkleNode
        FOREIGN KEY (NeuralNetHash)
        REFERENCES la.MerkleNode (MerkleNodeHash);
ALTER TABLE la.NeuralNetBlob
    CHECK CONSTRAINT FK_NeuralNetBlob_MerkleNode;

ALTER TABLE la.NeuralNetParameterBlob
    WITH CHECK ADD CONSTRAINT FK_NeuralNetParameterBlob_MerkleNode
    FOREIGN KEY (NNParameterHash)
    REFERENCES la.MerkleNode (MerkleNodeHash);
ALTER TABLE la.NeuralNetParameterBlob
    CHECK CONSTRAINT FK_NeuralNetParameterBlob_MerkleNode;

ALTER TABLE la.ResultsBlob
    WITH CHECK ADD CONSTRAINT FK_ResultsBlob_MerkleNode
    FOREIGN KEY (ResultsHash)
    REFERENCES la.MerkleNode (MerkleNodeHash);
ALTER TABLE la.ResultsBlob
    CHECK CONSTRAINT FK_ResultsBlob_MerkleNode;
