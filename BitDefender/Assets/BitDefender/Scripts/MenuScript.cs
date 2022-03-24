using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

public class MenuScript : MonoBehaviour
{

    //References to each submenus.
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject LevelMenu;
    [SerializeField] private GameObject HelpMenu;

    [SerializeField] private GameObject levelButtonTemplate;

    public MainGameScript mainGame;

    private Vector3 originalScale = Vector3.zero;
    private bool isClosed = false;

    private void Start()
    {
        //Store it's original localScale, we will be using it for transition.
        originalScale = gameObject.transform.localScale;

        //Reset the menu to ensure it always on the main menu.
        resetMenu();

        //Find all text files containing the game's level data, and create level buttons based on them.
        int totalLevel = 0;
        foreach(string file in System.IO.Directory.GetFiles(Application.dataPath + "/BitDefender/LevelData/","*.txt"))
        {
            GameObject levelButton = Instantiate(levelButtonTemplate, LevelMenu.transform);

            levelButton.transform.GetChild(0).GetComponent<Text>().text = (totalLevel + 1).ToString();

            float row = Mathf.Floor((float)totalLevel / 7);

            levelButton.transform.localPosition = new Vector3(
                (float)(totalLevel % 7) * 80.0f - 260.0f, 
                row * 80.0f + 120.0f,
                1.0f
            );

            levelButton.SetActive(true);
            totalLevel++;
        }
    }

    private void Update()
    {
        //This update function will just handle the transition.
        if (isClosed && gameObject.transform.localScale.magnitude > 0.0f)
        {
            gameObject.transform.localScale = Vector3.MoveTowards(gameObject.transform.localScale, Vector3.zero, Time.deltaTime * 0.02f);
        }
        else if(!isClosed && gameObject.transform.localScale.magnitude < originalScale.magnitude)
        {
            gameObject.transform.localScale = Vector3.MoveTowards(gameObject.transform.localScale, originalScale, Time.deltaTime * 0.02f);
        }
    }

    //This just resets the menu/go back to main menu.
    private void resetMenu()
    {
        LevelMenu.transform.localPosition = new Vector3(640.0f, 0.0f, 0.0f);
        HelpMenu.transform.localPosition = new Vector3(-640.0f, 0.0f, 0.0f);
        LevelMenu.SetActive(false);
        HelpMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    //Goes to the level menu.
    public void onLevelClick()
    {
        if (isClosed)
            return;

        LevelMenu.SetActive(true);
        MainMenu.SetActive(false);
        LevelMenu.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    //Goes to the help menu.
    public void onHelpClick()
    {
        if (isClosed)
            return;

        HelpMenu.SetActive(true);
        MainMenu.SetActive(false);
        HelpMenu.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    //Both back buttons on help and level menu, will redirect back to main menu.
    //Or rather just reset the entire thing.
    public void onBackClick()
    {
        if (isClosed)
            return;

        resetMenu();
    }

    //Self-explanatory.
    public void onExitClick()
    {
        if (isClosed)
            return;

        Application.Quit();
    }

    public void openMenu()
    {
        resetMenu();
        isClosed = false;
    }

    public void closeMenu()
    {
        isClosed = true;
    }

    //When a player select a level button, it will pass the text element object.
    public void onLevelSelected(Text buttonText)
    {

        if (isClosed)
            return;

        //Attempt to convert the level button's text to an integer, checking if there is no junk characters in it.
        //If successful then store the result to the level variable.
        int level = -1;
        if (int.TryParse(buttonText.text,out level))
        {
            //Now check if a level data file exists for this level number, if so we can then load the game.
            if (System.IO.File.Exists(Application.dataPath + "/BitDefender/LevelData/Level" + level.ToString() + ".txt"))
            {
                //Yes the file does exist, tell the maingame script to execute loading this level!
                mainGame.beginLevel(level);
            }
        }
    }
}
