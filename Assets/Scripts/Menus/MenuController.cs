using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

public class MenuController : MonoBehaviour
{
    //selected is updated by buttons when they become selected
    //public int selected;
    //camera that this can move around
    [SerializeField] private AsyncLoadManager ALM;
    [SerializeField] private MenuCamera menuCamera;
    //parent object containing all buttons from the main menu
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject settingsButtons;
    [SerializeField] private GameObject characterSelect;
    [SerializeField] private GameObject stageSelect;
    [SerializeField] private GameObject[] cursors;
    [SerializeField] private GameObject[] playerOptions;
    [SerializeField] private CharacterInfo[] characterInfos;
    [SerializeField] private GameObject readyText;
    private List<CharacterInfo> confirmedInfos = new List<CharacterInfo>();
    private SceneManager SM;
    /**
    0: Top Menu
    1: Settings
    2: Character Select
    3: Stage Select
    **/
    public int currentScreen = 0;
    int effectsVolume, musicVolume, commentaryVolume, commentaryFrequency;
    //MENU INTERFACES
    [SerializeField] private Slider effectsSlider, musicSlider, comVolumeSlider, comFreqSlider;
    [SerializeField] private TMP_Dropdown goreDropdown;
    [SerializeField] private Toggle screenshakeToggle;
    [SerializeField] private GameObject topFirstButton, settingsFirstButton, stageFirstButton;
    private int numPlayersConfirmed = 0;
    private bool amIConfirmed = false;
    public bool canMoveToGame = false;

    //tracks the stage the game will move to when it starts
    public int stageSelection;

    //sound
    AudioPlayer AP;

    //connected player images
    [SerializeField] private GameObject p1Connected;
    [SerializeField] private GameObject p2Connected;
    [SerializeField] private GameObject p3Connected;
    [SerializeField] private GameObject p4Connected;

