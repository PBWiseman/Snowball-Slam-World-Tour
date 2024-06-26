/// <remarks>
/// Author: Chase Bennett-Hill
/// Date Created: May 7, 2024
/// Bugs: None known at this time.
/// </remarks>
/// <summary>
/// Manages the Joey Turret Object
/// </summary>
using UnityEngine;

public class JoeyTurret : MonoBehaviour
{
    public float expireTime = 15f; //The time before the ability expires
    public GameObject parent; //The parent object of the ability
    private float throwTime; //The time to throw the snowball
    private float delayTime = 0.75f; //The delay time between throws

    private float spawnTime; //The time the ability was spawned
    private Animator animator;
    public PlaySFX playSFX;

    [SerializeField] private ParticleSystem magicianPoof;

    // Start is called before the first frame update
    void Start()
    {
        playSFX = GetComponent<PlaySFX>();
        if(Physics.CheckBox(transform.position, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity)) 
        {
            
        }
        transform.position = new Vector3(transform.position.x, 0.65f, transform.position.z);
        spawnTime = Time.time; //Sets the spawn time to the current time
        animator = GetComponentInChildren<Animator>();
        magicianPoof.Play();
        playSFX.playSound("JoeySpawn");
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > throwTime + delayTime)
        {
            {
                transform.forward = (
                    new Vector3(
                        GetClosestPlayer().transform.position.x,
                        0,
                        GetClosestPlayer().transform.position.z
                    ) - new Vector3(transform.position.x, 0, transform.position.z)
                ).normalized; //Sets the forward direction of the agent to the direction to the player

                GetComponent<ThrowSnowballs>().ThrowSnowball(); //Throws a snowball
                throwTime = Time.time; //Sets the throw time to the current time
               // animator.SetTrigger("doThrow");
            }
        }
        if (Time.time - spawnTime >= expireTime || GameSettings.currentGameState == GameStates.PostGame) //If the ability has expired
        {
            playSFX.playSound("JoeyDespawn");
            animator.SetTrigger("doDespawn"); 
            //This needs to be triggered by the animation not instant
        }
    }

    /// <summary>
    /// Gets the closest player to the agent
    /// </summary>
    /// <returns>The closest player to the agent</returns>
    private GameObject GetClosestPlayer()
    {
        // state = EnemyStates.Searching; //Sets the state to searching
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); //Gets all the players in the scene
        GameObject closestPlayer = null; //Initializes the closest player to null
        float closestDistance = Mathf.Infinity; //Initializes the closest distance to infinity
        foreach (GameObject player in players) //Loops through all the players
        {
            float distance = Vector3.Distance(transform.position, player.transform.position); //Gets the distance between the agent and the player
            if (distance < closestDistance) //Checks if the distance is less than the closest distance
            {
                closestDistance = distance; //Sets the closest distance to the distance
                closestPlayer = player; //Sets the closest player to the player
            }
        }
        return closestPlayer; //Returns the closest player
    }
}
