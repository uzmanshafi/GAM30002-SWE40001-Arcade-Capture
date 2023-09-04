using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
   [Header("Levels To Load")]
   public string _nameGameLevel;
   private string levelToLoad;
   [SerializeField] private GameObject noSavedGameDialog = null;

   [Header("Volume Settings")]
   [SerializeField] private TMP_Text volumeTextValue = null;
   [SerializeField] private Slider volumeSlider = null;
   [SerializeField] private float defaultVolume = 0.5f;

   [SerializeField] private GameObject confirmationPrompt = null;

   public void NewGameDialogYes()
   {
        SceneManager.LoadScene(_nameGameLevel);
   }

   public void LoadGameDialogYes()
   {
        if(PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
   }

   public void ExitButton()
   {
        Application.Quit();
   }

   public void SetVolume(float volume)
   {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
   }

   public void ResetButton(string MenuType)
   {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            ApplyVolume();
        }
          
   }

   public void ApplyVolume()
   {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
   }

   public IEnumerator ConfirmationBox()
   {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
   }
}
