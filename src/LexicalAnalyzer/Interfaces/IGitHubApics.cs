using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestEase;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Net.Http;

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

        //[Header("client_id", "d5e39c35fd7972a88e6d")]
        //[Header("client_secret", "7e8c71c521d17b259739423514bc33793a801ebe")]
        [Post("authorizations")]
        Task<HttpResponseMessage> GetTokenAsync();
    }
}
