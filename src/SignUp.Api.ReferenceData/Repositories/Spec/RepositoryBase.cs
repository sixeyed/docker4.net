using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace SignUp.Api.ReferenceData.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T>
    { 
        protected readonly IConfiguration _configuration;
        protected readonly ILogger _logger;

        private readonly Counter _queryCounter;
        private readonly string _metricsSource;

        protected abstract string GetAllSqlQuery { get; }

        public RepositoryBase(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;

            if (_configuration.GetValue<bool>("Metrics:Application:Enabled"))
            {
                _queryCounter = Metrics.CreateCounter("sql_queries", "SQL queries", "source", "status");
                _metricsSource = GetType().Name;
            }
        }

        public IDbConnection Connection
        {
            get
            {
                var connectionString = _configuration.GetConnectionString("SignUpDb");
                return new SqlConnection(connectionString);
            }
        }

        public IEnumerable<T> GetAll()
        {
            _logger.LogDebug("GetAll - executing SQL query: '{0}'", GetAllSqlQuery);
            if (_queryCounter != null)
            {
                _queryCounter.Labels(_metricsSource, "processed").Inc();
            }
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    var response = dbConnection.Query<T>(GetAllSqlQuery);
                    return response;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("GetAll - FAILED, ex: {ex}");
                if (_queryCounter != null)
                {
                    _queryCounter.Labels(_metricsSource, "failed").Inc();
                }
                return new List<T>();
            }
        }
    }
}
