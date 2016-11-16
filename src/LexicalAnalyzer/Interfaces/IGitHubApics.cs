using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestEase;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;

namespace LexicalAnalyzer.Interfaces
{
    public interface IGitHubApi
    {
        // All interface methods must return a Task or Task<T>. We'll discuss what sort of T in more detail below.

        // The [Get] attribute marks this method as a GET request
        // The "users" is a relative path the a base URL, which we'll provide later
        [Get("repositories")]
        Task<List<Repos>> GetReposAsync();
    }
}
