using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

public class InkManager : MonoBehaviour
{ 
    public TextAsset _inkJsonAsset;
    private Story _story;

    public Text _textField;

    [SerializeField]
    private VerticalLayoutGroup _choiceButtonContainer;

    [SerializeField]
    private Button _choiceButtonPrefab;

    [SerializeField]
    private Color _normalTextColor;

    [SerializeField]
    private Color _thoughtTextColor;

    private CharacterManager _characterManager;

    [SerializeField]
    private GameObject _choiceButtonsObj;

    [SerializeField]
    private GameObject _dialogBoxObj;

    [SerializeField]
    private GameObject _playerInput;

    public Text inputField;

    public GameObject Timeline;

    private bool isInPuting = false;


    //== 변수 연결
    private int _relationshipStrength;

    public int RelationshipStrength
    {
        get => _relationshipStrength;
        private set
        {
            Debug.Log($"Updating RelationshipStrength value. Old value: {_relationshipStrength}, new value: {value}");
            _relationshipStrength = value;
        }
    }

    private int _mentalHealth;

    public int MentalHealth
    {
        get => _mentalHealth;
        private set
        {
            Debug.Log($"Updating MentalHealth value. Old value: {_mentalHealth}, new value: {value}");
            _mentalHealth = value;
        }
    }
    //== 변수 연결 끝

    private static string _loadedState;


    // Start is called before the first frame update
    void Start()
    {
        _characterManager = FindObjectOfType<CharacterManager>();

        StartStory();

        InitializeVariables();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && isInPuting)
        {
            if (inputField.text != "")
            {
                OnClickPlayerInput(inputField.text);
                inputField.text = "";
            }
        }
        /*else if(Input.GetKeyDown(KeyCode.Return) && !isInPuting)
        {
            DisplayNextLine();
        }*/
        else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))&& !isInPuting)
        {
            DisplayNextLine();
        }
    }

    public void StartStory()
    {
        _story = new Story(_inkJsonAsset.text);

        // == 변수 연결
        if (!string.IsNullOrEmpty(_loadedState))
        {
            _story?.state?.LoadJson(_loadedState);
            _loadedState = null;
        }
        // == 변수 연결 끝

        //== 캐릭터
        _story.BindExternalFunction("ShowCharacter",
            (string name, string position, string mood) => _characterManager.ShowCharacter(name, position, mood));

        _story.BindExternalFunction("HideCharacter",
            (string name) => _characterManager.HideCharacter(name));

        _story.BindExternalFunction("ChangeMood",
            (string name, string mood) => _characterManager.ChangeMood(name, mood));
        //== 캐릭터 끝
  
        DisplayNextLine();
        RefreshChoiceView();
    }

    private void InitializeVariables()  // 변수 연결 : 잉크에서 값이 변경될 때마다 값을 업데이트하는 일종의 리스너
    {
        /*
        RelationshipStrength = (int)_story.variablesState["relationship_strength"];
        MentalHealth = (int)_story.variablesState["mental_health"];

        _story.ObserveVariable("relationship_strength", (arg, value) =>
        {
            RelationshipStrength = (int)value;
        });

        _story.ObserveVariable("mental_health", (arg, value) =>
        {
            MentalHealth = (int)value;
        });
        */
    }

    public void DisplayNextLine()
    {
        if (_story.canContinue && !isInPuting)
        {
            string text = _story.Continue(); // gets next line

            text = text?.Trim(); // removes white space from text

            ApplyStyling();

            _textField.text = text; // displays new text
        }
        else if (_story.currentChoices.Count > 0)
        {
            DisplayChoices();
        }
    }

    private void ApplyStyling()
    {
        if (_story.currentTags.Contains("thought"))
        {
            _textField.color = new Color(_textField.color.r, _textField.color.g, _textField.color.b, 0.5f);
            _textField.fontStyle = FontStyle.Italic;
        }
        if (_story.currentTags.Contains("input"))
        {
            inputField.text = "";
            DisplayPlayerInput();
            isInPuting = true;
        }
        if (_story.currentTags.Contains("END"))
        {
            Timeline.transform.GetChild(0).gameObject.SetActive(true);
        }
        if (_story.currentTags.Contains("END_Happy"))
        {
            Timeline.transform.GetChild(3).gameObject.SetActive(true);
        }
        if (_story.currentTags.Contains("END_Sad"))
        {
            Timeline.transform.GetChild(4).gameObject.SetActive(true);
        }

        else
        {
            _textField.color = new Color(_textField.color.r, _textField.color.g, _textField.color.b, 1f);
            _textField.fontStyle = FontStyle.Normal;
        }
    }

    private void DisplayChoices()
    {
        showChoiceButtonsImg();

        // check if choices are already being displayed
        if (_choiceButtonContainer.GetComponentsInChildren<Button>().Length > 0) return;

        for (int i = 0; i < _story.currentChoices.Count; i++)
        {
            var choice = _story.currentChoices[i];
            var button = CreateChoiceButton(choice.text);

            button.onClick.AddListener(() => OnClickChoiceButton(choice));
        }
    }

    private void DisplayPlayerInput()
    {
        showChoicePlayerInput();
    }

    Button CreateChoiceButton(string text)
    {
        // creates the button from a prefab
        var choiceButton = Instantiate(_choiceButtonPrefab);

        choiceButton.transform.SetParent(_choiceButtonContainer.transform, false);

        // sets text on the button
        var buttonText = choiceButton.GetComponentInChildren<Text>();
        buttonText.text = text;

        return choiceButton;
    }

    void OnClickChoiceButton(Choice choice)
    {
        showDialogImg();
        _story.ChooseChoiceIndex(choice.index);
        _story.Continue();
        RefreshChoiceView();
        DisplayNextLine();
    }

    void OnClickPlayerInput(string txt)
    {
        //showDialogImg();
        Clear_InputField();
        _playerInput.transform.GetChild(0).gameObject.SetActive(false);  //인풋
        _dialogBoxObj.transform.GetChild(1).gameObject.SetActive(true);  //넥스트
        isInPuting = false;
        _story.Continue();
        DisplayNextLine();
    }

    // Destroys all the old content and choices.
    void RefreshChoiceView()
    {
        if (_choiceButtonContainer != null)
        {
            foreach (var button in _choiceButtonContainer.GetComponentsInChildren<Button>())
            {
                Destroy(button.gameObject);
            }
        }
    }

    //== 변수 연결
    public string GetStoryState()
    {
        return _story.state.ToJson();
    }

    public static void LoadState(string state)
    {
        _loadedState = state;
    }
    //== 변수 연결 끝

    public void showChoiceButtonsImg()
    {
        _choiceButtonsObj.GetComponent<Image>().enabled = true;
        _dialogBoxObj.transform.GetChild(0).gameObject.SetActive(false);
        return;
    }

    public void showDialogImg()
    {
        //_choiceButtonsObj.GetComponent<Image>().enabled = false;
        _dialogBoxObj.transform.GetChild(0).gameObject.SetActive(true); 
        return;
    }

    public void showChoicePlayerInput()
    {
        _playerInput.transform.GetChild(0).gameObject.SetActive(true);  //인풋필드
        _dialogBoxObj.transform.GetChild(1).gameObject.SetActive(false); //넥스트버튼
        return;
    }

    private void Clear_InputField()
    {
        var inputFieldPaernt = inputField.transform.parent.GetComponent<InputField>();
        inputFieldPaernt.text = "";
    }

}
