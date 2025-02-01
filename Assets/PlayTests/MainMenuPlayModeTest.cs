using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Reflection;

public class MainMenuPlayModeTest
{
    private GameObject menuObject;
    private MainMenu mainMenu;

    [SetUp]
    public void Setup()
    {
        
        menuObject = new GameObject("MainMenu");
        mainMenu = menuObject.AddComponent<MainMenu>();

        
        FieldInfo playerSelectionField = typeof(MainMenu).GetField("playerSelection", BindingFlags.NonPublic | BindingFlags.Instance);
        
        MainMenu.PlayerSelect[] mockPlayers = new MainMenu.PlayerSelect[]
        {
            CreateMockPlayerSelect("Player1", 1, 2, true), 
            CreateMockPlayerSelect("Player2", 0, 1, false), 
            CreateMockPlayerSelect("Player3", 0, 0, true)  
        };

        playerSelectionField.SetValue(mainMenu, mockPlayers);

        
        GameSettings.settingsList.Clear();
    }

    private MainMenu.PlayerSelect CreateMockPlayerSelect(string name, int type, int color, bool isOn)
    {
        var playerSelect = new MainMenu.PlayerSelect
        {
            nameInput = new GameObject().AddComponent<TMP_InputField>(),
            typeDropdown = new GameObject().AddComponent<TMP_Dropdown>(),
            colorDropdown = new GameObject().AddComponent<TMP_Dropdown>(),
            toggle = new GameObject().AddComponent<Toggle>()
        };

        playerSelect.nameInput.text = name;
        playerSelect.typeDropdown.value = Mathf.Clamp(type, 0, 1);
        playerSelect.colorDropdown.value = Mathf.Clamp(color, 0, 3);
        playerSelect.toggle.isOn = isOn;
        
        return playerSelect;
    }

    [Test]
    public void StartButton_AddsOnlySelectedPlayersToGameSettings()
    {
        // Act
        mainMenu.StartButton();

        // Assert
        Assert.AreEqual(2, GameSettings.settingsList.Count, "Doar jucătorii selectați trebuie adăugați.");
        Assert.AreEqual("Player1", GameSettings.settingsList[0].playerName);
        Assert.AreEqual(0, GameSettings.settingsList[0].selectedType); 
        Assert.AreEqual(0, GameSettings.settingsList[0].selectedColor); 

        Assert.AreEqual("Player3", GameSettings.settingsList[1].playerName);
        Assert.AreEqual(0, GameSettings.settingsList[1].selectedType);
        Assert.AreEqual(0, GameSettings.settingsList[1].selectedColor);
    }

    [UnityTest]
    public IEnumerator StartButton_TriggersSceneChange()
    {
        // Listen for scene change
        bool sceneLoaded = false;
        SceneManager.sceneLoaded += (scene, mode) => { if (scene.name == "Game") sceneLoaded = true; };

        // Act
        mainMenu.StartButton();
        yield return new WaitForSeconds(0.5f);

        // Assert
        Assert.IsTrue(sceneLoaded, "SceneManager should load the Game scene.");
    }
}
