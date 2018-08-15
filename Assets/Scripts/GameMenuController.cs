using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour {

    public void resume()
    {
        GameController controller = FindObjectOfType<GameController>();
        controller.pauseScreen.SetActive(false);
        Time.timeScale = 1f;
    }

	public void mainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void retry()
    {
        FindObjectOfType<GameController>().restart();
    }

    public void quit()
    {
        Application.Quit();
    }
}
