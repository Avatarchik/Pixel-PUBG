using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//创建角色面板
public class CharacterCreatorCanvas : MonoBehaviour
{

    public RectTransform CharacterBadgePrefab;
    public RectTransform Canvas;
    [HideInInspector]
    public CharacterSaveData[] Characters;
    private Vector2 scrollPosition;
    private int indexCharacter = 0;
    public Transform previewSpot;
    private GameObject characterPreviewer;
    [HideInInspector]
    public CharacterSaveData characterLoaded;

    void Start()
    {
        //加载所有角色
        Setup();
        StartCoroutine(LoadCharacters());
    }
    public void Setup()
    {
        ClearCanvas();
        indexCharacter = PlayerPrefs.GetInt("INDEX_CRE_CHAR");
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

    public IEnumerator LoadCharacters()
    {
        if (UnitZ.playerSave)
        {
            //从存档加载所有角色
            Characters = UnitZ.playerSave.LoadAllCharacters();

            while (Characters == null)
            {
                yield return new WaitForEndOfFrame();
            }

            if (Characters.Length > 0)
            {
                //如果我们有一个角色
                if (indexCharacter >= Characters.Length)
                    indexCharacter = Characters.Length - 1;

                if (indexCharacter < 0)
                    indexCharacter = 0;

                //为预览而生成预制角色
                characterLoaded = Characters[indexCharacter];
                if (UnitZ.characterManager)
                {
                    UnitZ.characterManager.SetupCharacter(characterLoaded);
                    if (UnitZ.characterManager.CharacterSelected != null)
                    {
                        if (characterPreviewer != null)
                            Destroy(characterPreviewer);

                        //弃用所有无关组件
                        characterPreviewer = GameObject.Instantiate(UnitZ.characterManager.CharacterSelected, previewSpot.position, previewSpot.rotation);
                        characterPreviewer.GetComponent<CharacterMotor>().enabled = false;
                        characterPreviewer.GetComponent<HumanCharacter>().enabled = false;
                        characterPreviewer.GetComponent<CharacterInventory>().SetupStarterItem();
                        characterPreviewer.GetComponent<CharacterInventory>().enabled = false;
                        characterPreviewer.GetComponent<CharacterAnimation>().enabled = false;
                        characterPreviewer.GetComponent<CharacterFootStep>().enabled = false;
                        characterPreviewer.GetComponent<CharacterDriver>().enabled = false;
                        characterPreviewer.GetComponent<FPSController>().enabled = false;
                        characterPreviewer.GetComponent<CharacterItemDroper>().enabled = false;
                        characterPreviewer.GetComponent<PlayerView>().FPSCamera.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                //如果没有任何角色，就返回角色创建
                MainMenuManager menu = (MainMenuManager)GameObject.FindObjectOfType(typeof(MainMenuManager));
                if (menu)
                    menu.OpenPanelByName("CreateCharacter");
            }
        }
    }

    public void DrawCharactersToCanvas()
    {
        //在屏幕绘制角色预制体
        if (Canvas == null || CharacterBadgePrefab == null || Characters == null)
            return;
        ClearCanvas();
        for (int i = 0; i < Characters.Length; i++)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(CharacterBadgePrefab.gameObject, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(Canvas.transform);
            CharacterBadge charbloger = obj.GetComponent<CharacterBadge>();
            RectTransform rect = obj.GetComponent<RectTransform>();
            if (rect)
            {
                rect.anchoredPosition = new Vector2(5, -(((CharacterBadgePrefab.sizeDelta.y + 5) * i)));
                rect.localScale = CharacterBadgePrefab.gameObject.transform.localScale;
            }
            if (charbloger)
            {
                //更新GUI元素数据
                charbloger.Index = i;
                charbloger.CharacterData = Characters[i];
                if (UnitZ.characterManager)
                {
                    if (UnitZ.characterManager.CharacterPresets.Length > 0 && Characters[i].CharacterIndex < UnitZ.characterManager.CharacterPresets.Length)
                    {
                        charbloger.GUIImage.texture = UnitZ.characterManager.CharacterPresets[Characters[i].CharacterIndex].Icon;
                    }
                }
                charbloger.GUIName.text = Characters[i].PlayerName;
                charbloger.CharacterCreatorS = this;
                charbloger.name = Characters[i].PlayerName;
            }
        }
        Canvas.sizeDelta = new Vector2(Canvas.sizeDelta.x, (CharacterBadgePrefab.sizeDelta.y + 5) * Characters.Length);
    }

    public void CreateCharacter(Text textName)
    {
        //创建角色
        if (UnitZ.characterManager && textName)
        {
            if (UnitZ.characterManager.CreateCharacter(textName.text))
            {
                Setup();
                StartCoroutine(LoadCharacters());
                MainMenuManager menu = (MainMenuManager)GameObject.FindObjectOfType(typeof(MainMenuManager));
                if (menu)
                    menu.OpenPanelByNameNoPreviousSave("Home");
            }
        }
    }

    public void ChangeCharacter(int index)
    {
        if (UnitZ.playerSave)
        {
            ClearCanvas();
            //确认角色数据并存储
            characterLoaded.CharacterIndex = index;
            //characterLoaded.PlayerName = "your new name"; 
            if (UnitZ.playerSave.UpdateCharacter(characterLoaded))
            {
                StartCoroutine(LoadCharacters());
            }
        }
    }

    public void SelectCharacter(CharacterSaveData character)
    {
        //选择角色
        if (UnitZ.characterManager)
            UnitZ.characterManager.SetupCharacter(character);

        MainMenuManager menu = (MainMenuManager)GameObject.FindObjectOfType(typeof(MainMenuManager));
        if (menu)
            menu.OpenPanelByName("EnterWorld");
    }

    public void SetCharacter()
    {
        //准备角色
        if (UnitZ.characterManager)
            UnitZ.characterManager.SetCharacter();
    }

    public void SelectCreateCharacter(int index)
    {
        //选择角色
        if (UnitZ.characterManager)
            UnitZ.characterManager.SelectCreateCharacter(index);
    }

    public void RemoveCharacter(int index)
    {
        //移除角色
        if (UnitZ.characterManager)
        {
            UnitZ.characterManager.RemoveCharacter(Characters[index]);
            StartCoroutine(LoadCharacters());
        }
    }


}
