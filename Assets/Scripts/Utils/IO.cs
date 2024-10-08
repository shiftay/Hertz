using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;

public class IO : MonoBehaviour
{
    private Player player;
    private static string Path => Application.persistentDataPath + "/data.txt";
    
    [Button("Path")]
    private void DataPath() {
        Debug.Log(Path);
    }

    [Button("Write Data")]
    private void WriteData()
    {
        string json = JsonUtility.ToJson(GameManager.instance.MAINPLAYER, true);
        File.WriteAllText(Path, json);
    }
  
    [ContextMenu("Read Data")]
    public Player ReadData()
    {
        string json = File.ReadAllText(Path);
        player = JsonUtility.FromJson<Player>(json);
      
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif

        return player;
    }
}
