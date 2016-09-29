using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LexicalAnalyzer.Resources
{
    public class HtmlDocumentTree
    {
        private HtmlDocument node = null;
        private string url = "";
        private List<HtmlDocumentTree> childDocuments = new List<HtmlDocumentTree>();

        public HtmlDocument Node
        {
            get
            {
                return node;
            }

            set
            {
                node = value;
            }
        }

        public List<HtmlDocumentTree> ChildDocuments
        {
            get
            {
                return childDocuments;
            }

            set
            {
                childDocuments = value;
            }
        }

        public string Url
        {
            get
            {
                return url;
            }

            set
            {
                url = value;
            }
        }

        public HtmlDocumentTree(HtmlDocument _node, string _url)
        {
            Node = _node;
            Url = _url;
        }

    }
}
