using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;



public class WebTest : MonoBehaviour
{

    public  Text DebugText;

    static  string GenerateHash(string vName) {
        var tDevice = SystemInfo.deviceUniqueIdentifier;
        var tID = tDevice + vName;
        return Hash128.Compute(tID).ToString();
    }

    string mSessionUUID;
    string mUserUUID;

    string mDBURI = "https://modulo17.com/dbtest/user.php";
    //    string mDBURI = "https://rlshed.com/debug/user.php";


    List<string> DebugStrings=new List<string>();

    void Start()
	{
        AddDebugString("Start");

        while (true) {
            if(PlayerPrefs.HasKey("PlayerID")) {
                mUserUUID = PlayerPrefs.GetString("PlayerID");
                break;
            }
            PlayerPrefs.SetString("PlayerID", System.Guid.NewGuid().ToString());
        }

        mSessionUUID = System.Guid.NewGuid().ToString(); //New Session ID everytime we run
        AddDebugString("UserID:"+mUserUUID+" SessionID:"+mSessionUUID);

        StartCoroutine(NewUser());
	}


    public void ClickMe(string vText) {
        string tEventText = string.Format("User Clicked:{0}", vText);
        StartCoroutine(NewEvent(tEventText));
    }

    void    AddDebugString(string vString) {
        if (DebugStrings.Count > 10) {
            DebugStrings.RemoveRange(0, DebugStrings.Count - 10);
        }
        DebugStrings.Add(vString);

        DebugText.text = "";
        foreach (string tS in DebugStrings) {
            DebugText.text += tS + "\n";
        }
    }

    IEnumerator NewUser() //Create new user, will only work once per user
	{
		WWWForm form = new WWWForm();
        form.AddField("command", "newuser");
        form.AddField("userUUID", mUserUUID);

		using (UnityWebRequest www = UnityWebRequest.Post(mDBURI, form))
		{
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError)
			{
                if (www.error.Contains("409")) {
                    AddDebugString("Duplicate ID");
                    StartCoroutine(NewSession());
                } else {
                    AddDebugString(www.error);
                }
            }
			else
			{
				AddDebugString("User Created");
                StartCoroutine(NewSession());
            }
        }
	}

    IEnumerator NewSession() {
        WWWForm form = new WWWForm();
        form.AddField("command", "newsession");
        form.AddField("userUUID", mUserUUID);
        form.AddField("sessionUUID", mSessionUUID);

        using (UnityWebRequest www = UnityWebRequest.Post(mDBURI, form)) {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                if (www.error.Contains("409")) {
                    AddDebugString("Duplicate ID");
                } else {
                    AddDebugString(www.error);
                }
            } else {
                AddDebugString("Session Created");
                }
            }
    }

    IEnumerator NewEvent(string vName) {
        WWWForm form = new WWWForm();
        form.AddField("command", "newevent");
        form.AddField("userUUID", mUserUUID);
        form.AddField("sessionUUID", mSessionUUID);
        form.AddField("event", vName);

        using (UnityWebRequest www = UnityWebRequest.Post(mDBURI, form)) {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                if (www.error.Contains("409")) {
                    AddDebugString("Duplicate ID");
                } else {
                    AddDebugString(www.error);
                }
            } else {
                AddDebugString("Event Created");
            }
        }
    }
}
