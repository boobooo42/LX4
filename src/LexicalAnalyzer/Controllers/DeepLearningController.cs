using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LexicalAnalyzer.Controllers
{
    public class DeepLearningController : Controller
    {
        [HttpGet("api/deeplearning/nets")]
        public IEnumerable<string> Nets()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("api/deeplearning/neuralnet/status")]
        public IEnumerable<string> NeuralNetStatus()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
