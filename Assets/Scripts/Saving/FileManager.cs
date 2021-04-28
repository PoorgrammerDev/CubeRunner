using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class FileManager
{
    public static string directory = "CubeRunnerData";
    public static string fileName = "cuberunner.dat";
    public static string backupFileName = "cuberunner.dat.bak";

    public static void Save(SaveObject save) {
        BinaryFormatter formatter = new BinaryFormatter();
        if (!DirectoryExists()) {
            Directory.CreateDirectory(GetDirectoryPath());
        }

        //copies to backup
        if (SaveExists()) {
            File.Copy(GetFullPath(), GetBackupPath(), true);
        }

        //opens file
        FileStream file = File.Create(GetFullPath());

        //saves info to file
        formatter.Serialize(file, save);

        //closes file
        file.Close();
    }

    public static SaveObject Load() {
        BinaryFormatter formatter = new BinaryFormatter();
        if (SaveExists()) {
            try {
                FileStream file = File.Open(GetFullPath(), FileMode.Open);
                SaveObject save = (SaveObject) formatter.Deserialize(file);

                file.Close();
                return save;
            }
            catch (SerializationException) {
                Debug.LogWarning("Failed to load save file.");
            }
        }
        
        if (BackupExists()) {
            Debug.Log("Attempting to load backup save file.");
            try {
                FileStream file = File.Open(GetBackupPath(), FileMode.Open);
                SaveObject save = (SaveObject) formatter.Deserialize(file);

                file.Close();
                return save;
            }
            catch (SerializationException) {
                Debug.LogWarning("Failed to load backup save file.");
            }
        }

        return new SaveObject();
    }

    public static bool SaveExists() {
        return File.Exists(GetFullPath());
    }

    private static bool BackupExists() {
        return File.Exists(GetBackupPath());
    }

    private static bool DirectoryExists() {
        return Directory.Exists(GetDirectoryPath());
    }

    private static string GetDirectoryPath() {
        return Path.Combine(Application.persistentDataPath, directory);
    }

    private static string GetFullPath() {
        return Path.Combine(GetDirectoryPath(), fileName);
    }

    private static string GetBackupPath() {
        return Path.Combine(GetDirectoryPath(), backupFileName);
    }
}
