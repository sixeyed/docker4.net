using Microsoft.Extensions.Configuration;
using RestSharp;
using SignUp.Entities;
using System.Collections.Generic;

namespace SignUp.Web.ReferenceData
{
    public class ApiReferenceDataLoader : IReferenceDataLoader
    {
        private readonly IConfiguration _config;

        public ApiReferenceDataLoader(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<Country> GetCountries()
        {
            var client = new RestClient(_config["ReferenceDataApi:Url"]);
            var request = new RestRequest("countries");
            var response = client.Execute<List<Country>>(request);
            return response.Data;
        }        

        public IEnumerable<Role> GetRoles()
        {
            var client = new RestClient(_config["ReferenceDataApi:Url"]);
            var request = new RestRequest("roles");
            var response = client.Execute<List<Role>>(request);
            return response.Data;
        }
    }
}