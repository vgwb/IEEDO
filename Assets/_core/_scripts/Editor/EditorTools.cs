using UnityEngine;
using UnityEditor;

public class EditorTools : ScriptableObject
{
    [MenuItem("Tools/IEEDO/Revel Data Path")]
    static void RevelDataPath()
    {
        // EditorUtility.DisplayDialog("IEEDO", "Application Path: " + Application.persistentDataPath, "OK", "");
        // Debug.Log("persistentDataPath: " + Application.persistentDataPath);
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}
