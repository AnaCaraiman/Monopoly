using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

// ───── DUMMY CLASSES ─────
// (These dummy versions mimic the minimal API used by Player. 
//  Replace or remove them if your project already defines these classes.)

public enum MonopolyNodeType
{
    Property,
    Other
}

public class MonopolyNode
{
    public int price = 100;
    public int houseCost = 50;
    public int NumberOfHouses = 0;
    public bool IsMortgaged = false;
    public string name = "DummyNode";
    public MonopolyNodeType monopolyNodeType = MonopolyNodeType.Property;
    public int MortgageValue = 100;

    public void SetOwner(Player player)
    {
    }

    public void ResetNode()
    {
    }

    public void BuildHouseOrHotel()
    {
        NumberOfHouses++;
        if (NumberOfHouses > 5) NumberOfHouses = 5;
    }

    public int SellHouseOrHotel()
    {
        if (NumberOfHouses > 0)
        {
            NumberOfHouses--;
            return houseCost / 2;
        }

        return 0;
    }

    public int MortgageProperty()
    {
        IsMortgaged = true;
        return MortgageValue;
    }

    public void UnMortgageProperty()
    {
        IsMortgaged = false;
    }

    public void PlayerLandedOnNode(Player player)
    {
    }
}

public class PlayerInfo
{
    public string playerName;
    public int cash;
    public bool arrowActive = false;

    public void SetPlayerNameAndCash(string name, int cash)
    {
        playerName = name;
        this.cash = cash;
    }

    public void SetPlayerCash(int cash)
    {
        this.cash = cash;
    }

    public void ActivateArrow(bool active)
    {
        arrowActive = active;
    }
}

public class GameManager
{
    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (_instance == null) _instance = new GameManager();
            return _instance;
        }
        set { _instance = value; }
    }

    public Player GetCurrentPlayer;
    public bool RolledADouble = false;
    public bool HasRolledDice = false;

    public void ResetRolledADouble()
    {
        RolledADouble = false;
    }

    public void RemovePlayer(Player player)
    {
    }

    public void Continue()
    {
    }
}

public class MonopolyBoard
{
    private static MonopolyBoard _instance;

    public static MonopolyBoard instance
    {
        get
        {
            if (_instance == null) _instance = new MonopolyBoard();
            return _instance;
        }
        set { _instance = value; }
    }

    public List<MonopolyNode> route = new List<MonopolyNode>();

    public void MovePlayerToken(int distance, Player player)
    {
    }

    public (List<MonopolyNode>, bool) PlayerHasAllNodesOfSet(MonopolyNode node)
    {
        // For testing, simply return a set containing the node and "true"
        return (new List<MonopolyNode> { node }, true);
    }
}

public class TradingSystem
{
    private static TradingSystem _instance;

    public static TradingSystem instance
    {
        get
        {
            if (_instance == null) _instance = new TradingSystem();
            return _instance;
        }
        set { _instance = value; }
    }

    public void FindMissingProperty(Player player)
    {
    }
}

public class CommunityChest
{
    private static CommunityChest _instance;

    public static CommunityChest instance
    {
        get
        {
            if (_instance == null) _instance = new CommunityChest();
            return _instance;
        }
        set { _instance = value; }
    }

    public void AddBackJailFreeCard()
    {
    }
}

public class ChanceField
{
    private static ChanceField _instance;

    public static ChanceField instance
    {
        get
        {
            if (_instance == null) _instance = new ChanceField();
            return _instance;
        }
        set { _instance = value; }
    }

    public void AddBackJailFreeCard()
    {
    }
}

// ───── END DUMMY CLASSES ─────

[TestFixture]
public class PlayerEditModeTests
{
    private Player player;
    private PlayerInfo playerInfo;
    private MonopolyNode startingNode;

    [SetUp]
    public void Setup()
    {
        // (Reset the singleton instances so that Player’s dependencies are our dummy versions.)
        GameManager.instance = new GameManager();
        MonopolyBoard.instance = new MonopolyBoard();
        TradingSystem.instance = new TradingSystem();
        CommunityChest.instance = new CommunityChest();
        ChanceField.instance = new ChanceField();

        // Create a dummy starting node and a dummy token GameObject.
        startingNode = new MonopolyNode();
        playerInfo = new PlayerInfo();
        player = new Player();
        player.name = "TestPlayer";
        player.InitializePlayer(startingNode, 1000, playerInfo, new GameObject("Token"));
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(player.MyToken);
    }

