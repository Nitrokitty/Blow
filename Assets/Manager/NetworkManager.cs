using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;

public class NetworkManager : MonoBehaviour
{

    public static string CognitoAPIKey = "71d7a02e-ea15-4831-8fe1-ea62e5e2ddd0";
    public static string CognitoOrganizationName = "None440";
    public static string CognitoUrl = @"https://services.cognitoforms.com/";
    public static string ProfileFormName = "N2OBlowProfiles";
    public static string FileDataFormName = "N2OBlowFileData";
    internal static int maxRetrys = 3;
    int retryDelaySeconds = 5;
    long delayTicks;
    internal static Queue<Request> queue;
    public static List<FileData> referenceData;
    public static ProfileData profileReferenceData;

    private void Awake()
    {
        queue = new Queue<Request>();
    }

    private void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        delayTicks = new TimeSpan(hours: 0, minutes: 0, seconds: retryDelaySeconds).Ticks;

    }

    private void Update()
    {
        if (queue.Count > 0)
        {
            var request = queue.Dequeue();
            if (request.retry && request.retryCount > 0)
            {
                if (request.startTimeTicks + delayTicks > DateTime.Now.Ticks)
                {
                    queue.Enqueue(request);
                    if (queue.First() == request)
                        return;
                    request = queue.Dequeue();
                }
            }

            switch (request.type)
            {
                case RequestType.Post:
                    StartCoroutine(Post(request));
                    break;
                case RequestType.Get:
                    StartCoroutine(Get(request));
                    break;
            }
        }
    }

    public void RetrieveRuns(Dictionary<RunSearchCriteria, string> criteria, out List<FileData> data, int max = 20)
    {
        data = new List<FileData>();
        referenceData = null;
        string uri = CognitoUrl + @"forms/api/" + CognitoAPIKey + @"/forms/" + FileDataFormName + "/searchEntries?criteria==(";
        if (criteria == null || criteria.Count == 0)
            return;

        foreach (var c in criteria)
        {
            switch (c.Key)
            {
                case RunSearchCriteria.Level:
                    uri += "Level";
                    break;
                case RunSearchCriteria.UserId:
                    uri += "UserId";
                    break;
                case RunSearchCriteria.Username:
                    uri += "Username";
                    break;
                default:
                    uri += "Id1";
                    break;
            }

            uri += ".ToLower()=\"" + c.Value.ToLower();
            if (criteria.Last().Key == c.Key)
                uri += "\")";
            else
                uri += " and ";
        }

        Debug.Log(uri);

        var request = new Request(uri, "", RequestType.Get, returnFunction: null, retry: false);

        request.returnFunction = ((r) =>
        {
           
            var req = r.requestStatus;
            var text = System.Text.UTF8Encoding.Default.GetString(req.downloadHandler.data);
            List<FileData> fData = new List<FileData>();

            var keys = new List<string>() { "Level", "UserId","Id1", "Time","Username" };
            while (text.Contains("Level"))
            {
                var f = new FileData(false);
                foreach (var key in keys)
                {                
                    text = text.Substring(text.IndexOf(key) + key.Length + 3);
          
                    var value = text.Substring(0, text.IndexOf('"'));
                    if (key == "Level")
                        f.Level = GameManager.GetLevelFromDisplayName(value);
                    else if (key == "UserId")
                        f.UserID = value;
                    else if (key == "Id1")
                        f.ID = value;
                    else if (key == "Time")
                        f.Time = Int64.Parse(value);
                    else
                        f.Username = value;
                }
                fData.Add(f);
            }
            

            referenceData = fData.OrderBy((f) => f.Time).ToList();
            return true;
        });

        queue.Enqueue(request);
    }

    public void RetrieveProfile(Dictionary<ProfileSearchCriteria, string> criteria, Action action, bool executeActionOnProfileFound = true)
    {
        string uri = CognitoUrl + @"forms/api/" + CognitoAPIKey + @"/forms/" + ProfileFormName + "/searchEntries?criteria==(";
        if (criteria == null || criteria.Count == 0)
            return;
        foreach(var c in criteria)
        {
            switch (c.Key)
            {
                case ProfileSearchCriteria.Email:
                    uri += "Email";
                    break;
                case ProfileSearchCriteria.Username:
                    uri += "Username";
                    break;
                default:
                    uri += "Id1";
                    break;
            }

            uri += ".ToLower()=\"" + c.Value.ToLower();
            if (criteria.Last().Key == c.Key)
                uri += "\")";
            else
                uri += " and ";
        }

        Debug.Log(uri);

        var request = new Request(uri, "", RequestType.Get, returnFunction: null, retry: false);

        request.returnFunction = ((r) =>
        {
            var req = r.requestStatus;
            var text = System.Text.UTF8Encoding.Default.GetString(req.downloadHandler.data);
            Debug.Log(text);
            if (text.Replace(" ", string.Empty) != "{\"entries\":[],\"orders\":[]}" && executeActionOnProfileFound)
                action();
            else if (text.Replace(" ", string.Empty) == "{\"entries\":[],\"orders\":[]}" && !executeActionOnProfileFound)
                action();
            return true;
        });

        queue.Enqueue(request); 
    }

    public void RetrieveProfileNow(Dictionary<ProfileSearchCriteria, string> criteria, Action action, bool executeActionOnProfileFound = true)
    {
        string uri = CognitoUrl + @"forms/api/" + CognitoAPIKey + @"/forms/" + ProfileFormName + "/searchEntries?criteria==(";

        Debug.Log("RETRIEVE");

        if (criteria == null || criteria.Count == 0)
            return;
        foreach (var c in criteria)
        {
            switch (c.Key)
            {
                case ProfileSearchCriteria.Email:
                    uri += "Email";
                    break;
                case ProfileSearchCriteria.Username:
                    uri += "Username";
                    break;
                case ProfileSearchCriteria.Password:
                    uri += "Password";
                    break;
                default:
                    uri += "Id1";
                    break;
            }

            uri += ".ToLower()=\"" + c.Value.ToLower();
            if (criteria.Last().Key == c.Key)
                uri += "\")";
            else
                uri += "\" and ";
        }

        Debug.Log(uri);

        var request = new Request(uri, "", RequestType.Get, returnFunction: null, retry: false);

        request.returnFunction = ((r) =>
        {
            var req = r.requestStatus;
            var text = System.Text.UTF8Encoding.Default.GetString(req.downloadHandler.data);
            Debug.Log(text);
            if (text.Replace(" ", string.Empty) != "{\"entries\":[],\"orders\":[]}")
            {
                profileReferenceData = new ProfileData(true);
                var keys = new List<string>() { "Username", "Id1", "Email", "Password" };
                foreach (var key in keys)
                {

                    text = text.Substring(text.IndexOf(key) + key.Length + 3);
                    var value = text.Substring(0, text.IndexOf('"'));
                    if (key == "Username")
                        profileReferenceData.UserName = value;
                    else if (key == "Id1")
                        profileReferenceData.ID = value;
                    else if (key == "Email")
                        profileReferenceData.Email = value;
                    else
                        profileReferenceData.Password = value;
                    Debug.Log(value);
                }
                //username, id, email, password
                
                if(executeActionOnProfileFound)
                    action();
            }
            else if (text.Replace(" ", string.Empty) == "{\"entries\":[],\"orders\":[]}" && !executeActionOnProfileFound)
                action();
            return true;
        });
        GetNow(request);

        return;
    }

    public void ProfileIDExistsCallback()
    {

    }

    public void PostProfile(ProfileData profile)
    {
        Hashtable headers = new Hashtable();
        headers.Add("Content-Type", "application/json");

        var uri = CognitoUrl + @"forms/api/" + CognitoAPIKey + @"/forms/" + ProfileFormName + @"/entry";

        var jsonObj = new JSON();
        jsonObj.AddLine("$type", "Cognito.Forms.FormEntry." + CognitoOrganizationName + "." + ProfileFormName);
        jsonObj.AddLine("$version", "6");
        jsonObj.AddLine("$etag", "c6dbbee3-f990-4312-9906-8cd82bf8df4e");
        jsonObj.AddLine("Username", profile.UserName);
        jsonObj.AddLine("Email", profile.Email);
        jsonObj.AddLine("Id1", profile.ID);
        jsonObj.AddLine("Password", profile.Password);

        Debug.Log(jsonObj.ToString());

        queue.Enqueue(new Request(uri, jsonObj.ToString(), RequestType.Post, returnFunction: null, retry: true));
    }

    public void PostRun(FileData data)
    {
        Hashtable headers = new Hashtable();
        headers.Add("Content-Type", "application/json");

        var uri = CognitoUrl + @"forms/api/" + CognitoAPIKey + @"/forms/" + FileDataFormName + @"/entry";

        var jsonObj = new JSON();
        jsonObj.AddLine("$type", "Cognito.Forms.FormEntry." + CognitoOrganizationName + "." + FileDataFormName);
        jsonObj.AddLine("$version", "6");
        jsonObj.AddLine("$etag", "c6dbbee3-f990-4312-9906-8cd82bf8df4e");
        jsonObj.AddLine("Level", GameManager.GetLevelDisplayName(data.Level));
        jsonObj.AddLine("Username", data.Username);
        jsonObj.AddLine("Id1", data.ID);
        jsonObj.AddLine("UserId", data.UserID);
        jsonObj.AddLine("Time", data.Time.ToString());

        Debug.Log(jsonObj.ToString());

        queue.Enqueue(new Request(uri, jsonObj.ToString(), RequestType.Post, returnFunction: null, retry: true));
    }

    //https://codetolight.wordpress.com/2017/01/18/unity-rest-api-interaction-with-unitywebrequest-and-besthttp/
    IEnumerator Post(string uri, string json, Request requestObj = null)
    {
        if (requestObj == null)
            requestObj = new Request(uri, json, RequestType.Post);

        requestObj.requestStatus = UnityWebRequest.Post(uri, json);

        

        yield return requestObj.SendAsync();


    }

    void GetNow(Request request)
    {
        request.requestStatus = UnityWebRequest.Get(request.uri);

        request.SendSync();
    }

    IEnumerator Get(Request request)
    {
        request.requestStatus = UnityWebRequest.Get(request.uri);

        yield return request.SendAsync();
    }

    IEnumerator Post(Request request)
    {
        request.requestStatus = UnityWebRequest.Post(request.uri, request.json);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(request.json);
        request.requestStatus.uploadHandler = new UploadHandlerRaw(bodyRaw);

        yield return request.SendAsync();
    }


}

