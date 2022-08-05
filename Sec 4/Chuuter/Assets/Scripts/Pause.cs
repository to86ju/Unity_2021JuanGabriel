using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    public Button exitButton;

    public AudioMixerSnapshot pausaSnp, gameSnp;

    private void Awake()
    {
        pauseMenu.SetActive(false);
        exitButton.onClick.AddListener(ExitGame);
    }

    // Update is called once per frame
    void Update()
    {
        //si pulsamos la tecla q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            pauseMenu.SetActive(true);
            Time.timeScale = 0;//Parar el juego

            pausaSnp.TransitionTo(0.1f);

        }
    }

    //funcion para poder volver al juego
    public void ResumenGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        gameSnp.TransitionTo(0.1f);
    }

    //funcion para salir de juego
    private void ExitGame()
    {
        print("Ejecucion finalizada");
        Application.Quit();
    }
}
