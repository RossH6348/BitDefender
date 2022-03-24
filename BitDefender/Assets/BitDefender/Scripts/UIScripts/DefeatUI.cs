using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

public class DefeatUI : MonoBehaviour
{

    [SerializeField] private MenuScript mainMenu;
    [SerializeField] private MainGameScript mainGame;
    [SerializeField] private SteamVR_LaserPointer laserPointer;

    [SerializeField] private GameObject ReturnButton;
    [SerializeField] private GameObject RestartButton;

    private Vector3 originalScale = Vector3.zero;

    //Enable pointer once more.
    void OnEnable()
    {

        transform.localScale = Vector3.zero;
        if (laserPointer.holder != null)
            laserPointer.holder.SetActive(true);
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

    public void onRestartClick()
    {
        mainGame.gameObject.SetActive(true);
        mainGame.beginLevel(mainGame.currentLevel);
        gameObject.SetActive(false);
    }
}
