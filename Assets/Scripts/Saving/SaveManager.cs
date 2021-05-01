using UnityEngine;
using System.Collections.Generic;


public class SaveManager : MonoBehaviour
{
    public const int UPGRADES_MAX_PATHS = 2;
    [SerializeField] private SaveObject saveObject;

    private bool loaded = false;
    public int HighScore => saveObject.highScore;
    public int TotalBits => saveObject.totalBits;
    public string GameVersion => saveObject.gameVersion;

    // Start is called before the first frame update
    void Awake() {
        saveObject = FileManager.Load();
        loaded = true;

        //if array doesn't exist, initialize a new one
        if (saveObject.upgrades == null || saveObject.upgrades.Count == 0) {
            saveObject.upgrades = GetNewUpgradesArray();
        }
        //else, ensure the size is conformed
        else {
            SizeMatchUpgrades();
        }
        
        LoadLegacyStats();
        CheckVersion();
        Save();
    }

    // If the player has legacy-style stats of High Score and Total Bits from the old PlayerPref-based save-system,
    // load them into the new save system and remove the old PlayerPref keys
    void LoadLegacyStats() {
        int legacyHighScore = PlayerPrefs.GetInt(TagHolder.PREF_LEGACY_HIGH_SCORE, -1);
        int legacyTotalBits = PlayerPrefs.GetInt(TagHolder.PREF_LEGACY_TOTAL_BITS, -1);

        if (legacyHighScore != -1) {
            PlayerPrefs.DeleteKey(TagHolder.PREF_LEGACY_HIGH_SCORE);
            ContestHighScore(legacyHighScore);
        }

        if (legacyTotalBits != -1) {
            PlayerPrefs.DeleteKey(TagHolder.PREF_LEGACY_TOTAL_BITS);
            if (legacyTotalBits > TotalBits) {
                saveObject.totalBits = legacyTotalBits;
            }
        }
    }

    void CheckVersion() {
        if (saveObject.gameVersion != null && saveObject.gameVersion != Application.version) {
            //check versions and do stuff here...
        }

        saveObject.gameVersion = Application.version;
    }

    [ContextMenu("Save")]
    public void Save() {
        if (!loaded) return;
        FileManager.Save(saveObject);
    }

    //autosave function
    void OnApplicationQuit() {
        Save();
    }

    /*******************
    High Score Functions
    *******************/
    public bool ContestHighScore(int num) {
        if (num > saveObject.highScore) {
            saveObject.highScore = num;
            return true;
        }
        return false;
    }

    /*************
    Bits Functions
    *************/
    public void AddBits(int value) {
        saveObject.totalBits += value;
    }

    public bool SubtractBits(int value) {
        if (value <= saveObject.totalBits) {
            saveObject.totalBits -= value;
            return true;
        }
        return false;
    }

    /*************
    Upgrades Functions
    *************/
    public int GetUpgradeLevel(PowerUpType powerUpType, int upgIndex) {
        return saveObject.upgrades[(int) powerUpType].levels[upgIndex];
    }

    public void SetUpgradeLevel(PowerUpType powerUpType, int upgIndex, int level) {
        saveObject.upgrades[(int) powerUpType].levels[upgIndex] = level;
    }

    public void SetActivePath(PowerUpType powerUpType, int upgIndex) {
        saveObject.upgrades[(int) powerUpType].activePath = upgIndex;
    }

    public int GetActivePath(PowerUpType powerUpType) {
        return saveObject.upgrades[(int) powerUpType].activePath;
    }

    private List<UpgradeEntry> GetNewUpgradesArray() {
        List<UpgradeEntry> upgradesArray = new List<UpgradeEntry>();

        for (int i = 0; i < (int) PowerUpType.COUNT; i++) {
            upgradesArray.Add(new UpgradeEntry(UPGRADES_MAX_PATHS));
        }

        return upgradesArray;
    }

    private void SizeMatchUpgrades() {
        //if inner levels arrays are too small, expand them
        for (int i = 0; i < saveObject.upgrades.Count; i++) {
            int levelArrCount = saveObject.upgrades[i].levels.Count;
            if (levelArrCount < UPGRADES_MAX_PATHS) {
                saveObject.upgrades[i].levels.AddRange(new int[UPGRADES_MAX_PATHS - levelArrCount]);
            }
        }

        //if missing upgrade entry objects, add more
        int upgEntryCount = saveObject.upgrades.Count; 
        if (upgEntryCount < (int) PowerUpType.COUNT) {
            for (int i = 0; i < (int) PowerUpType.COUNT - upgEntryCount; i++) {
                saveObject.upgrades.Add(new UpgradeEntry(UPGRADES_MAX_PATHS));
            }
        }
    }
}

[System.Serializable]
public class SaveObject {
    public string gameVersion;
    public int highScore;
    public int totalBits;
    public List<UpgradeEntry> upgrades;
}

[System.Serializable]
public class UpgradeEntry {
    public UpgradeEntry(int listSize) {
        levels = new List<int>();
        levels.AddRange(new int[listSize]);

        activePath = 0;
    }

    public List<int> levels;
    public int activePath;
}