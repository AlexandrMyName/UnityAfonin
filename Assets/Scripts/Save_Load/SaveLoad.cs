using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using Firebase.Database;
using Google.MiniJSON;
using Firebase;

public class SaveLoad : MonoBehaviour 
{

    DatabaseReference UserData;
    GameObject localSaves;

    public void Start()
    {
        UserData = FirebaseDatabase.DefaultInstance.RootReference;
        localSaves = GameObject.Find("User_Saves");
    }


    public static void Save (string Path, LocalSave save)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(LocalSave));
        FileStream fileStream = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write);
        xmlSerializer.Serialize(fileStream, save);
        fileStream.Close();
    }

    public static LocalSave Load (string Path)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(LocalSave));
        FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);
        LocalSave localData = (LocalSave)xmlSerializer.Deserialize(fileStream);
        fileStream.Close();
        return localData;
    }


    public IEnumerator SaveToDB(string name,string path)
    {
        //default
        LocalSave local = new LocalSave();
        local.UserName = name;
        local.UserMoney = 100.ToString();
        local.UserLevel = 1.ToString();
        //default

        SaveLoad.Save(path, local);

        UserSaves userS = new UserSaves(name,local.UserMoney, local.UserLevel);
        string js = JsonUtility.ToJson(userS);

        var dbA = UserData.Child("Saves:").Child(local.UserName.ToString()).SetRawJsonValueAsync(js);

        yield return new WaitUntil(predicate: () => dbA.IsCompleted);

        if(dbA.Exception != null)
        {
            Debug.Log(dbA.Exception);
        }
        else
        {
           



            Debug.Log("Данные отправлены на сервер" + "JSON: ");
        }
    }

    public IEnumerator LoadFromDB(string name)
    {
        var db = UserData.Child("Saves:").Child(name).GetValueAsync();

        yield return new WaitUntil(predicate: () => db.IsCompleted);

        if (db.Exception != null)
        {
            Debug.Log(db.Exception);
        }
        else
        {
            //Расширяемое..
            DataSnapshot data = db.Result;

            localSaves.GetComponent<UserSaves>().UserName = data.Child("UserName").ToString();
            localSaves.GetComponent<UserSaves>().UserMoney = data.Child("UserMoney").ToString();
            localSaves.GetComponent<UserSaves>().UserLevel = data.Child("UserLevel").ToString();


            Debug.Log("Данные подгружены с сервера");
           
        }
    }
}
