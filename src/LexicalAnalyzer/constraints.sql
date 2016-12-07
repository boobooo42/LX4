/* Foreign key constraints */
ALTER TABLE la.CorpusContent
    WITH CHECK ADD CONSTRAINT FK_CorpusContent_Corpus
        FOREIGN KEY (CorpusId)
        REFERENCES la.Corpus (Id)
		ON UPDATE CASCADE
		ON DELETE CASCADE;
ALTER TABLE la.CorpusContent
    CHECK CONSTRAINT FK_CorpusContent_Corpus;


ALTER TABLE la.LearningModelBlob
    WITH CHECK ADD CONSTRAINT FK_LearningModelBlob_MerkleNode
        FOREIGN KEY (Hash)
        REFERENCES la.MerkleNode (Hash)
		ON UPDATE CASCADE
		ON DELETE CASCADE;
ALTER TABLE la.LearningModelBlob
    CHECK CONSTRAINT FK_LearningModelBlob_MerkleNode;

ALTER TABLE la.CorpusBlob
    WITH CHECK ADD CONSTRAINT FK_CorpusBlob_MerkleNode
        FOREIGN KEY (Hash)
        REFERENCES la.MerkleNode (Hash)
				ON UPDATE CASCADE
		ON DELETE CASCADE;
ALTER TABLE la.CorpusBlob
    CHECK CONSTRAINT FK_CorpusBlob_MerkleNode;

ALTER TABLE la.ContentBlob
    WITH CHECK ADD CONSTRAINT FK_ContentBlob_MerkleNode
        FOREIGN KEY (Hash)
        REFERENCES la.MerkleNode (Hash)
				ON UPDATE CASCADE
		ON DELETE CASCADE;
ALTER TABLE la.ContentBlob
    CHECK CONSTRAINT [FK_ContentBlob_MerkleNode];

ALTER TABLE la.MerkleEdge
    WITH CHECK ADD CONSTRAINT FK_MerkleNode_MerkleEdge_Parent
        FOREIGN KEY (ParentHash)
        REFERENCES la.MerkleNode (Hash)
		ON UPDATE NO ACTION
		ON DELETE NO ACTION;
ALTER TABLE la.[MerkleEdge]
    CHECK CONSTRAINT FK_MerkleNode_MerkleEdge_Parent;

ALTER TABLE la.MerkleEdge
    WITH CHECK ADD CONSTRAINT FK_MerkleNode_MerkleEdge_Child
        FOREIGN KEY (ChildHash)
        REFERENCES la.MerkleNode (Hash)
				ON UPDATE NO ACTION
		ON DELETE NO ACTION;
ALTER TABLE la.[MerkleEdge]
    CHECK CONSTRAINT FK_MerkleNode_MerkleEdge_Child;

ALTER TABLE la.NeuralNetBlob
    WITH CHECK ADD CONSTRAINT FK_NeuralNetBlob_MerkleNode
        FOREIGN KEY (Hash)
        REFERENCES la.MerkleNode (Hash)
		ON UPDATE CASCADE
		ON DELETE CASCADE;
ALTER TABLE la.NeuralNetBlob
    CHECK CONSTRAINT FK_NeuralNetBlob_MerkleNode;

ALTER TABLE la.NeuralNetParameterBlob
    WITH CHECK ADD CONSTRAINT FK_NeuralNetParameterBlob_MerkleNode
    FOREIGN KEY (Hash)
    REFERENCES la.MerkleNode (Hash)
	ON UPDATE CASCADE
	ON DELETE CASCADE;
ALTER TABLE la.NeuralNetParameterBlob
    CHECK CONSTRAINT FK_NeuralNetParameterBlob_MerkleNode;

ALTER TABLE la.ResultsBlob
    WITH CHECK ADD CONSTRAINT FK_ResultsBlob_MerkleNode
    FOREIGN KEY (Hash)
    REFERENCES la.MerkleNode (Hash)
	ON UPDATE CASCADE
	ON DELETE CASCADE;
ALTER TABLE la.ResultsBlob
    CHECK CONSTRAINT FK_ResultsBlob_MerkleNode;
