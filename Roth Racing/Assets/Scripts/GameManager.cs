using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    bool skipCountdown = true;
        
    public static GameManager instance;
    [HideInInspector]
    public GameState currentGameState;
    private RaceManager raceManager;
    private TileManager tileManager;
    private AudioManager audioManager;

    [Header("BALANCE")]
    public float minNewEnemySpawnDelay;
    public float maxNewEnemySpawnDelay;

    [Header("UI")]
    [SerializeField]
    private GameObject gameResultsPanel;
    [SerializeField]
    private GameObject racerResultWidgetContainer;
    [SerializeField]
    private GameObject racerResultWidget;
    [SerializeField]
    private GameObject mainMenuObject;

    #region HUD
    [Header("HUD")]
    [SerializeField]
    private Text raceCountDownText;
    [SerializeField]
    private Text remainingPathTiles;
    [SerializeField]
    private Image healthBar;

    [Header("HUD - CANNON")]
    [SerializeField]
    private GameObject ammoIcon;
    [SerializeField]
    private Transform ammoIconParent;
    private List<GameObject> ammoIcons = new List<GameObject>();

    [Header("HUD - HP")]
    [SerializeField]
    private GameObject hpIcon;
    [SerializeField]
    private Transform hpIconParent;
    private List<GameObject> hpIcons = new List<GameObject>();

    [Header("HUD - RUM")]
    [SerializeField]
    private GameObject rumIcon;
    [SerializeField]
    private Transform rumIconParent;
    private List<GameObject> rumIcons = new List<GameObject>();
    #endregion


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
            //if (!skipCountdown)
            //   // StartCoroutine(StartRaceCountdown());
            //else
                SkipCountdown();
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

    //private IEnumerator StartRaceCountdown()
    //{
    //    currentGameState = GameState.StartingRace;
    //    OpenMainMenu(false);
    //    raceManager.InitializeRace();
    //    raceCountDownText.gameObject.SetActive(true);
    //    raceCountDownText.text = "3";
    //    yield return new WaitForSeconds(1);
    //    raceCountDownText.text = "2";
    //    yield return new WaitForSeconds(1);
    //    raceCountDownText.text = "1";
    //    yield return new WaitForSeconds(1);
    //    audioManager.PlaySeaShanty();
    //    raceCountDownText.text = "GO!";
    //    currentGameState = GameState.Racing;
    //    yield return new WaitForSeconds(1f);
    //    raceCountDownText.gameObject.SetActive(false);
    //}

    private void SkipCountdown()
    {
        currentGameState = GameState.StartingRace;
        OpenMainMenu(false);
        raceManager.InitializeRace();
        audioManager.PlaySeaShanty();
        raceCountDownText.text = "GO!";
        currentGameState = GameState.Racing;
        raceCountDownText.gameObject.SetActive(false);
    }

    public void ShowGameResults()
    {
        gameResultsPanel.SetActive(true);
        currentGameState = GameState.GameOver;
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

    #region UI METHODS
    private void OpenMainMenu(bool open)
    {
        if (!open)
        {
            mainMenuObject.SetActive(false);
        }
    }

    public void UpdatePlayerLifeHUD()
    {
        int currentIconCount = hpIconParent.childCount;
        if (PlayerData.instance.currLife > currentIconCount)
        {
            int missingIcons = PlayerData.instance.currLife - currentIconCount;
            for (int i = 0; i < missingIcons; i++)
            {
                Instantiate(hpIcon, hpIconParent);
            }
        }
        else
        {
            int surplusIcons = currentIconCount - PlayerData.instance.currLife;
            for (int i = 0; i < surplusIcons; i++)
            {
                Destroy(hpIconParent.transform.GetChild(0).gameObject);
            }
        }
    }

    public void UpdatePlayerAmmoHUD()
    {
        int currentIconCount = ammoIconParent.childCount;
        if (PlayerData.instance.currAmmo > currentIconCount)
        {
            int missingIcons = PlayerData.instance.currAmmo - currentIconCount;
            for (int i = 0; i < missingIcons; i++)
            {
                Instantiate(ammoIcon, ammoIconParent);
            }
        }
        else
        {
            int surplusIcons = currentIconCount - PlayerData.instance.currAmmo;
            for (int i = 0; i < surplusIcons; i++)
            {
                Destroy(ammoIconParent.transform.GetChild(0).gameObject);
            }
        }
    }
    #endregion

}
