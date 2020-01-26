using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer
{
    // RACER PROPERTIES
    public bool isAI = true;
    public string name;
    public float moveSpeed;

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
        "Isaac Perandus"
    };

    #region INITIALIZING RACE
    public void InitializeRace()
    {
        nextAvailablePlace = 1;
        RegisterPlayerInRace();
        for (int i = 0; i < enemySpawnLocations.Length; i++) 
        {
            AddRandomRacer(enemySpawnLocations[i],i);
        }
        participantCount = raceParticipants.Count;
        activeParticipants = participantCount;
    }

    private void RegisterPlayerInRace()
    {
        Racer player = new Racer();
        player.name = playerName;
        player.isAI = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().InitializePlayer(player);
        raceParticipants.Add(player);
    }

    private void AddRandomRacer(Transform spawnLocation, int racerID)
    {
        Racer newRacer = new Racer();
        newRacer.name = enemyNames[racerID];
        newRacer.moveSpeed = Random.Range(85f, 115f);
        raceParticipants.Add(newRacer);
        GameObject newRacerObject = Instantiate(enemyBoat, spawnLocation);
        newRacerObject.GetComponent<EnemyController>().InitializeEnemy(newRacer);
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


}

