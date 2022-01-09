using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Text;

namespace TestCases
{
    public class MyDb : IDisposable
    {
        public FirestoreDb firestoreDb;
        public MyDb()
        {
            var firebaseSecret = "{Paste Base64 encoded firebase sdk JSON here}";
            var firebaseJson = Encoding.UTF8.GetString(Convert.FromBase64String(firebaseSecret));
            var projectId = JsonConvert.DeserializeObject<dynamic>(firebaseJson).project_id;

            firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                JsonCredentials = firebaseJson
            }.Build();
        }

        public void Dispose()
        {
            firestoreDb = null;
        }
    }
}