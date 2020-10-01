using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mazePanel;
    [SerializeField]
    private GameObject winPanel;
    [SerializeField]
    private GameObject MazeObject;


    // Start is called before the first frame update
    void Start()
    {
        // freeze time and dont show winpanel yet
        Time.timeScale = 0;
        winPanel.SetActive(false);
    }

    public void Win()
    {
        // freeze time and show win
        Time.timeScale = 0;
        winPanel.SetActive(true);
    }

    public void Play()
    {
        // unfreeze time and let player reach end
        mazePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void Retry()
    {
        // return menu
        winPanel.SetActive(false);
        mazePanel.SetActive(true);
        MazeObject.GetComponent<Maze>().GenerateMaze();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