public class JSON
{
    string jsonTxt = "";

    public void AddLine(string key, string value)
    {
        jsonTxt += jsonTxt == "" || jsonTxt.Last() == '{' ? "\"" + key + "\":\"" + value + "\"" : ",\"" + key + "\":\"" + value + "\"";
    }

    public void AddBracket(bool isOpenBracket)
    {
        jsonTxt = isOpenBracket ? "{" : "}";
    }

    public override string ToString()
    {
        return "{" + jsonTxt + "}";
    }
}

public class Request
{
    internal string uri;
    internal string json;
    internal RequestType type;
    internal Func<Request, bool> returnFunction;
    internal bool retry;
    internal int retryCount = 0;
    internal long startTimeTicks;
    internal AsyncOperation call;
    internal UnityWebRequest requestStatus;

    public Request(string uri, string json, RequestType type, Func<Request, bool> returnFunction = null, bool retry = false)
    {
        this.uri = uri;
        this.json = json;
        this.type = type;
        this.returnFunction = returnFunction;
        this.retry = retry;
    }

    public IEnumerator SendAsync()
    {
        requestStatus.SetRequestHeader("Content-Type", "application/json");

        requestStatus.downloadHandler = new DownloadHandlerBuffer();

        yield return requestStatus.Send();

        CheckRequest(this);
    }

