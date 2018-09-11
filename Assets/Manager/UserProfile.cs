using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UserProfile : MonoBehaviour

{
    public static string UserName = "Guest";
    public static string ID = "00000000-0000-0000-0000-000000000000";
    public static string Email = "guest@profile.dat";
    public static string GuestID = "00000000-0000-0000-0000-000000000000";
    public static string GuestUserName = "Guest";
    public static string GuestEmail = "guest@profile.dat";

    public static bool ContinueAsGuest = false;
    public static bool IsLoggedIn = false;

    //string testID =  "a3471314-d529-445b-bd7d-e0f42424f11e";

    private void Awake()
    {
        if(GameObject.FindGameObjectsWithTag("User Profile").Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(transform.gameObject);
        var profileData = new List<ProfileData>();
        FileManager.Load(out profileData);

        //Create Guest Account
        if (profileData.Count == 0)
        {
            FileManager.Append<ProfileData>();
        }

    }

    public static string GenereateID(bool ensureIsUnique, bool setId)
    {

        string id = Guid.NewGuid().ToString();

        var networkManager = GameObject.Find("UserProfile").GetComponent<NetworkManager>();

        if (ensureIsUnique)
        {
            var d = new Dictionary<ProfileSearchCriteria, string>();
            d.Add(ProfileSearchCriteria.ID, id);
            networkManager.RetrieveProfile(d, () =>
            {                
                var oldSetId = setId;
                setId = false;
                UserProfile.GenereateID(true, oldSetId);
            });
        }

        if (setId)
            ID = id;
        return id;
    }

    public static void SetUserProfile(ProfileData profileData)
    {
        UserName = profileData.UserName;
        ID = profileData.ID;
        Email = profileData.Email;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
