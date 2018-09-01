using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEntryController : MonoBehaviour {

    public Button downloadButton;
    public Button deleteButton;

    public Color downloadedColor;
    public Color deletedColor;
    public Color errorColor;

    public Game game { get; set; }
    public GameController controller { get; set; }

    bool error;
    float timeConsumed;

    void Awake()
    {
        EventManager.StartListening("Restart", restart);
    }

    void Start()
    {
        timeConsumed = 0f;
        downloadButton.onClick.AddListener(() => {
            if (Time.timeScale == 1f)
            {
                if (controller.getGameFromHardDrive(game.name) == null)
                {
                    bool downloaded = controller.downloadGame(game);
                    if (downloaded)
                    {
                        GetComponent<Image>().color = downloadedColor;
                    }
                    else
                    {
                        GetComponent<Image>().color = errorColor;
                        error = true;
                    }
                }
            }
        });
        deleteButton.onClick.AddListener(() => {
            if (Time.timeScale == 1f)
            {
                controller.deleteGame(game);
                GetComponent<Image>().color = deletedColor;
            }
        });
    }

    void Update()
    {
        if (error)
        {
            timeConsumed += Time.deltaTime;
            if (timeConsumed >= 0.5f)
            {
                error = false;
                timeConsumed = 0f;
                GetComponent<Image>().color = deletedColor;
            }
        }
    }

    void restart()
    {
        GetComponent<Image>().color = deletedColor;
    }
}
