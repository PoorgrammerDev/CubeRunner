using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private SaveObject saveObject;

    private bool loaded = false;
    public int HighScore => saveObject.highScore;
    public int TotalBits => saveObject.totalBits;

    // Start is called before the first frame update
    void Awake() {
        saveObject = FileManager.Load();
        loaded = true;
    }

    public void Save() {
        if (!loaded) return;
        FileManager.Save(saveObject);
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
    public void AddBits (int value) {
        saveObject.totalBits += value;
    }

    public bool SubtractBits (int value) {
        if (value >= saveObject.totalBits) {
            saveObject.totalBits -= value;
            return true;
        }
        return false;
    }
}
