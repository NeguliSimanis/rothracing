using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer
{
    // RACER PROPERTIES
    public bool isAI = true;
    public string name;
    public float moveSpeed;

    public int racerCurrentHP;
    public int racerMaxHP;

    // RACE DATA
    public bool isAlive = true;
    public float raceFinishTime;
    public bool finishedRace = false;
    public int raceRank;
}

public class RaceManager : MonoBehaviour
{
    public List<Racer> raceParticipants = new List<Racer>();

    [SerializeField]
    private GameObject enemyBoat;
    private int participantCount;
    private int activeParticipants;
    private int nextAvailablePlace;

    [SerializeField]
    Transform[] enemySpawnLocations;

    // RACER NAMES
    public string playerName = "Weylam Roth";
    public string[] enemyNames =
    {
        "Cpt. Fairgraves",
        "Inquisitor Viril",
        "Isaac Perandus",
        "Blackguard Deserter"
    };

    #region INITIALIZING RACE
    public void InitializeRace()
    {
        nextAvailablePlace = 1;
        RegisterPlayerInRace();
        for (int i = 0; i < enemySpawnLocations.Length; i++) 
        {
            AddRandomRacer(enemySpawnLocations[i]);
        }
        participantCount = raceParticipants.Count;
        activeParticipants = participantCount;
        StartCoroutine(ManageEnemySpawning());
    }

    private void RegisterPlayerInRace()
    {
        Racer player = new Racer();
        player.name = playerName;
        player.isAI = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().InitializePlayer(player);
        raceParticipants.Add(player);
    }

    private void AddRandomRacer(Transform spawnLocation)
    {
        Racer newRacer = new Racer();
        newRacer.moveSpeed = Random.Range(85f, 115f);
        newRacer.racerMaxHP = 100;
        newRacer.racerCurrentHP = newRacer.racerMaxHP;
        raceParticipants.Add(newRacer);
        GameObject newRacerObject = Instantiate(enemyBoat, spawnLocation);
        newRacerObject.transform.parent = null;
        newRacerObject.transform.position = new Vector3(newRacerObject.transform.position.x, newRacerObject.transform.position.y, 0);
        newRacerObject.GetComponent<EnemyController>().InitializeEnemy(newRacer, enemySpawnLocations);
    }
    #endregion

    public void RegisterRacerFinishingRace(Racer racer, float finishTime)
    {
        racer.raceRank = nextAvailablePlace;
        nextAvailablePlace++;
        racer.finishedRace = true;
        racer.raceFinishTime = finishTime;
        activeParticipants--;
        if (activeParticipants <= 0)
            GameManager.instance.ShowGameResults();
    }



    private IEnumerator ManageEnemySpawning()
    {
        yield return new WaitForSeconds(
            Random.Range(GameManager.instance.minNewEnemySpawnDelay, GameManager.instance.maxNewEnemySpawnDelay));
        AddRandomRacer(enemySpawnLocations[Random.Range(0, enemySpawnLocations.Length)]);
        StartCoroutine(ManageEnemySpawning());
    }

}

