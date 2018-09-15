using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorTools : EditorWindow
{

    [MenuItem("Window/UnitZ/Tools/Copy Transform")]
    public static void showCopyTools()
    {
        EditorWindow.GetWindow(typeof(EditorTools));
    }
    public void CopyTransformsRecurse(Transform src, GameObject dst)
    {
        var components = (Component[])src.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(components[i]);
            if (dst.GetComponent(components[i].GetType()) == null)
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(dst.gameObject);
        }

        dst.transform.localScale = src.localScale;

        dst.transform.rotation = src.rotation;
        dst.transform.localRotation = src.localRotation;

        dst.transform.position = src.position;
        dst.transform.localPosition = src.localPosition;


        foreach (Transform child in dst.transform)
        {
            var curSrc = src.Find(child.name);
            if (curSrc)
            {
                CopyTransformsRecurse(curSrc, child.gameObject);
            }
        }
    }

    public void CopyComponentRecurse(Transform src, GameObject dst)
    {
        var components = (Component[])src.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(components[i]);
            if (dst.GetComponent(components[i].GetType()) == null)
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(dst.gameObject);
        }

        foreach (Transform child in dst.transform)
        {
            var curSrc = src.Find(child.name);
            if (curSrc)
            {
                CopyComponentRecurse(curSrc, child.gameObject);
            }
        }
    }
    GameObject source;
    GameObject target;
    void OnGUI()
    {
        titleContent.text = "Settings";
        source = (GameObject)EditorGUILayout.ObjectField("Source Transform", source, typeof(GameObject));
        target = (GameObject)EditorGUILayout.ObjectField("Target Transform", target, typeof(GameObject));


        if (GUILayout.Button("Copy All Transform"))
        {
            if (source != null && target != null)
            {
                CopyTransformsRecurse(source.transform, target);
            }
        }

        if (GUILayout.Button("Copy All Components"))
        {
            if (source != null && target != null)
            {
                CopyComponentRecurse(source.transform, target);
            }
        }
    }
}
