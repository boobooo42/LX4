﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer
{
    public interface IFileRepository
    {
        List<File> GetAll();
        void setHash(File file, string result);
        void insertFile(File file);
    }
}
