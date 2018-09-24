using System;
using System.Collections.Generic;
using System.Reflection;
using ActivityInfo.Query;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace ActivityInfo
{
    public class Client
    {

        private RestClient client;
        private Random random = new Random();
        private Newtonsoft.Json.JsonSerializer jsonSerializer;
        private RestSharp.Serializers.ISerializer restSerializer;

        public Client(string email, String password)
        {
            client = new RestClient();
            client.BaseUrl = new Uri("https://www.activityinfo.org/resources");
            client.Authenticator = new HttpBasicAuthenticator(email, password);
            client.AddHandler("application/json", RestSharp.Serializers.Newtonsoft.Json.NewtonsoftJsonSerializer.Default);

            jsonSerializer = new Newtonsoft.Json.JsonSerializer();
            restSerializer = new RestSharp.Serializers.Newtonsoft.Json.NewtonsoftJsonSerializer(jsonSerializer);
        }

        public List<Partner> QueryPartners(int databaseId) 
        {
            var formId = string.Format("P{0:D10}", databaseId);
            return Query<Partner>(formId);
        }

        public List<T> Query<T>(string formId) where T : new()
        {
            QueryMapper<T> mapper = new QueryMapper<T>(formId);
            ColumnSet columnSet = Query(mapper.QueryModel);
            return mapper.Map(columnSet);
        }

        public ColumnSet Query(QueryModel query) {
            var request = new RestRequest();
            request.Resource = "query/columns";
            request.JsonSerializer = restSerializer;
            request.Method = Method.POST;
            request.AddJsonBody(query);
            request.AddHeader("Accept", "application/json");
            var response = client.Execute<ColumnSet>(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new Exception(String.Format("Query failed with status code {0}: {1}", response.StatusCode, response.Content));
            }
            return response.Data;
        }

        private string generateId()
        {
            return String.Format("s{0:D10}", random.Next());
        }

        public T QueryRecord<T>(RecordRef recordRef) where T : BaseRecord, new() {
            var request = new RestRequest();
            request.Resource = String.Format("form/{0}/record/{1}", recordRef.FormId, recordRef.RecordId);
            request.JsonSerializer = restSerializer;

            var response = client.Execute<JObject>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(string.Format("Request failed: {0}", response.StatusCode));
            }

            T record = new T();
            record.RecordId = response.Data["recordId"].ToString();
            record.ReadFields(response.Data);

            return record;
        }

        /// <summary>
        /// Creates a new record
        /// </summary>
        /// After this method returns, the RecordId property of the record 
        /// parameter will be set.
        /// <returns>The record.</returns>
        /// <param name="record">The record to create</param>
        /// <typeparam name="T">The type of the record</typeparam>
        public RecordRef CreateRecord<T>(T record) where T : BaseRecord {
            Transaction tx = new Transaction();
            record.RecordId = generateId();

            tx.AddChange(new RecordUpdate(record));

            ExecuteUpdate(tx);

            return record.Ref;
        }

        /// <summary>
        /// Updates an existing record
        /// </summary>
        /// <param name="record">Record.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void UpdateRecord<T>(T record) where T : BaseRecord {
            Transaction tx = new Transaction();
            tx.AddChange(new RecordUpdate(record));

            ExecuteUpdate(tx);
        }

        public void ExecuteUpdate(Transaction tx) 
        {
            var request = new RestRequest();
            request.Resource = "update";
            request.Method = Method.POST;
            request.JsonSerializer = new RestSharp.Serializers.Newtonsoft.Json.NewtonsoftJsonSerializer();
            request.AddJsonBody(tx);

            var response = client.Execute(request);
            if(response.StatusCode == System.Net.HttpStatusCode.BadRequest) {
                throw new Exception(string.Format("Invalid update: {0}", response.Content));

            } else if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new Exception(string.Format("Request failed: {0}", response.StatusCode));
            }
        }
    }
}
