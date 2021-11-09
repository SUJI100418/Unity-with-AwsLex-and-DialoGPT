using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameButtonScript : MonoBehaviour
{
    TitleManager _titleManager;
    void Start()
    {
        _titleManager = FindObjectOfType<TitleManager>();

        if (_titleManager == null)
        {
            Debug.LogError("Game State Manager was not found!");
        }
    }

    public void OnClick()
    {
        _titleManager?.ExitGame();
    }
}