    void Start() {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(topFirstButton);
        AP = GetComponent<AudioPlayer>();
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0)) {
        //    switch (selected) {
        //        //VERSUS MATCH
        //        case 0:
        //        menuCamera.goToVersusSetup();
        //        mainMenuButtons.SetActive(false);
        //        characterSelect.SetActive(true);
        //        break;

        //        //SETTINGS
        //        case 1:
        //        menuCamera.goToSettings();
        //        mainMenuButtons.SetActive(false);
        //        break;

        //        //QUIT GAME
        //        case 2:
        //        Debug.Log("Quitting game. Goodbye!");
        //        Application.Quit();
        //        break;

        //        default:
        //        Debug.Log("Error: unknown menu option");
        //        break;
        //    }
        //}

    }

    public void loadGameplay(int targetScene) {
        for (int i = 0; i < cursors.Length; i++)
            {
                string currentPlayer = "Player" + i;
                MenuCursor currentCursor = cursors[i].GetComponent<MenuCursor>();
                PlayerPrefs.SetInt(currentPlayer, currentCursor.playerSlot);
            }
            switch (targetScene) {
                case 0:
                    //Greece
                    ALM.BeginLoad("GreeceArena");
                    break;
                case 1:
                    //Canada
                    ALM.BeginLoad("CanadaArena");
                    break;
                case 2:
                    //Japan
                    ALM.BeginLoad("JapanArena");
                    break;
                case 3:
                    //Mexico
                    ALM.BeginLoad("MexicoStage");
                    break;
                case 4:
                    //Egypt
                    ALM.BeginLoad("EgyptArena");
                    break;
                default:
                   //Scene that hasn't been made yet
                    ALM.BeginLoad("GreeceArena");
                    break; 
            }
    }

    public void OptionSelect(int optionID)
    {
        //findAllCursors();
        switch (optionID)
        {
            //VERSUS MATCH
            case 0:
                currentScreen = 3;
                //menuCamera.goToVersusSetup();
                stageSelect.SetActive(true);
                mainMenuButtons.SetActive(false);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(stageFirstButton);
                //sound
                if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick2"));
                break;

            //SETTINGS
            case 1:
                currentScreen = 1;
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(settingsFirstButton);
                menuCamera.goToSettings();
                settingsButtons.SetActive(true);
                mainMenuButtons.SetActive(false);
                goreDropdown.value = PlayerPrefs.GetInt("goreMode", 0);
                musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 100) * 100;
                effectsSlider.value = PlayerPrefs.GetFloat("effectsVolume", 100) * 100;
                comVolumeSlider.value = PlayerPrefs.GetFloat("commentaryVolume", 100) * 100;
                comFreqSlider.value = PlayerPrefs.GetFloat("commentaryFrequency", 100) * 100;
                screenshakeToggle.isOn = (PlayerPrefs.GetInt("screenshake", 1) != 0);
                //sound
                if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick2"));
                break;

            //QUIT GAME
            case 2:
                Debug.Log("Quitting game. Goodbye!");
                Application.Quit();
                break;

            default:
                Debug.Log("Error: unknown menu option");
                break;
        }
    }

    public void returnToTop() {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(topFirstButton);
        menuCamera.goToMainMenu();
        currentScreen = 0;
        mainMenuButtons.SetActive(true);
        stageSelect.SetActive(false);
        settingsButtons.SetActive(false);

        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick2"));
    }

    public void findAllCursors() {
        cursors = GameObject.FindGameObjectsWithTag("MenuCursor");
    }

    public void characterSelected(int playerNumber, int playerSlot) {
        Debug.Log("Player " + playerNumber + " selected Character " + playerSlot);
        //todo: set it so playerNumber can control playerSlot's character options
        playerOptions[playerSlot].SetActive(true);
    }

    public void characterUnselected(int playerNumber, int playerSlot) {
        Debug.Log("Player " + playerNumber + " unselected Character " + playerSlot);
        //todo: reverse that thing from the last comment
        playerOptions[playerSlot].SetActive(false);
        //if (confirm)
        if (amIConfirmed) {
            unconfirmCharacter(playerSlot);
        }
    }

    public void confirmCharacter(int playerSlot) {
        amIConfirmed = true;
        confirmedInfos.Add(characterInfos[playerSlot]);
        characterInfos[playerSlot].confirm();
        numPlayersConfirmed++;
        Debug.Log("players confirmed: " + numPlayersConfirmed + " - Time: " + Time.fixedTime);
        Debug.Log("players needed: " + cursors.Length);
        if (numPlayersConfirmed == cursors.Length) {
            canMoveToGame = true;
            readyText.SetActive(true);
        }
    }

    public void unconfirmCharacter(int playerSlot) {
        amIConfirmed = false;
        confirmedInfos.Remove(characterInfos[playerSlot]);
        characterInfos[playerSlot].unconfirm();
        numPlayersConfirmed--;
        Debug.Log("players confirmed: " + numPlayersConfirmed + "Time: " + Time.fixedTime);
        if (canMoveToGame) {
            canMoveToGame = false;
            readyText.SetActive(false);
        }
    }

    public void moveToStageSelect() {
        currentScreen = 3;
        //findAllCursors();
        /**for (int i = 0; i < cursors.Length; i++) {
            if (cursors[i].GetComponent<MenuCursor>().playerNumber != 1) {
                cursors[i].SetActive(false);
            }
        }**/
        mainMenuButtons.SetActive(false);
        stageSelect.SetActive(true);

        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick2"));
    }

    /**public void backToCharSelect() {
        currentScreen = 2;
        findAllCursors();
        for (int i = 0; i < cursors.Length; i++) {
            cursors[i].SetActive(true);
        }
        Debug.Log("going back to character select");
        characterSelect.SetActive(true);
        stageSelect.SetActive(false);
    }**/

    public void backToStageSelect() {
        currentScreen = 3;
        /**findAllCursors();
        for (int i = 0; i < cursors.Length; i++) {
            cursors[i].SetActive(true);
        }**/
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(stageFirstButton);
        findAllCursors();
        for (int i = 0; i < cursors.Length; i++) {
            MenuCursor currentCursor = cursors[i].GetComponent<MenuCursor>();
            currentCursor.hideCursor();
            currentCursor.charConfirmed = false;
            currentCursor.deselect();
            currentCursor.Leave();
        }
        Debug.Log("going back to stage select");
        characterSelect.SetActive(false);

        GameObject[] playerHolders = GameObject.FindGameObjectsWithTag("PlayerHolder");
        for (int i = 0; i < playerHolders.Length; i++)
        {
            Destroy(playerHolders[i].gameObject);
        }

        stageSelect.SetActive(true);
        
        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClose"));
    }

    public void setGore() {
        PlayerPrefs.SetInt("goreMode", goreDropdown.value);

        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void setMusicVolume() {
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value / 100f);
    }

    public void setEffectsVolume() {
        PlayerPrefs.SetFloat("effectsVolume", effectsSlider.value / 100f);

        //sound
        if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void setCommentaryVolume() {
        PlayerPrefs.SetFloat("commentaryVolume", comVolumeSlider.value / 100f);

        //sound
        if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void setCommentaryFrequency() {
        PlayerPrefs.SetFloat("commentaryFrequency", comFreqSlider.value / 100f);

        //sound
        if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void setScreenshake() {
        if (screenshakeToggle.isOn) {
            PlayerPrefs.SetInt("screenshake", 1);
        } else {
            PlayerPrefs.SetInt("screenshake", 0);
        }

        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }


    public void selectStage(int stageID) {
        stageSelection = stageID;
        currentScreen = 2;
        stageSelect.SetActive(false);
        characterSelect.SetActive(true);
        findAllCursors();
        for (int i = 0; i < cursors.Length; i++) {
            cursors[i].GetComponent<MenuCursor>().findCharSelectItems();
            cursors[i].GetComponent<MenuCursor>().showCursor();
        }

        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuOpen"));
    }

    public void showConnected(int playerNumber)
    {
        switch (playerNumber)
        {
            case 1:
                p1Connected.SetActive(true);
                break;
            case 2:
                p2Connected.SetActive(true);
                break;
            case 3:
                p3Connected.SetActive(true);
                break;
            case 4:
                p4Connected.SetActive(true);
                break;
            default:
                Debug.Log("Player Num Error");
                break;
        }
    }

    public void hideConnected(int playerNumber)
    {
        switch (playerNumber)
        {
            case 1:
                p1Connected.SetActive(false);
                break;
            case 2:
                p2Connected.SetActive(false);
                break;
            case 3:
                p3Connected.SetActive(false);
                break;
            case 4:
                p4Connected.SetActive(false);
                break;
            default:
                Debug.Log("Player Num Error");
                break;
        }
    }
}