    [Test]
    public void TestCollectMoney()
    {
        int initialMoney = player.ReadMoney;
        player.CollectMoney(200);
        Assert.AreEqual(initialMoney + 200, player.ReadMoney);
        Assert.AreEqual(initialMoney + 200, playerInfo.cash);
    }

    [Test]
    public void TestBuyProperty()
    {
        var property = new MonopolyNode();
        property.price = 200;
        int initialMoney = player.ReadMoney;
        player.BuyProperty(property);
        Assert.AreEqual(initialMoney - 200, player.ReadMoney);
        Assert.IsTrue(player.GetMonopolyNodes.Contains(property));
    }

    [Test]
    public void TestPayRent_SufficientFunds()
    {
        // Create an owner player.
        var owner = new Player();
        owner.name = "Owner";
        var ownerInfo = new PlayerInfo();
        owner.InitializePlayer(new MonopolyNode(), 1000, ownerInfo, new GameObject("OwnerToken"));

        int initialMoney = player.ReadMoney;
        int rentAmount = 100;
        player.PayRent(rentAmount, owner);
        Assert.AreEqual(initialMoney - rentAmount, player.ReadMoney);
        Assert.AreEqual(1000 + rentAmount, owner.ReadMoney);

        Object.DestroyImmediate(owner.MyToken);
    }

    [Test]
    public void TestPayMoney_SufficientFunds()
    {
        int initialMoney = player.ReadMoney;
        player.PayMoney(150);
        Assert.AreEqual(initialMoney - 150, player.ReadMoney);
    }

    [Test]
    public void TestGoToJail()
    {
        // Calling GoToJail should mark the player as "in jail."
        player.GoToJail(15);
        Assert.IsTrue(player.IsInJail);
    }

    [Test]
    public void TestIncreaseNumTurnsInJail()
    {
        int turns = player.NumTurnsInJail;
        player.IncreaseNumTurnsInJail();
        Assert.AreEqual(turns + 1, player.NumTurnsInJail);
    }

    [Test]
    public void TestCountHousesAndHotels()
    {
        // Create two dummy properties.
        var property1 = new MonopolyNode();
        var property2 = new MonopolyNode();
        // property1 has 3 houses; property2 is at “hotel” level (5 houses).
        property1.NumberOfHouses = 3;
        property2.NumberOfHouses = 5;
        player.addProperty(property1);
        player.addProperty(property2);
        int[] counts = player.CountHousesAndHotels();
        Assert.AreEqual(3, counts[0]); // houses
        Assert.AreEqual(1, counts[1]); // hotels
    }

    [Test]
    public void TestBuildHousesOrHotelEvenly()
    {
        // Create a set of two dummy properties with no houses.
        var property1 = new MonopolyNode() { houseCost = 50 };
        var property2 = new MonopolyNode() { houseCost = 50 };
        var properties = new List<MonopolyNode> { property1, property2 };

        // Give the player extra money to afford a house.
        player.CollectMoney(500);
        int initialMoney = player.ReadMoney;
        player.BuildHousesOrHotelEvenly(properties);

        // One of the properties should now have 1 house.
        bool houseBuilt = (property1.NumberOfHouses == 1 || property2.NumberOfHouses == 1);
        Assert.IsTrue(houseBuilt);
        Assert.Less(player.ReadMoney, initialMoney);
    }

    [Test]
    public void TestUnMortgageProperties()
    {
        // Create a dummy property that is mortgaged.
        var property = new MonopolyNode();
        property.IsMortgaged = true;
        player.addProperty(property);
        int cost = property.MortgageValue + (int)(property.MortgageValue * 0.1f);
        // Ensure the player has enough funds to unmortgage.
        player.CollectMoney(cost + 500);
        player.UnMortgageProperties();
        Assert.IsFalse(property.IsMortgaged);
    }

    [Test]
    public void TestUseChanceJailFreeCard()
    {
        // Use reflection to force the private "isInJail" field to true.
        var isInJailField = typeof(Player).GetField("isInJail",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        isInJailField.SetValue(player, true);

        player.AddChanceJailFreeCard();
        player.UseChanceJailFreeCard();
        Assert.IsFalse(player.IsInJail);
    }

    [Test]
    public void TestUseCommunityJailFreeCard()
    {
        // Force the player into jail.
        var isInJailField = typeof(Player).GetField("isInJail",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        isInJailField.SetValue(player, true);

        player.AddCommunityJailFreeCard();
        player.UseCommunityJailFreeCard();
        Assert.IsFalse(player.IsInJail);
    }
}