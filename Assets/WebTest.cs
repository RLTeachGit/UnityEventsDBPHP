using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;



public class WebTest : MonoBehaviour
{

    static  string GenerateHash(string vName) {
        var tDevice = SystemInfo.deviceUniqueIdentifier;
        var tID = tDevice + vName;
        return Hash128.Compute(tID).ToString();
    }

    string mSessionUUID;
    string mUserUUID;

    string mDBURI = "https://modulo17.com/dbtest/user.php";
//    string mDBURI = "https://rlshed.com/debug/user.php";

    void Start()
	{
        mSessionUUID = System.Guid.NewGuid().ToString(); //New Session ID everytime we run

        mUserUUID = GenerateHash("Richard"); //Make User ID from device and a user name
        StartCoroutine(NewUser());
	}


    public void ClickMe(string vText) {
        string tEventText = string.Format("User Clicked:{0}", vText);
        StartCoroutine(NewEvent(tEventText));
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
                    Debug.Log("Duplicate ID");
                    StartCoroutine(NewSession());
                } else {
                    Debug.Log(www.error);
                }
            }
			else
			{
				Debug.Log("User Created");
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
                    Debug.Log("Duplicate ID");
                } else {
                    Debug.Log(www.error);
                }
            } else {
                Debug.Log("Session Created");
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
                    Debug.Log("Duplicate ID");
                } else {
                    Debug.Log(www.error);
                }
            } else {
                Debug.Log("Event Created");
            }
        }
    }
}
