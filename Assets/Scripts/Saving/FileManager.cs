using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class FileManager
{
    public static string directory = "CubeRunnerData";
    public static string fileName = "cuberunner.dat";

    public static void Save(SaveObject save) {
        if (!DirectoryExists()) {
            Directory.CreateDirectory(GetDirectoryPath());
        }


        BinaryFormatter formatter = new BinaryFormatter();

        //opens file
        FileStream file = File.Create(GetFullPath());

        //saves info to file
        //TODO: make this create backup for redundancy
        formatter.Serialize(file, save);

        //closes file
        file.Close();
    }

    public static SaveObject Load() {
        if (SaveExists()) {
            try {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream file = File.Open(GetFullPath(), FileMode.Open);

                SaveObject save = (SaveObject) formatter.Deserialize(file);

                file.Close();
                return save;
            }
            catch (SerializationException) {
                Debug.Log("Failed to load save file.");
            }
        }
        return new SaveObject();
    }

    public static bool SaveExists() {
        return File.Exists(GetFullPath());
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
}
