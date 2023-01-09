using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System;
using UnityEngine.SceneManagement;
using Google.MiniJSON;
using UnityEngine.SocialPlatforms;

public class Autorization_Game : MonoBehaviour
{
    [SerializeField] private GameObject button_All;
    [SerializeField] private GameObject button_Register;
    [SerializeField] private GameObject button_SignIN;
    [SerializeField] private GameObject button_SignOUT;

    private DatabaseReference db;
    private FirebaseAuth autorization;
    [SerializeField] private bool isSign_IN;

    [SerializeField] private string[] urls;
    [SerializeField] private bool isIthernet;

    public string path;

    [Obsolete]
    void Start()
    {
        StartCoroutine(Check_IthernetConection(result =>
        {
            db = FirebaseDatabase.DefaultInstance.RootReference;
            autorization = FirebaseAuth.DefaultInstance;

            isIthernet = result;
            if(result)
            {
                Debug.Log("�������� ���������� � ����� ������ �����������!");
                autorization.StateChanged += Check_CurentUser;
            }
            else
            {
                Debug.Log("���� �����������! ��������� �������� �����������...");
                autorization.StateChanged -= Check_CurentUser;
            }
        }));
    }

    [Obsolete]
    private IEnumerator Check_IthernetConection(Action <bool> callBack)
    {
        foreach(string url in urls)
        {
            UnityWebRequest web = UnityWebRequest.Get(url);

            yield return web.SendWebRequest();

            if(web.isNetworkError != true)
            {
                callBack(true);
                yield break;
            }
            callBack(false);
        }
    }

    private void Check_CurentUser(object slender,EventArgs e)
    {
        if(autorization.CurrentUser != null)
        {
            isSign_IN = true;
            Debug.Log("����� ���������� : " + autorization.CurrentUser.Email);
        }
        else
        {
            isSign_IN = false;
            Debug.Log("����������� �� ���������, ���������� ����������������� ��� ������� � �������! ");
            Debug.Log("|[������ ���������� ��������, ����������� ���� � ������� ����� �������� �������������]|");
        }
    }
    [Obsolete]
    private IEnumerator Check_Email_Registr(string email,string password, Action <bool> callBack)
    {
        var auth = autorization.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(predicate: () => auth.IsCompleted);

        if(auth.Exception != null)
        {
            Debug.Log($"������ ������������: " + auth.Exception);
        }
        else
        {
            Debug.Log($"������������ ���������:" + autorization.CurrentUser.Email);
        }

        if(autorization.CurrentUser != null)
        {
            callBack(true);
            yield break;
        }
        callBack(false);
    }

    [Obsolete]
    private IEnumerator Check_Email_SignIN(string email, string password, Action<bool> callBack)
    {
        var auth = autorization.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(predicate: () => auth.IsCompleted);

        if (auth.Exception != null)
        {
            Debug.Log($"������ ������������: " + auth.Exception);
        }
        else
        {
            Debug.Log($"������������ ���������:" + autorization.CurrentUser.Email);
        }

        if (autorization.CurrentUser != null)
        {
            callBack(true);
            yield break;
        }
        callBack(false);
    }

    #region Autorization [REGISTER/LOGIN]
    [Obsolete]
    public void Autorization_Register(string email,string name,string password)
    {
        StartCoroutine(Check_IthernetConection(result =>
        {
            isIthernet = result;
            if (result)
            {
                Debug.Log("����������!");

                StartCoroutine(Check_Email_Registr(email, password, result =>
                {
                    if (result)
                    {
                       StartCoroutine(DataBase_Save(email, name, password));     
                    }
                    else
                    {
                        Debug.Log("Email �� ���������� ��� ������ ������!");
                    }
                }));
            }
            else
            {
                Debug.Log("���� �����������!");
               
            }
        }));
    }

    [Obsolete]
    public void Autorization_SignIN(string email, string name, string password)
    {
        StartCoroutine(Check_IthernetConection(result =>
        {
            isIthernet = result;
            if (result)
            {
                Debug.Log("����������!");

                StartCoroutine(Check_Email_SignIN(email, password, result =>
                {
                    if (result)
                    {
                        StartCoroutine(DataBase_Load(name));
                    }
                    else
                    {
                        Debug.Log("Email �� ���������� ��� ������ ������!");
                    }
                }));
            }
            else
            {
                Debug.Log("���� �����������!");
            }
        }));
    }

    public void Autorization_SignOUT()
    {
        Debug.Log("�������� ����� �� �������!");
        autorization.SignOut();
        SceneManager.LoadScene(0);
    }
    #endregion
    private IEnumerator DataBase_Save(string email,string name,string password)
    {
        DateTime date = new DateTime();
        //date = date.AddDays(1);
        User curentUser = new User(name, password, email, date.ToString());

        var json = JsonUtility.ToJson(curentUser);

        User user = JsonUtility.FromJson<User>(json);

        var curentDB = db.Child("Users:").Child(name).SetRawJsonValueAsync(json);

        yield return new WaitUntil(predicate: () => curentDB.IsCompleted);

        if(curentDB.Exception != null)
        {
            Debug.Log("������ ���� ������: " + curentDB.Exception.Message);
        }
        else
        {
           

            SaveLoad saveloadDB = GameObject.Find("User_Saves").GetComponent<SaveLoad>();

            StartCoroutine(saveloadDB.SaveToDB(name,path));

            //GameObject localDataSave = GameObject.Find("User_Saves");
            //localDataSave.GetComponent<UserSaves>().UserName = name;




            Debug.Log("������ ���������");
        }
    }

    private IEnumerator DataBase_Load(string name)
    {

        var curentDB = db.Child("Users:").Child(name).GetValueAsync();

        
        yield return new WaitUntil(predicate: () => curentDB.IsCompleted);

        if (curentDB.Exception != null)
        {
            Debug.Log("������ ���� ������: " + curentDB.Exception.Message);
        }
        else
        {
            DataSnapshot data = curentDB.Result ;

            Debug.Log("������������! " + data.Child("name").Value.ToString());


            SaveLoad saveloadDB = GameObject.Find("User_Saves").GetComponent<SaveLoad>();

           
            Debug.Log("PARSE XML");

                StartCoroutine(saveloadDB.SaveToDB(name,path));
                Debug.Log("���������� � ���� ������..");

                StartCoroutine(saveloadDB.LoadFromDB(name));
                Debug.Log("�������� �� ���� ������..");
            

            Debug.Log("������ ���������" + data.Child("Users:").Value + " | [USER] |");
            Debug.Log("This project uses Google service autorization");
        }
    }
}
