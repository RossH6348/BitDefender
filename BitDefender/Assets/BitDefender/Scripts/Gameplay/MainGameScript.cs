using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

//Name: Ross Hutchins
//ID: HUT18001284

public class MainGameScript : MonoBehaviour
{
    //Camera reference, for spawning enemies right infront of them.
    [SerializeField] private Transform VRCamera;

    //We will need to reference the two hands, so we can forcibly detach the shield of pistol from the player if they die or win.
    [SerializeField] private Hand LHand;
    [SerializeField] private Hand RHand;

    //References to menus and screens.
    [SerializeField] private MenuScript menu;
    public GameObject victoryScreen;
    public GameObject defeatScreen;

    //This dictionary will allow easy converting string from a file into spawning the appropriate enemy.
    private Dictionary<string, GameObject> enemies = new Dictionary<string, GameObject>();

    //Now apparently unity can't serializeField a dictionary, so these two lists are a workaround.
    [SerializeField] List<string> enemyKeys = new List<string>();
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();

    public List<GameObject> Threats = new List<GameObject>(); //Array to contain all current alive enemies.

    private bool gameInProgress = false; //Is the game still in progress?
    private int Health = 100; //How much health does the CPU/Player have remaining?
    public int MaxHealth = 100; //How much health can the CPU/Player have?
    public float spherePlayArea = 12.5f; //How large is the play area for the viruses and such?

    [SerializeField] private GameObject laserPistol;
    [SerializeField] private GameObject firewallShield;
    [SerializeField] private Transform playerAim; //Where is the player currently pointing at? This will be used for Spyware, such as causing viruses to move away from player's aim.
    public Transform aimPoint; //Where on the sphere play area is the player aiming at? This will be extremely useful for making viruses scram in present of spyware.


    //All HUD elements.
    [SerializeField] private List<Transform> HealthBar = new List<Transform>();
    [SerializeField] private List<Text> LowHealthText = new List<Text>();
    private float FlashPulse = 0.0f;


    // Start is called before the first frame update
    void Start()
    {

        //Initialize the dictionary at startup, by going through the key and prefabs list as our workaround.
        for (int i = 0; i < Mathf.Min(enemyKeys.Count, enemyPrefabs.Count); i++)
            if(!enemies.ContainsKey(enemyKeys[i]) && enemyPrefabs[i] != null)
                enemies.Add(enemyKeys[i], enemyPrefabs[i]);

    }

    // Update is called once per frame
    void Update()
    {

        //Clamp the ratio value so it never goes into the negatives.
        float ratio = Mathf.Max((float)Health / (float)MaxHealth, 0.0f);

        for (int i = HealthBar.Count - 1; i > -1; i--)
            HealthBar[i].localScale = new Vector3(ratio, 1.0f, 1.0f);
        
        if (ratio <= 0.3334f) //Player is at 1/3 or lower, start flashing Warning text.
            FlashPulse = (FlashPulse + 360.0f * Time.deltaTime) % 360.0f;
        else
            FlashPulse = 0.0f;

        //Change alpha based on flashpulse which is just an angle going from 0 to 360, so 0 and 180 is completely transparent and 90 and 270 is opaque.
        //I am using the absolute function to forcibly change the negative values into a positive one instead.
        for (int i = LowHealthText.Count - 1; i > -1; i--)
            LowHealthText[i].color = new Color(LowHealthText[i].color.r, LowHealthText[i].color.g, LowHealthText[i].color.b, Mathf.Sin(Mathf.Abs(FlashPulse * Mathf.Deg2Rad)));

        if (gameInProgress)
        {
            //Constantly update where on the sphere the player is aiming at, this is for the viruses to make use of.
            aimPoint.position = BitDefender.GlobalMathFunctions.RaySphere(playerAim, Vector3.zero, spherePlayArea);
            aimPoint.LookAt(transform);

            //Animate the laser and shield appearing, upon level starting.
            if (laserPistol.transform.localScale.x < 1.0f)
                laserPistol.transform.localScale = firewallShield.transform.localScale = Vector3.MoveTowards(laserPistol.transform.localScale, new Vector3(1.0f, 1.0f, 1.0f), 5.0f * Time.deltaTime);

            //Keep cycling through the list, getting rid of deleted viruses.
            for (int i = Threats.Count - 1; i > -1; i--)
                if (Threats[i] == null)
                    Threats.RemoveAt(i);
        }
    }

