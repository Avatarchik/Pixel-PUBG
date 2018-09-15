using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

namespace CnControls
{
    public class ErrorPopupWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<ErrorPopupWindow>();
            window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 250, 133);
            window.titleContent = new GUIContent("ERROR");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField(
                "There's no EventSystem object in your scene (or it is disabled), CnControls won't work. Please add one using GameObject -> UI -> Event System menu or push the button below",
                EditorStyles.wordWrappedLabel);

            GUILayout.Space(12);
            if (GUILayout.Button("Create now!"))
            {
                EditorApplication.isPlaying = false;

                EditorApplication.playmodeStateChanged += OnPlaymodeChanged;
            }

            GUILayout.Space(6);

            if (GUILayout.Button("Close this window"))
            {
                Close();
            }
        }

        private void OnPlaymodeChanged()
        {
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.playmodeStateChanged -= OnPlaymodeChanged;

                var eventSystemGo = new GameObject("EventSystem");
                eventSystemGo.AddComponent<EventSystem>();
                eventSystemGo.AddComponent<StandaloneInputModule>();

                Close();
            }
        }
    }
}
