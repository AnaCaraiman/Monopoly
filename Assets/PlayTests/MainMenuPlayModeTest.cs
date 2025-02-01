using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class MainMenuPlayModeTest
{
    private GameObject settingsObject;
    private MainMenu mainMenu;

    [SetUp]
    public void Setup()
    {
        // Create a test instance of the SettingsObject
        settingsObject = new GameObject("SettingsObject");
        mainMenu = settingsObject.AddComponent<MainMenu>();

        // Create mock UI elements
        mainMenu.GetType().GetField("playerSelection", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(mainMenu, new MainMenu.PlayerSelect[]
            {
                CreateMockPlayerSelect("Player1", 1, 2, true), // AI, Red
                CreateMockPlayerSelect("Player2", 0, 1, false), // Human, Green (Not selected)
                CreateMockPlayerSelect("Player3", 0, 0, true)  // Human, Blue
            });

        // Clear GameSettings before each test
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
        playerSelect.typeDropdown.value = Mathf.Clamp(type, 0, 1); // Ensure type is 0 or 1
        playerSelect.colorDropdown.value = Mathf.Clamp(color, 0, 3); // Ensure color is between 0-3
        playerSelect.toggle.isOn = isOn;
        
        return playerSelect;
    }

    [Test]
    public void StartButton_AddsOnlySelectedPlayersToGameSettings()
    {
        // Act
        mainMenu.StartButton();

        // Assert
        Assert.AreEqual(2, GameSettings.settingsList.Count, "Only toggled players should be added.");
        Assert.AreEqual("Player1", GameSettings.settingsList[0].playerName);
        Assert.AreEqual(0, GameSettings.settingsList[0].selectedType); // AI
        Assert.AreEqual(0, GameSettings.settingsList[0].selectedColor); // Red

        Assert.AreEqual("Player3", GameSettings.settingsList[1].playerName);
        Assert.AreEqual(0, GameSettings.settingsList[1].selectedType); // Human
        Assert.AreEqual(0, GameSettings.settingsList[1].selectedColor); // Blue
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
