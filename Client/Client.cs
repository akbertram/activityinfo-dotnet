using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using ActivityInfo.Query;
using ActivityInfo.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ActivityInfo
{
    public class Client
    {

        private Random random = new Random();
        private Newtonsoft.Json.JsonSerializer jsonSerializer;
        private string baseUrl;
        private string authentication;

        public Client(string email, String password)
        {
            this.baseUrl = "https://www.activityinfo.org/resources";
            this.authentication = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(email + ":" + password));

            jsonSerializer = new Newtonsoft.Json.JsonSerializer();
            jsonSerializer.DateFormatString = "yyyy-MM-dd";
        }

        public List<Partner> QueryPartners(int databaseId)
        {
            var formId = string.Format("P{0:D10}", databaseId);
            return Query<Partner>(formId);
        }

        public T QueryResource<T>(string path)
        {
            HttpWebRequest request = WebRequest.CreateHttp(baseUrl + path);
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=UTF-8";
            request.Method = "GET";
            request.Headers.Add("Authorization", authentication);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return this.jsonSerializer.Deserialize<T>(new JsonTextReader(reader));
            }
        }

        public DatabaseMetadata QueryDatabase(int databaseId)
        {
            return QueryResource<DatabaseMetadata>(
                String.Format("/database/{0}", databaseId));
        }

        public object QuerySchema(string formId)
        {
            return QueryResource<FormSchema>(
                String.Format("/form/{0}/schema", formId));
        }

        public List<T> Query<T>(string formId) where T : new()
        {
            QueryMapper<T> mapper = new QueryMapper<T>(formId);
            ColumnSet columnSet = Query(mapper.QueryModel);
            return mapper.Map(columnSet);
        }

        public ColumnSet QueryAllColumns(string formId)
        {
            return QueryResource<ColumnSet>(String.Format("/form/{0}/query/columns", formId));
        }

        private byte[] SerializeBody(object value)
        {

            StringWriter writer = new StringWriter();
            jsonSerializer.Serialize(new JsonTextWriter(writer), value);

            return System.Text.Encoding.UTF8.GetBytes(writer.ToString());
        }

        public ColumnSet Query(QueryModel query)
        {

            byte[] body = SerializeBody(query);

            HttpWebRequest request = WebRequest.CreateHttp(baseUrl + "/query/columns");
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=UTF-8";
            request.ContentLength = body.Length;
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.Headers.Add("Authorization", authentication);

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(body, 0, body.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
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

        public T QueryRecord<T>(RecordRef recordRef) where T : BaseRecord, new()
        {

            var url = String.Format("{0}/form/{1}/record/{2}",
                                    baseUrl,
                                    recordRef.FormId,
                                    recordRef.RecordId);

            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=UTF-8";
            request.PreAuthenticate = true;
            request.Method = "GET";
            request.Headers.Add("Authorization", authentication);

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
        public RecordRef CreateRecord<T>(T record) where T : BaseRecord
        {
            Transaction tx = new Transaction();
            record.RecordId = GenerateId();

            tx.AddChange(new RecordUpdate(record));

            ExecuteUpdate(tx);

            return record.Ref;
        }

        public void DeleteRecord(RecordRef recordRef)
        {


        }

        /// <summary>
        /// Updates an existing record
        /// </summary>
        /// <param name="record">Record.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void UpdateRecord<T>(T record) where T : BaseRecord
        {
            Transaction tx = new Transaction();
            tx.AddChange(new RecordUpdate(record));

            ExecuteUpdate(tx);
        }

        public void ExecuteUpdate(IChange change)
        {
            Transaction tx = new Transaction();
            tx.AddChange(change);

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
            request.Headers.Add("Authorization", authentication);
            request.Method = "POST";

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(body, 0, body.Length);
            }

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
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

        private string ReadErrorMessage(WebResponse response)
        {
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}