using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestEase;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;

namespace LexicalAnalyzer.Interfaces
{
    [Header("User-Agent", "RestEase")]
    public interface IGitHubApi
    {

        // The [Get] attribute marks this method as a GET request
        // The "users" is a relative path the a base URL, which we'll provide later
        [Get("repositories")]
        Task<Response<List<Repos>>> GetReposAsync();

        [Get("tags")]
        Task<Response<List<Tags>>> GetTagsAsync();

        [Get("zipball_url")]
        Task<Response<byte[]>> GetByteAsync();
    }
}