    //This function will handle damaging the player, based on whether it is a projectile or an enemy.
    public void Damage(GameObject Threat)
    {
        ThreatAI ai = Threat.GetComponent<ThreatAI>();
        if (ai != null)
        {
            ai.state = ThreatState.Destroyed;
            Health -= ai.DMG;
        }
        else
        {
            //Doesnt have threatAI, maybe it might be DDOS' projectiles.
            DDOSProjectile DDOS = Threat.GetComponent<DDOSProjectile>();
            if (DDOS != null)
            {
                Health -= DDOS.DMG;
                Destroy(Threat);
            }
        }

        if (Health <= 0)
            gameInProgress = false; //The CPU is now infected, end game here immediately.
    }


    //This will load in the level data's commands, and keep track of what level the player is on.
    public int currentLevel = 0;
    private List<string> commands = new List<string>();
    public void beginLevel(int level)
    {

        //Record which level, incase the player wants to restart or progress to next level.
        currentLevel = level;

        //Read from the level data file, all the commands that will occur during gameplay.
        commands = new List<string>(System.IO.File.ReadAllLines(Application.dataPath + "/BitDefender/LevelData/Level" + level.ToString() + ".txt"));

        //Clear all enemies that may still remain in the system.
        for (int i = Threats.Count - 1; i > -1; i--)
        {
            if (Threats[i] != null)
                Destroy(Threats[i]);
            Threats.RemoveAt(i);
        }

        //Reset both the shield and pistol.
        laserPistol.transform.SetPositionAndRotation(new Vector3(0.8f, 1.0f, 0.0f), Quaternion.identity);
        firewallShield.transform.SetPositionAndRotation(new Vector3(-0.8f, 1.0f, 0.0f), Quaternion.identity);
        laserPistol.transform.localScale = firewallShield.transform.localScale = Vector3.zero;

        //Close the main menu so it isn ot obstructing the player's view and gameplay.
        menu.closeMenu();

        //Set player health back to max.
        Health = MaxHealth;

        //We can now change gameInProgress to true, to allow Update to do some game logics.
        gameInProgress = true;
        StartCoroutine("Commander");
    }

