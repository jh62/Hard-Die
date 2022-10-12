using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject loadingText;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuValidate;
    [SerializeField] private AudioClip menuSelect;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void OnButtonOver(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void OnNewGameButonClicked()
    {
        gameObject.SetActive(false);
        loadingText.SetActive(true);
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }
}