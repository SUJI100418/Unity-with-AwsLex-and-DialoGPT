using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
using UnityEngine.UI;

/*
gate way 서버 주소: (엔드 포인트 삭제)
*/

public class DialoGPTManager : MonoBehaviour
{
    private string sample_text = "전화번호가 알고 싶어요.";
    public string user_id = "me";

    public Text inputField;
    public Text Bot_displayText;

    //로그용
    private bool isShowLogText = false;
    public Text Input_Text_En_Log;
    public Text Output_Text_En_Log;


    void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && inputField.text != "")
        {
            CallPostRequest(inputField.text);
            Bot_displayText.text = "...";
        }

        ShowLogText();
    }

    public void CallPostRequest(string txt)
    {
        // 입력 : json 파일 생성
        Player thisPlayer = new Player();
        thisPlayer.text = inputField.text;
        string json = JsonUtility.ToJson(thisPlayer);
        string address = "https:/(엔드 포인트 삭제)/api/fox-bot-lm/send-text";
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
            Debug.Log("입력 영어로 : " + thisFox.intent);
            Debug.Log("봇 답변 영어 : " + thisFox.input_text);
        }

        Bot_displayText.text = thisFox.utterance;

        Input_Text_En_Log.text = "input: " + thisFox.intent;
        Output_Text_En_Log.text = "out: " + thisFox.input_text;
    }

    private void Clear_InputField()
    {
        var inputFieldPaernt = inputField.transform.parent.GetComponent<InputField>();
        inputFieldPaernt.text = "";
    }

    private void ShowLogText()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isShowLogText = !isShowLogText;
        }

        if (isShowLogText)
        {
            Input_Text_En_Log.gameObject.SetActive(true);
            Output_Text_En_Log.gameObject.SetActive(true);
        }
        else if (!isShowLogText)
        {
            Input_Text_En_Log.gameObject.SetActive(false);
            Output_Text_En_Log.gameObject.SetActive(false);
        }
    }
}