    IEnumerator Commander()
    {

        yield return new WaitForSeconds(5.0f); //Wait at least 5 seconds for actually beginning to carry out the commands.

        while (gameInProgress && (commands.Count > 0 || Threats.Count > 0))
        {

            if (commands.Count > 0)
            {
                if (commands[0].Equals("WAIT"))
                {
                    if (Threats.Count > 0)
                        yield return new WaitForSeconds(1.0f); //Check again in 1 seconds time that they cleared the wave?
                    else
                        commands.RemoveAt(0); //Delete the command, we are done with it now.
                }
                else
                {
                    //WAIT is the only command where it doesn't have parameters, the rest does so we need to split the commands[0] into parameters.
                    string[] parameters = commands[0].Split(' ');
                    if (parameters[0].Equals("DELAY"))
                    {
                        yield return new WaitForSeconds(float.Parse(parameters[1]));
                        commands.RemoveAt(0); //Go ahead and delete this command afterward.
                    }
                    else
                    {
                        //WAIT and DELAY are the only commands that isn't spawning enemies, so we can safely just use the dictionary at this point.
                        //The first two parameters for spawning goes as followed: [AMOUNT] [TYPE/NAME] [RANDOM]

                        //We can use TYPE/NAME as a key for our dictionary to get the equivalent prefab.
                        //[RANDOM] determines if we should spawn each one in random locations, elsewise in a cluster.

                        //Convert the first parameter to an integer.
                        int amount = int.Parse(parameters[0]);

                        //Convert the 3rd parameter to a boolean.
                        bool randomized = (parameters[2].Equals("TRUE") ? true : false); //The 2nd parameter could be VIRUS,SPYWARE,DDOS and so on due to the new Warning dialog.

                        //Prepare a origin point to spawn enemies from, incase of a cluster type.
                        Vector3 originPoint = new Vector3(Random.Range(-180.0f, 180.0f), Random.Range(-20.0f, 45.0f), spherePlayArea);
                        if (parameters[1].Equals("SPYWARE"))
                            originPoint = BitDefender.GlobalMathFunctions.XYZToLatLong(BitDefender.GlobalMathFunctions.RaySphere(VRCamera, transform.position, spherePlayArea)); //Make spyware or warning spawn right infront of the player initially.

                        //Convert the 1st parameter to an integer, and begin for loop. After seeing if this type of enemy exists within the database.
                        if (enemies.ContainsKey(parameters[1]))
                        {
                            for (int i = amount; i > 0; i--)
                            {
                                GameObject newEnemy = Instantiate(enemies[parameters[1]], transform);

                                //Get the AI of this new spawned enemy, and give it a random position.
                                ThreatAI ai = newEnemy.GetComponent<ThreatAI>();
                                if (ai != null)
                                {
                                    Vector3 spawnPos = originPoint;
                                    if (randomized)
                                    {
                                        //Nevermind... Command dictates that we place them randomly.
                                        spawnPos = new Vector3(Random.Range(-180.0f, 180.0f), Random.Range(-20.0f, 45.0f), spherePlayArea);
                                    }
                                    else if (amount > 1) //Only do cluster, if it more than 1 enemy spawning in one go.
                                    {
                                        //Figure out the angle step, (so 360 / amount) times by i, followed by converting it to radians.
                                        float angle = ((360.0f / (float)amount) * (float)i) * Mathf.Deg2Rad;
                                        spawnPos.x += Mathf.Sin(angle) * 5.0f;
                                        spawnPos.y += Mathf.Cos(angle) * 5.0f;
                                    }

                                    //Set position of this new enemy.
                                    ai.setPosition(spawnPos);
                                }
                                else
                                {
                                    //It is a dialog type, since it doesn't have a ThreatAI attached to it.
                                    //Find the correct component to show.
                                    Transform uiPanel = newEnemy.transform.GetChild(0).transform.Find(parameters[2]);
                                    if (uiPanel != null)
                                        uiPanel.gameObject.SetActive(true);

                                    //Move the Warning dialog down relative to the MainGame gameObject.
                                    newEnemy.transform.position -= transform.position;
                                }

                                //Lastly, add this new enemy to the list of Threats.
                                Threats.Add(newEnemy);

                                //Alright, we can activate this enemy, cuz it is ready to start infesting!
                                newEnemy.SetActive(true);
                            }
                        }
                        commands.RemoveAt(0);
                    }
                }
            }
            else
            {
                yield return new WaitForSeconds(0.1f); //Wait to check in 0.1 seconds time again, to see if they cleared all threats, or ran out of health.
            }
        }

        if (gameInProgress)
            victoryScreen.SetActive(true);
        else
            defeatScreen.SetActive(true);

        //Before we deactivate the MainGame, we need to detach the laser pistol and shield from the player and put it back in the main game.
        //Assuming the player is holding in either of their hands.
        LHand.DetachObject(LHand.currentAttachedObject);
        RHand.DetachObject(RHand.currentAttachedObject);

        //Disable the maingame object.
        gameObject.SetActive(false);

        yield break;
    }

}
