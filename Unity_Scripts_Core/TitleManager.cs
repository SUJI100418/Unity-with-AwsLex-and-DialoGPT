using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private InkManager _inkManager;
    private CharacterManager _characterManager;
    private SoundManager _soundManager;

    private bool isStartGame = false;
    private bool isTimeLinePlaying = true;

    GameObject title_to_main;
    public GameObject Fade_Image;

    private void Start()
    {
        _inkManager = FindObjectOfType<InkManager>();
        _characterManager = FindObjectOfType<CharacterManager>();
        _soundManager = FindObjectOfType<SoundManager>();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Title")
        {
            title_to_main = GameObject.Find("TitleToMain");
            if (title_to_main.transform.GetChild(0).gameObject.activeInHierarchy == false)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                isTimeLinePlaying = !isTimeLinePlaying;
            }

            PlayAndPauseTimeLine();
            //ShowTitleText();
        }
    }

    public void StartGame()
    {
        isStartGame = true;

        title_to_main = GameObject.Find("TitleToMain");

        title_to_main.GetComponent<PlayableDirector>().Play();

        //씬 전환 효과 추가 가능
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    public void SaveGame()
    {
        SaveData save = CreateSaveGameObject();

        var bf = new BinaryFormatter();

        var savePath = Application.persistentDataPath + "/savedata.save";

        FileStream file = File.Create(savePath);
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("Game saved");
    }

    private SaveData CreateSaveGameObject()
    {
        return new SaveData
        {
            InkStoryState = _inkManager.GetStoryState(),
            Characters = _characterManager.GetVisibleCharacters()
        };
    }

    public void LoadGame()
    {
        var savePath = Application.persistentDataPath + "/savedata.save";

        if (File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(savePath, FileMode.Open);
            file.Position = 0;

            SaveData save = (SaveData)bf.Deserialize(file);

            file.Close();

            InkManager.LoadState(save.InkStoryState);
            CharacterManager.LoadState(save.Characters);

            StartGame();
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }

    public void ExitGame()
    {
        Debug.Log("ExitGame");
        Application.Quit();
    }

    public void ShowTitleText()
    {
        if (title_to_main.transform.GetChild(1).gameObject.activeInHierarchy == false)
        {
            //Debug.Log("aa");
            isTimeLinePlaying = false;
            //UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
    }

    public void PlayAndPauseTimeLine()
    {
        if (isStartGame)
        {
            if (isTimeLinePlaying)
            {
                title_to_main.GetComponent<PlayableDirector>().Play();
            }
            else
            {
                title_to_main.GetComponent<PlayableDirector>().Pause();
            }
        }
    }
}
