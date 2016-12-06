using LexicalAnalyzer.Models;
using LexicalAnalyzer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Interfaces
{
    public interface IScraper : ITask, IGuid, IStop
    {
        /* NOTE: All IScraper implementors must implement these as static
         * properties */
        /*
        static string DisplayName {
            get;
        }
        static string Description {
            get;
        }
        static string ContentType {
            get;
        }
        static IEnumerable<KeyValueProperty> DefaultProperties {
            get;
        }
        */
        string UserGivenName { get; set; }
        string TypeName { get; }
        string Desc { get; }
        string Type
        {
            get;
        }

        IEnumerable<KeyValueProperty> Properties {
            get; set;
        }
    }
}
