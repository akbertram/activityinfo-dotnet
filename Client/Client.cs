using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using ActivityInfo.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ActivityInfo
{
    public class Client
    {

        private Random random = new Random();
        private Newtonsoft.Json.JsonSerializer jsonSerializer;
        private string baseUrl;
        private NetworkCredential credential;

        public Client(string email, String password)
        {
            this.baseUrl = "https://www.activityinfo.org/resources";
            this.credential = new NetworkCredential(email, password);
            jsonSerializer = new Newtonsoft.Json.JsonSerializer();
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

        private byte[] SerializeBody(object value) {

            StringWriter writer = new StringWriter();
            jsonSerializer.Serialize(new JsonTextWriter(writer), value);

            return System.Text.Encoding.UTF8.GetBytes(writer.ToString());
        }

        public ColumnSet Query(QueryModel query) {

            byte[] body = SerializeBody(query);

            HttpWebRequest request = WebRequest.CreateHttp(baseUrl + "/query/columns");
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=UTF-8";
            request.ContentLength = body.Length;
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.Credentials = this.credential;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(body, 0, body.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if(response.StatusCode != HttpStatusCode.OK) {
                throw new ActivityInfoException(String.Format("Request failed: {0}", response.StatusCode));
            }

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return this.jsonSerializer.Deserialize<ColumnSet>(new JsonTextReader(reader));
            }
        }

        private string GenerateId()
        {
            return String.Format("s{0:D10}", random.Next());
        }

        public T QueryRecord<T>(RecordRef recordRef) where T : BaseRecord, new() {

            var url = String.Format("{0}/form/{1}/record/{2}", 
                                    baseUrl, 
                                    recordRef.FormId, 
                                    recordRef.RecordId);

            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=UTF-8";
            request.PreAuthenticate = true;
            request.Method = "GET";
            request.Credentials = this.credential;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ActivityInfoException(String.Format("Request failed: {0}", response.StatusCode));
            }

            JObject jsonObject;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                jsonObject = this.jsonSerializer.Deserialize<JObject>(new JsonTextReader(reader));
            }

            T record = new T();
            record.RecordId = jsonObject["recordId"].ToString();
            record.ReadFields(jsonObject);

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
            record.RecordId = GenerateId();

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
            byte[] body = SerializeBody(tx);

            HttpWebRequest request = WebRequest.CreateHttp(baseUrl + "/update");
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=UTF-8";
            request.ContentLength = body.Length;
            request.PreAuthenticate = true;
            request.Credentials = this.credential;
            request.Method = "POST";

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(body, 0, body.Length);
            }

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            } catch(WebException e) {
                response = (HttpWebResponse)e.Response;   
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new ActivityInfoException(string.Format("Invalid update: {0}", ReadErrorMessage(response)));
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ActivityInfoException(string.Format("Request failed: {0}", response.StatusCode));
            }
        }

        private string ReadErrorMessage(WebResponse response) {
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
