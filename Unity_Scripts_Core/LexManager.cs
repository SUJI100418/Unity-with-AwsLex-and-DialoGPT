using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
using UnityEngine.UI;

/*
lex 서버 주소: (엔드 포인트 삭제)
*/

[Serializable]
public class Player
{
    public string text;  // lex와 연동, 이름 변경하지 말기.
}
public class Fox
{
    public string utterance;
    public string intent;
    public string input_text;
}

public class LexManager : MonoBehaviour
{
    private string sample_text = "전화번호가 알고 싶어요.";
    public string user_id = "me";

    public Text inputField;
    public Text Bot_displayText;

    //로그용
    private bool isShowLogText = false;
    public Text Input_Text_Log;
    public Text Intent_Text_Log;

    public Sprite[] sprite;
    public Image image;
    int idx = 0;
    public GameObject Timeline;

    int count = 1;
    private int end_count = 15;

    public bool can_send = true;


    void Start()
    {
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && inputField.text != "" && can_send)
        {
            CallPostRequest(inputField.text);
            Bot_displayText.text = "...";
            can_send = false;
        }

        if (count == end_count)
        {
            Debug.Log('끝');

            CaseManager.instance.is_finish = true;
            CaseManager.instance.Exit_NPC(idx);

            count = 0;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            count = end_count - 1;
        }

        ShowLogText();
    }

    public void CallPostRequest(string txt)
    {
        // 입력 : json 파일 생성
        Player thisPlayer = new Player();
        thisPlayer.text = inputField.text;
        string json = JsonUtility.ToJson(thisPlayer);
        string address = $"https:/(엔드 포인트 삭제)/api/fox-bot/user-id/{user_id}/send-text";
        Debug.Log(json + address);

        Clear_InputField();

        // POST 
        StartCoroutine(PostRequest(address, json));
    }

    IEnumerator PostRequest(string uri, string json)
    {
        Fox thisFox = new Fox();

        var uwr = new UnityWebRequest(uri, "POST");
        byte[] jsonTOSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonTOSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            sample_text = "Error";
            Debug.Log("Error!!");
        }
        else
        {
            Debug.Log(uwr.downloadHandler.text);

            //thisFox.utterance = uwr.downloadHandler.text;
            string[] split_text;
            split_text = uwr.downloadHandler.text.Split(':');

            thisFox.utterance = split_text[0];
            thisFox.intent = split_text[1];
            thisFox.input_text = split_text[2];

            Debug.Log("utterance : " + thisFox.utterance);
            Debug.Log("intent : " + thisFox.intent);
            Debug.Log("input_text : " + thisFox.input_text);
        }

        Bot_displayText.text = thisFox.utterance;

        Intent_Text_Log.text = "의도: " + thisFox.intent + ", " + count;
        Input_Text_Log.text = "번역: " + thisFox.input_text;

        class_intent(thisFox.intent);
        UpdateSprite();

        count += 1;
        can_send = true;
    }

   private void Clear_InputField()
   {
        var inputFieldPaernt = inputField.transform.parent.GetComponent<InputField>();
        inputFieldPaernt.text = "";
   }

    private void ShowLogText()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            isShowLogText = !isShowLogText;
        }

        if (isShowLogText)
        {
            Input_Text_Log.gameObject.SetActive(true);
            Intent_Text_Log.gameObject.SetActive(true);
        }
        else if(!isShowLogText)
        {
            Input_Text_Log.gameObject.SetActive(false);
            Intent_Text_Log.gameObject.SetActive(false);
        }
    }

    private void UpdateSprite()
    {
        image.sprite = sprite[idx];
        image.preserveAspect = true;
    }
}