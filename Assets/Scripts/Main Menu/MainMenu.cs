using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour
{
    [Serializable]
    public class PlayerSelect
    {
        public TMP_InputField nameInput;
        public TMP_Dropdown typeDropdown;
        public TMP_Dropdown colorDropdown;
        public Toggle toggle;

    }

    [SerializeField] PlayerSelect[] playerSelection;

    public void StartButton()
    {
        foreach(var player in playerSelection)
        {
            if(player.toggle.isOn)
            {   
                Setting setting = new Setting(player.nameInput.text, player.typeDropdown.value, player.colorDropdown.value);
                GameSettings.AddSetting(setting);   
            }
        }
        SceneManager.LoadScene("Game");
    }
}
