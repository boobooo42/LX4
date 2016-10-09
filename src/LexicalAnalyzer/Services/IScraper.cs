﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Services
{
    public interface IScraper : ITask, IGuid
    {
        string DisplayName {
            get;
        }
        string Description {
            get;
        }
        string ContentType {
            get;
        }
    }
}