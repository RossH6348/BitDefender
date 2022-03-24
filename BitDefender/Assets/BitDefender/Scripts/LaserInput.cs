using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

//Thank you to valentin-simian for this code of making the laser pointer able to interact with the UI elements.
//Credit goes to this guy instead, not me, I only made very slight modifications.
//https://answers.unity.com/questions/1600790/how-to-implement-steamvr-laser-pointer.html
public class LaserInput : MonoBehaviour
{
    private SteamVR_LaserPointer laserPointer;
    private bool selected = false;
    // Start is called before the first frame update
    void Start()
    {

        laserPointer = GetComponent<SteamVR_LaserPointer>();

        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
        selected = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {

        Button button = e.target.GetComponent<Button>();
        if (button != null && selected)
            button.onClick.Invoke();

    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        Button button = e.target.GetComponent<Button>();
        if (button != null && !selected)
        {
            button.image.color = button.colors.selectedColor;
            selected = true;
        }
    }
    public void PointerOutside(object sender, PointerEventArgs e)
    {
        Button button = e.target.GetComponent<Button>();
        if (button != null && selected)
        {
            button.image.color = button.colors.normalColor;
            selected = false;
        }
    }
}
