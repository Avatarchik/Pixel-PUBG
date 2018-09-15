using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;

class EditorSetting : EditorWindow
{
    private GameObject gameManager;
    private GameNetwork gameNetwork;
    private UnitZManager unitZmanager;

    [MenuItem("Window/UnitZ/Settings")]
    public static void ShowSetting()
    {
        EditorWindow.GetWindow(typeof(EditorSetting));
    }

    void loadData()
    {
        gameManager = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/UnitZ/Game/GameManager.prefab", typeof(GameObject));
        if (gameManager)
        {
            gameNetwork = gameManager.GetComponent<GameNetwork>();
            unitZmanager = gameManager.GetComponent<UnitZManager>();
        }
    }

    void OnEnable()
    {
        loadData();
    }

    void OnGUI()
    {
        titleContent.text = "Settings";

        if (gameManager == null)
            return;

        GUI.contentColor = Color.white;
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Server Name");
        gameNetwork.matchName = EditorGUILayout.TextField(gameNetwork.matchName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Game Port");
        gameNetwork.networkPort = EditorGUILayout.IntField(gameNetwork.networkPort);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Max Player");
        gameNetwork.maxConnections = EditorGUILayout.IntField(gameNetwork.maxConnections);
        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(gameManager);
        }

    }

}

