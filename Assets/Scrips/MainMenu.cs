using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenu;

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true); // ✅ corregido
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame() // ✅ buena práctica: PascalCase
    {
        SceneManager.LoadScene("Tutorial");
    }
}