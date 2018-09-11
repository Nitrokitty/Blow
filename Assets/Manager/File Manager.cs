using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Linq;

public static class FileManager {


    public static string FileDirectoryPath = Application.persistentDataPath;
    static string InfoFileName = "info";
    static string ProfileFileName = "profile";
    public static string FileExtension = "dat";
    static string CurrentFileName { get; set; }
    static BinaryFormatter binaryFormatter;
    static BinaryFormatter BinaryFormatter
    {
        get
        {
            if (binaryFormatter == null)
                binaryFormatter = new BinaryFormatter();
            return binaryFormatter;
        }
    }
    static string FullFilePath
    {
        get {  return FileDirectoryPath + @"/" + CurrentFileName + "." + FileExtension; }
    }
    static bool isSuccessful = false;
    static bool fileIsSet = false;

    public static bool Load<T>(out List<T> data)
        where T : Data
    {
        data = Load<T>();

        return isSuccessful;
    }

    static List<T> Load<T>()
         where T : Data
    {
        var data = new List<T>();

        isSuccessful = false;

        if (!SetFile(typeof(T)))
        {
            isSuccessful = true;
            return data;
        }

        using (var stream = File.Open(FullFilePath, FileMode.Open))
        {
            try
            {
                data = BinaryFormatter.Deserialize(stream) as List<T>;
                isSuccessful = true;

                Debug.Log("Game Load Successful!");
            }
            catch (SerializationException e)
            {
                Debug.Log("Could Not Load game: " + e.Message);
            }
        }

        return data;
    }

    public static bool TryGetUser(out ProfileData profileData, string password, string userName = null, string email = null)
    {
        profileData = null;

        if (userName == null && email == null)
            return false;

        var profiles = Load<ProfileData>();

        if (profiles.Count == 0)
            return false;

        foreach(var p in profiles)
        {
            if((!string.IsNullOrEmpty(userName) ? p.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) : true) && 
                (p.Password == password) &&
                (!string.IsNullOrEmpty(email) ? p.Email.Equals(email, StringComparison.OrdinalIgnoreCase) : true)
            )
            {
                profileData = p;
                return true;
            }
        }
        return false;

    }

    public static bool Append<T>(T instanceData = null)
         where T : Data
    {
        var data = Load<T>();

        data.Add(instanceData??(T) Activator.CreateInstance(typeof(T), args: new object[] { true }));

        Save(data);

        return isSuccessful;
    }

    public static bool Overwrite<T>(string id, T instanceData = null)
         where T : Data
    {
        var data = Load<T>();
        instanceData = instanceData ?? (T)Activator.CreateInstance(typeof(T), args: new object[] { true });

        data.Remove(data.First((d) => d.ID == id));
        data.Add(instanceData);

        Save(data);

        return isSuccessful;
    }

    static void Save<T>(List<T> data)
        where T : Data
    {
        SetFile(typeof(T));

        isSuccessful = false;

        using (var stream = File.Open(FullFilePath, FileMode.Open))
        {
            try
            {
                BinaryFormatter.Serialize(stream, data);
                isSuccessful = true;
                Debug.Log("Game Save Successful (" + FullFilePath + ")!");
            }
            catch (SerializationException e)
            {
                Debug.Log("Could Not Save game");
                Debug.Log(e.Message);
            }
        }
    }

    static bool SetFile(Type t)
    {
        if (t == typeof(FileData))
            CurrentFileName = InfoFileName;
        else if (t == typeof(ProfileData))
            CurrentFileName = ProfileFileName;

        fileIsSet = true;

        if (!File.Exists(FullFilePath))
            File.Create(FullFilePath).Close();
        
        return File.ReadAllLines(FullFilePath).Length != 0;
    }

    public static List<FileData> GetRuns(Levels level, string userId = null)
    {
        var data = Load<FileData>();

        if (data == null || data.Count == 0)
            return data;

        data = string.IsNullOrEmpty(userId) ? data : data.Where((d) => d.UserID == userId).ToList();

        var runs = new List<FileData>();

        foreach (var d in data)
        {
            if (d.Level == level)
                runs.Add(d);
        }

        return runs;
    }

    public static List<FileData> GetRuns(Levels level, int rankMax, string userId = null)
    {
        var runs = GetRuns(level, userId);

        runs = runs.OrderBy((r) => r.Time).ToList();
        if (rankMax > 0 && runs.Count > rankMax)
            runs = runs.Take(rankMax).ToList();

        return runs;
    }
}

[Serializable]
public abstract class Data
{
    public string ID;
    protected string ToStringDelimieter = "\t";
    protected string IDDelimeter = "#";
    public Data()
    {   }

    public override string ToString()
    {
        return "ID: " + ID;
    }
}

[Serializable]
public class FileData : Data
{
    public Levels Level;
    public long Time;
    public string UserID;
    public string Username;

    public FileData(bool useCurrentData = false)
         : base()
    {
        if (useCurrentData)
        {
            Level = GameManager.Manager.CurrentLevel;
            Time = GameManager.Manager.CurrentRunTime.Ticks;
            UserID = UserProfile.ID;
            Username = UserProfile.UserName;
            ID = UserID + IDDelimeter +  Level + IDDelimeter + Time;
        }
    }

    public override string ToString()
    {
        return "Time: " + Time + ToStringDelimieter + "Level: " + Level + ToStringDelimieter + "UserID: " + UserID + ToStringDelimieter + "Username: " + Username + ToStringDelimieter + base.ToString();
    }

}

[Serializable]
public class ProfileDataJson
{
    public List<ProfileData> Data { get; set; }
}

[Serializable]
public class FileDataJson
{
    public List<Entries> Entries { get; set; }

    
}

[Serializable]
public class Entries
{
    public Form Form { get; set; }
    public Entry Entry { get; set; }

    
}

[Serializable]
public class Form
{

}

[Serializable]
public class Entry
{
    public Form Level { get; set; }
    public Form UserId { get; set; }
    public string Id { get; set; }
    public int Time { get; set; }
    public string Username { get; set; }
}



[Serializable]
public class ProfileData : Data
{
    public string UserName;
    public string Password;
    public string Email;

    public ProfileData(string password, bool useCurrentData = false)
        : this(useCurrentData)
    {
        Password = password;
    }

    public ProfileData(string userName = null, string password = null, string email = null, string id = null)
        : this(false)
    {
        UserName = userName;
        Password = password;
        Email = email;
        ID = id;
    }

    public ProfileData(bool useCurrentData = false)
        :base()
    {
        if (useCurrentData)
        {
            UserName = UserProfile.UserName;
            ID = UserProfile.ID;
            Email = UserProfile.Email;
        }
    }

    public override string ToString()
    {
        return "UserName: " + UserName + ToStringDelimieter + "Password: " + Password + ToStringDelimieter + base.ToString();
    }
}