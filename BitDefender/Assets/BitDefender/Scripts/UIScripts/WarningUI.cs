using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

//Name: Ross Hutchins
//ID: HUT18001284

public class WarningUI : MonoBehaviour
{
    [SerializeField] private SteamVR_LaserPointer laserPointer;

    [SerializeField] private GameObject CloseButton;

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

    public void onCloseClick()
    {
        if (laserPointer.holder != null)
            laserPointer.holder.SetActive(false);
        Destroy(gameObject);
    }
}
