using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameScript : MonoBehaviour
{

    public MenuScript menu;

    //This dictionary will allow easy converting string from a file into spawning the appropriate enemy.
    private Dictionary<string, GameObject> enemies = new Dictionary<string, GameObject>();

    //Now apparently unity can't serializeField a dictionary, so these two lists are a workaround.
    [SerializeField] List<string> enemyKeys = new List<string>();
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();

    private bool gameInProgress = false; //Whether a level/game is in progress.
    private List<GameObject> Threats = new List<GameObject>(); //Array to contain all current alive enemies.

    private int Health = 100; //How much health does the CPU/Player have remaining?

    public void beginLevel(int level)
    {
        //Read from the level data file, all the commands that will occur during gameplay.
        string[] commands = System.IO.File.ReadAllLines(Application.dataPath + "/BitDefender/LevelData/Level" + level.ToString() + ".txt");

        //Close the main menu so it isn ot obstructing the player's view and gameplay.
        menu.closeMenu();
    }

    // Start is called before the first frame update
    void Start()
    {

        //Initialize the dictionary at startup, by going through the key and prefabs list as our workaround.
        for (int i = 0; i < Mathf.Min(enemyKeys.Count, enemyPrefabs.Count); i++)
            enemies.Add(enemyKeys[i], enemyPrefabs[i]);

    }

    // Update is called once per frame
    void Update()
    {
        


    }

}
