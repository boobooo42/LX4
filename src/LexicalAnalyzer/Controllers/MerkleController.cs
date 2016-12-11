using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Controllers
{
    public class MerkleController : Controller
    {
        /* Private members */
        IMerkleTreeContext m_context;

        public MerkleController(IMerkleTreeContext context) {
            m_context = context;
        }

        [HttpGet("api/merkle/nodes")]
        public IEnumerable<MerkleNode> GetNodes() {
            IEnumerable<MerkleNode> list =
                m_context.MerkleNodeRepository.List();
            return list;
        }

        /// <summary>
        /// Access a Merkle node with the given Merkle hash.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Use this call to get a specific Merkle node with a given hash.
        /// </p>
        /// <p>
        /// The hash is a base 16 encoded SHA256 string.
        /// </p>
        /// </remarks>
        [HttpGet("api/merkle/node/{hash}")]
        public MerkleNode GetNode(string hash)
        {
            /* TODO: I need to implement a generic MerkleNodeRepository */
            /*
            MerkleNode node =
                m_context.MerkleNodeRepository.GetByHash(
                    new MerkleHash(hash));
            return node;
            */
            return default(MerkleNode);
        }

        [HttpGet("api/merkle/tree")]
        public string GetTree() {
            /* Get a list of all pinned Merkle nodes (we wouldn't want to show
             * trees for every Merkle node) */
            var pinnedNodes = m_context.MerkleNodeRepository.ListPinned();
            return JsonConvert.SerializeObject(pinnedNodes);
        }

        [HttpGet("api/merkle/tree/{hash}")]
        public string GetTree(string hash) {
            /* Look for a Merkle node with the given hsah */
            MerkleNode root = m_context.MerkleNodeRepository.GetByHash(hash);
            if (root == null) {
                Response.StatusCode = 404;  /* Not Found */
                var error = new LexicalAnalyzer.Models.Error();
                error.Message =
                    "Could not find Merkle node with the given hash";
                return JsonConvert.SerializeObject(error);
            }
            return JsonConvert.SerializeObject(root);
        }
    }
}
