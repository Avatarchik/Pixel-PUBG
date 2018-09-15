using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUICharacterPresetLoader : MonoBehaviour
{

    public RectTransform CharacterPresetPrefab;
    public Transform Canvas;
    public RectTransform View;
    public Text Description;
    public bool ChangeCharacter;

    void Start()
    {
        Draw();
    }

    void ClearCanvas()
    {
        if (Canvas == null)
            return;

        foreach (Transform child in Canvas.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void Draw()
    {
        if (Canvas == null || CharacterPresetPrefab == null || View == null)
            return;

        if (UnitZ.levelManager != null)
        {
            ClearCanvas();
            float offset = 0;
            RectTransform rootRect = Canvas.gameObject.GetComponent<RectTransform>();
            float width = (CharacterPresetPrefab.sizeDelta.x + 5) * UnitZ.characterManager.CharacterPresets.Length;
            if (width < View.rect.width)
            {
                offset = ((View.rect.width) - width) / 2;
            }
            for (int i = 0; i < UnitZ.characterManager.CharacterPresets.Length; i++)
            {
                GameObject obj = (GameObject)GameObject.Instantiate(CharacterPresetPrefab.gameObject, Vector3.zero, Quaternion.identity);
                obj.transform.SetParent(Canvas.transform);

                GUICharacterPreset character = obj.GetComponent<GUICharacterPreset>();
                if (character)
                {
                    character.GUIImage.texture = UnitZ.characterManager.CharacterPresets[i].Icon;
                    character.Name.text = UnitZ.characterManager.CharacterPresets[i].Name;
                    character.Description = UnitZ.characterManager.CharacterPresets[i].Description;
                    character.Index = i;
                    character.changeCharacter = ChangeCharacter;
                }

                RectTransform rect = obj.GetComponent<RectTransform>();
                if (rect)
                {
                    rect.anchoredPosition = new Vector2(offset + ((CharacterPresetPrefab.sizeDelta.x + 5) * i) + (CharacterPresetPrefab.sizeDelta.x / 2), 5);
                    rect.localScale = CharacterPresetPrefab.gameObject.transform.localScale;
                }
            }
            rootRect.sizeDelta = new Vector2(offset + offset + (CharacterPresetPrefab.sizeDelta.x + 5) * UnitZ.characterManager.CharacterPresets.Length, rootRect.sizeDelta.y);
        }
    }
}
