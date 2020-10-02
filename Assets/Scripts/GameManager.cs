using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Properties

    [SerializeField]
    private GameObject mazePanel;
    [SerializeField]
    private GameObject winPanel;
    [SerializeField]
    private GameObject MazeObject;

    #endregion

    #region Monobehavior Constructor

    /// <summary>
    /// Standard method of Monobehaviour Script that is called at loading of script
    /// </summary>
    void Start()
    {
        // freeze time and dont show winpanel yet
        Time.timeScale = 0;
        winPanel.SetActive(false);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Method to make the player win the game and show the win menu
    /// </summary>
    public void Win()
    {
        // freeze time and show win
        Time.timeScale = 0;
        winPanel.SetActive(true);
    }

    /// <summary>
    /// Function for button press to unfreeze time and hide the menu
    /// </summary>
    public void Play()
    {
        // unfreeze time and let player reach end
        mazePanel.SetActive(false);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Function for button press to let player regenerate new maze after playing and hide/show menu's
    /// </summary>
    public void Retry()
    {
        // return menu
        winPanel.SetActive(false);
        mazePanel.SetActive(true);
        MazeObject.GetComponent<Maze>().GenerateMaze();
    }

    /// <summary>
    /// (Unused) function to physically quit the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    #endregion
}
