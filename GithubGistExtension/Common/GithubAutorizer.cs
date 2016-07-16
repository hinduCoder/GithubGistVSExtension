using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using RestSharp;

namespace GithubGistExtension.Common
{
    public class GithubAutorizer
    {
        private string _clientId = "f12075cc0610132510bf";
        private string _clientSecret = "d8352b2eb67171a848893ce40f0053d22f5ab6f4";

        public GithubAutorizer()
        {
            

        }

        public async Task<string> MakeAuthorization()
        {
            throw new NotImplementedException();
        }
    }
}