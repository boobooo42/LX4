INSERT INTO la.Corpus ( Id, Name, Description, Locked )
    VALUES ( '1',
        N'Emily Dickinson Poetry',
        N'A small sample of Emily Dickinson''s work copied from Wikisource.',
        '0' );

INSERT INTO la.CorpusContent ( Id, CorpusId, Hash, Name, Type )
    VALUES ( 
        '1',
        '1',
        N'02abfb2131a43a79f803e38912621bf266e4e5b2284ab7c348441f21d7da87e6',
        N'Much madness is divinest sense',
        N'poem' );
INSERT INTO la.MerkleNode ( Hash, Type, Pinned )
    VALUES (
        N'02abfb2131a43a79f803e38912621bf266e4e5b2284ab7c348441f21d7da87e6',
        N'ContentBlob',
        '0' );
INSERT INTO la.ContentBlob ( Hash, Contents )
    VALUES (
        N'02abfb2131a43a79f803e38912621bf266e4e5b2284ab7c348441f21d7da87e6',
        N'Much madness is divinest sense\nTo a discerning eye\nMuch sense the starkest madness.\n''T is the majority\nIn this, as all, prevails.\nAssent, and you are sane \nDemur, --- you''re straightway dangerous,\nAnd handled with a chain.'
        );