    public void SendSync()
    {
        requestStatus.SetRequestHeader("Content-Type", "application/json");

        requestStatus.downloadHandler = new DownloadHandlerBuffer();

        requestStatus.Send();

        CheckRequest(this);
    }

    void CheckRequest(Request requestObj)
    {
        var requestErrorOccurred = false;

       
        if (requestObj.requestStatus.responseCode == 0)
        {
            var originalTimeScale = Time.timeScale;
            
            while (requestObj.requestStatus.responseCode == 0)
            {
              
            }
            Time.timeScale = originalTimeScale;
        }

        if (requestObj.requestStatus.isError)
        {
            Debug.Log("Something went wrong, and returned error: " + requestObj.requestStatus.error);
            requestErrorOccurred = true;
        }
        else
        {
            if (requestObj.requestStatus.responseCode == 201 || requestObj.requestStatus.responseCode == 200)
                Debug.Log("Request finished successfully!");
            else if (requestObj.requestStatus.responseCode == 401)
            {
                Debug.Log("Error 401: Unauthorized. Resubmitted request!");
                requestErrorOccurred = true;
            }
            else
            {
                Debug.Log("Request failed (status:" + requestObj.requestStatus.responseCode + ").");
                requestErrorOccurred = true;
            }
        }

        if (requestErrorOccurred && requestObj.retry && requestObj.retryCount < NetworkManager.maxRetrys)
        {
            requestObj.retryCount++;
            requestObj.startTimeTicks = DateTime.Now.Ticks;
            NetworkManager.queue.Enqueue(requestObj);
        }
        else if (requestObj.returnFunction != null)
        {
            returnFunction(this);
        }

    }
}

public enum RequestType
{
    Post,
    Get
}

public enum ProfileSearchCriteria
{
    ID,
    Email,
    Username,
    Password
}

public enum RunSearchCriteria
{
    Level,
    RunId,
    UserId,
    Username
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}