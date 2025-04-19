using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // On-click behavior for the Play Button.
    // May need to make a new panel depending on how we want the game to be customizable (e.g. difficulties)
    // before sending the player to the main scene.
    public void PlayButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    // On-click behavior for the Quit Button.
    public void QuitButton()
    {
        Application.Quit();
    }
}
