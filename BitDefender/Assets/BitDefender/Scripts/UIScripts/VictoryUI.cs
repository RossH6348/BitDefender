using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

//Name: Ross Hutchins
//ID: HUT18001284

public class VictoryUI : MonoBehaviour
{
    [SerializeField] private MenuScript mainMenu;
    [SerializeField] private MainGameScript mainGame;
    [SerializeField] private SteamVR_LaserPointer laserPointer;

    [SerializeField] private GameObject ReturnButton;
    [SerializeField] private GameObject NextButton;

    //Enable pointer once more.
    void OnEnable()
    {

        transform.localScale = Vector3.zero;
        if (laserPointer.holder != null)
            laserPointer.holder.SetActive(true);

        //Check if there is another leveldata file after the current level.
        //This determines which buttons need to be shown.
        if (System.IO.File.Exists(Application.dataPath + "/BitDefender/LevelData/Level" + (mainGame.currentLevel + 1).ToString() + ".txt"))
        {
            NextButton.SetActive(true);
            NextButton.transform.localPosition = new Vector3(160.0f, -160.0f, 0.0f);
            ReturnButton.transform.localPosition = new Vector3(-160.0f, -160.0f, 0.0f);
        }
        else
        {
            NextButton.SetActive(false);
            ReturnButton.transform.localPosition = new Vector3(0.0f, -160.0f, 0.0f);
        }

    }

    void Update()
    {
        gameObject.transform.localScale = Vector3.MoveTowards(gameObject.transform.localScale, new Vector3(0.005f, 0.005f, 0.005f), Time.deltaTime * 0.02f);
    }

    public void onReturnClick()
    {
        mainMenu.gameObject.SetActive(true);
        mainMenu.openMenu();
        gameObject.SetActive(false);
    }

    public void onNextClick()
    {
        mainGame.gameObject.SetActive(true);
        mainGame.beginLevel(mainGame.currentLevel + 1);
        gameObject.SetActive(false);
    }
}
