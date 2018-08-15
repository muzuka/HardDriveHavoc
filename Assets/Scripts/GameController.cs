using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public int hardDriveSize;

    public float gameSwitchingTime;

    [Header("Life Images:")]
    public Image[] hearts;
    public Sprite emptyHeart;

    [Header("Object references:")]
    public Slider patienceSlider;

    public Text hardDriveSpaceText;
    public Text nextGameText;
    public Text currentGameText;

    public GameObject hardDriveTransform;
    public GameObject gameLibraryTransform;

    public GameObject hardDriveGameReference;
    public GameObject gameLibraryGameReference;

    public GameObject gameOverScreen;
    public GameObject pauseScreen;

    public GameObject tutorial;

    public Game nextGame { get; set; }
    public Game currentGame { get; set; }

    bool lookingForGame;
    bool switchingGames;

    float timeConsumed;

    int hardDriveFilledSpace;

    List<Game> gameLibrary;
    List<HardDriveGameController> hardDriveGames;

    int currentHealth;

	// Use this for initialization
	void Start () 
    {
        initializeLibrary();
        restart();
        Time.timeScale = 0f;
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pauseScreen.activeSelf)
            {
                pauseScreen.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                pauseScreen.SetActive(true);
                Time.timeScale = 0f;
            }

        }

        if (currentGame != null)
        {
            lookingForGame = !isGameDownloaded(currentGame.name);
        }

        if (switchingGames)
        {
            // Grace period between choosing a game and switching
            timeConsumed += Time.deltaTime;
            Debug.Log(timeConsumed);
            if (timeConsumed > gameSwitchingTime)
            {
                currentGame = nextGame;
                nextGame = getRandomGame();
                resetText(ref nextGameText);
                switchingGames = false;
                timeConsumed = 0f;
            }
        }
        else
        {
            // Player is choosing a game to play.
            Color currentColor = nextGameText.color;
            if (currentColor.a >= 0.9f)
            {
                switchingGames = true;
            }
            else
            {
                currentColor.a += Time.deltaTime / 5;
                nextGameText.color = currentColor;
            }

            if (lookingForGame)
            {
                // player cannot play the game
                patienceSlider.value += Time.deltaTime / 5;
                if (patienceSlider.value == 1f)
                {
                    hearts[currentHealth].sprite = emptyHeart;
                    currentHealth++;
                    patienceSlider.value = 0f;

                    if (currentHealth == 3)
                    {
                        gameOverScreen.SetActive(true);
                        Time.timeScale = 0f;
                    }
                }
            }
            else
            {
                patienceSlider.value -= Time.deltaTime / 100;
            }
        }

        hardDriveSpaceText.text = hardDriveFilledSpace.ToString() + " / " + hardDriveSize.ToString();

        if (nextGame != null)
        {
            nextGameText.text = nextGame.name;
        }
        if (currentGame != null)
        {
            currentGameText.text = currentGame.name;
        }
    }

    public void restart()
    {
        nextGame = getRandomGame();
        currentGame = null;
        resetText(ref nextGameText);

        lookingForGame = false;

        hardDriveGames = new List<HardDriveGameController>();
        hardDriveFilledSpace = 0;

        timeConsumed = 0f;
        currentHealth = 0;

        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
    }

    public void startGame()
    {
        Time.timeScale = 1f;
        tutorial.SetActive(false);
    }

    void initializeLibrary()
    {
        gameLibrary = new List<Game>();
        gameLibrary.Add(new Game("Battlefield Frontlines 4", 70));
        gameLibrary.Add(new Game("Bitcher 3", 25));
        gameLibrary.Add(new Game("Call of Doodoo: Black Opera 3", 100));
        gameLibrary.Add(new Game("Call of Doodoo: Infinity Wars", 100));
        gameLibrary.Add(new Game("Destineeeee", 50));
        gameLibrary.Add(new Game("Elderly Trolls Online", 80));
        gameLibrary.Add(new Game("Grandiose Theft Auto 5", 65));
        gameLibrary.Add(new Game("Near Automation", 50));
        gameLibrary.Add(new Game("New Doom", 70));
        gameLibrary.Add(new Game("Spyrim", 8));
        gameLibrary.Add(new Game("Uncharted Adventures", 60));

        foreach (Game game in gameLibrary)
        {
            GameObject libraryRef = Instantiate(gameLibraryGameReference, gameLibraryTransform.transform);
            libraryRef.GetComponent<GameEntryController>().game = game;
            libraryRef.GetComponent<GameEntryController>().controller = this;
            Text[] gameEntryTexts = libraryRef.GetComponentsInChildren<Text>();
            foreach (Text gameText in gameEntryTexts)
            {
                if (gameText.gameObject.name == "Title")
                {
                    gameText.text = game.name;
                }
                else if (gameText.gameObject.name == "Space Requirement")
                {
                    gameText.text = game.space.ToString() + " GB";
                }
            }
        }
    }

    public bool downloadGame(Game game)
    {
        if (hardDriveFilledSpace + game.space <= hardDriveSize)
        {
            GameObject hardDriveGame = Instantiate(hardDriveGameReference, hardDriveTransform.transform);
            hardDriveGames.Add(hardDriveGame.GetComponent<HardDriveGameController>());
            hardDriveGame.GetComponent<HardDriveGameController>().game = game;
            hardDriveGame.GetComponent<HardDriveGameController>().controller = this;
            hardDriveGame.GetComponent<HardDriveGameController>().download();
            hardDriveFilledSpace += game.space;
            return true;
        }
        else 
        {
            Debug.Log("Run out of space!");
            return false;
        }
    }

    public void deleteGame(Game game)
    {
        foreach (HardDriveGameController gameSection in hardDriveGames)
        {
            if (gameSection.game == game)
            {
                gameSection.delete();
                break;
            }
        }
    }

    public void removeGame(HardDriveGameController gameController)
    {
        hardDriveFilledSpace -= gameController.game.space;
        hardDriveGames.Remove(gameController);
    }

    public HardDriveGameController getGameFromHardDrive(string gameName)
    {
        foreach (HardDriveGameController gameSection in hardDriveGames)
        {
            if (gameSection.game.name == gameName)
            {
                return gameSection;
            }
        }
        return null;
    }

    public bool isGameDownloaded(string gameName)
    {
        HardDriveGameController controller = getGameFromHardDrive(gameName);

        if (controller == null)
        {
            return false;
        }

        return !controller.downloading || !controller.deleting;
    }

    public Game getRandomGame()
    {
        Random.InitState(System.Environment.TickCount);
        int randomGame = Random.Range(0, gameLibrary.Count - 1);
        return gameLibrary[randomGame];
    }

    void resetText(ref Text text)
    {
        Color transparent = text.color;
        transparent.a = 0f;
        text.color = transparent;
    }
}
