using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [HideInInspector]
    public GameState currentGameState;
    private RaceManager raceManager;
    private TileManager tileManager;
    private AudioManager audioManager;

    [Header("UI")]
    [SerializeField]
    private GameObject gameResultsPanel;
    [SerializeField]
    private GameObject racerResultWidgetContainer;
    [SerializeField]
    private GameObject racerResultWidget;
    [SerializeField]
    private GameObject mainMenuObject;

    [Header("HUD")]
    [SerializeField]
    private Text raceCountDownText;
    [SerializeField]
    private Text remainingPathTiles;

    public Vector3 finishLineCoordinates;

    private bool allowRestartingCouroutineActive = false;
    private bool allowRestartingWithAnyKey = false;

    #region LISTEN TO SCENE CHANGE
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeGame();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    private void Awake()
    {
        if (GameManager.instance != null)
            Destroy(gameObject);
        GameManager.instance = this;
        PlayerData.instance = new PlayerData();
        GetComponents();
    }

    private void GetComponents()
    {
        tileManager = gameObject.GetComponent<TileManager>();
        raceManager = gameObject.GetComponent<RaceManager>();
        audioManager = gameObject.GetComponent<AudioManager>();
    }

    private void Update()
    {
        ListenToPlayerInput();
    }

    private void ListenToPlayerInput()
    {
        if (Input.anyKey && currentGameState == GameState.MainMenu)
        {
            StartCoroutine(StartRaceCountdown());
        }
        if (currentGameState == GameState.GameOver && !allowRestartingCouroutineActive)
        {
            StartCoroutine(AllowRestartGameWithAnyKeyAfterDelay());
        }
        if (currentGameState == GameState.GameOver && Input.anyKeyDown && allowRestartingWithAnyKey)
        {
            RestartGame();
        }
    }

    private IEnumerator AllowRestartGameWithAnyKeyAfterDelay()
    {
        allowRestartingCouroutineActive = true;
        yield return new WaitForSeconds(1);
        allowRestartingWithAnyKey = true;
    }

    private void InitializeGame()
    {
        allowRestartingWithAnyKey = false;
        allowRestartingCouroutineActive = false;
        UpdateRemainingPathTilesText(tileManager.pathLength, tileManager.pathLength);
        currentGameState = GameState.MainMenu;
        tileManager.GenerateLevel();
        mainMenuObject.SetActive(true);
        gameResultsPanel.SetActive(false);
        raceCountDownText.gameObject.SetActive(false);
    }

    private IEnumerator StartRaceCountdown()
    {
        currentGameState = GameState.StartingRace;
        OpenMainMenu(false);
        raceManager.InitializeRace();
        raceCountDownText.gameObject.SetActive(true);
        raceCountDownText.text = "3";
        yield return new WaitForSeconds(1);
        raceCountDownText.text = "2";
        yield return new WaitForSeconds(1);
        raceCountDownText.text = "1";
        yield return new WaitForSeconds(1);
        audioManager.PlaySeaShanty();
        raceCountDownText.text = "GO!";
        currentGameState = GameState.Racing;
        yield return new WaitForSeconds(1f);
        raceCountDownText.gameObject.SetActive(false);
       

    }

    private void OpenMainMenu(bool open)
    {
        if (!open)
        {
            mainMenuObject.SetActive(false);
        }
    }

    public void ShowGameResults()
    {
        gameResultsPanel.SetActive(true);
        currentGameState = GameState.GameOver;

        int particapantsToDisplay = raceManager.raceParticipants.Count;
        int nextRankToDisplay = 1;

        while (particapantsToDisplay > 0)
        {
            foreach (Racer racer in raceManager.raceParticipants)
            {
                if (racer.raceRank == nextRankToDisplay)
                {
                    particapantsToDisplay--;
                    nextRankToDisplay++;

                    GameObject newParticipantInfo = Instantiate(racerResultWidget, racerResultWidgetContainer.transform);
                    ParticipantResult newParticpantResult = newParticipantInfo.GetComponent<ParticipantResult>();
                    newParticpantResult.racerRank.text = racer.raceRank.ToString();
                    newParticpantResult.racerName.text = racer.name;
                    newParticpantResult.racerTime.text = racer.raceFinishTime.ToString();
                }
                
            }
        }
    }

    public void RestartGame()
    {   
        SceneManager.LoadScene("Main");
    }

    public void UpdateRemainingPathTilesText(int remainingTiles, int maxTiles)
    {
        remainingPathTiles.text = remainingTiles + "/" + maxTiles;
    }

    public void RegisterParticipantReachingFinish(Racer participant, bool isPlayer = false)
    {
        raceManager.RegisterRacerFinishingRace(participant, Time.time);
        if (isPlayer)
            PlayerData.instance.canMove = false;
    }
}
