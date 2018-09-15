using UnityEngine;

//迷你地图
public class Minimap : MonoBehaviour
{

    public bool FixedPlayer;
    public Vector2 WorldRealSize;
    public RectTransform MiniMap;
    public GameObject Player;
    public RectTransform SafeArea;
    public RectTransform DeadArea;
    public RectTransform PlayerIndicator;

    public float MapScale = 1;
    public void Zoom(float zoom)
    {
        if (MiniMap == null)
            return;

        MapScale += zoom;
        if (MapScale < 1)
        {
            MiniMap.anchoredPosition = Vector2.zero;
            MapScale = 1;
        }

        MiniMap.localScale = Vector2.one * MapScale;
    }

    //世界位置->地图
    Vector2 WorldPositionToMap(Vector2 minimap, Vector3 realposition)
    {
        Vector2 res = Vector2.zero;
        float scaleRatioX = resize(minimap.x, WorldRealSize.x);
        float scaleRatioY = resize(minimap.y, WorldRealSize.y);
        res = new Vector2(realposition.x * scaleRatioX, realposition.z * scaleRatioY);
        return res;
    }

    float resize(float minimap, float realsize)
    {
        return minimap / realsize;
    }

    void Start()
    {

    }

    void Update()
    {
        if (UnitZ.NetworkGameplay != null)
        {
            if (SafeArea)
            {
                //安全区
                if (UnitZ.NetworkGameplay.safeArea != null)
                {
                    SafeArea.anchoredPosition = WorldPositionToMap(new Vector2(MiniMap.rect.width, MiniMap.rect.height), UnitZ.NetworkGameplay.safeArea.transform.position);
                    Vector2 newsize = new Vector2(resize(MiniMap.rect.width, WorldRealSize.x) * UnitZ.NetworkGameplay.safeArea.transform.localScale.x, resize(MiniMap.rect.height, WorldRealSize.y) * UnitZ.NetworkGameplay.safeArea.transform.localScale.z);
                    SafeArea.sizeDelta = newsize;
                }
                SafeArea.gameObject.SetActive(UnitZ.NetworkGameplay.safeArea && UnitZ.NetworkGameplay.safeArea.activeSelf);
            }

            if (DeadArea)
            {
                //危险区
                if (UnitZ.NetworkGameplay.lastDeadArea != null)
                {
                    DeadArea.anchoredPosition = WorldPositionToMap(new Vector2(MiniMap.rect.width, MiniMap.rect.height), UnitZ.NetworkGameplay.lastDeadArea.transform.position);
                    Vector2 newsize = new Vector2(resize(MiniMap.rect.width, WorldRealSize.x) * UnitZ.NetworkGameplay.lastDeadArea.transform.localScale.x, resize(MiniMap.rect.height, WorldRealSize.y) * UnitZ.NetworkGameplay.lastDeadArea.transform.localScale.z);
                    DeadArea.sizeDelta = newsize;
                }
                DeadArea.gameObject.SetActive(UnitZ.NetworkGameplay.lastDeadArea && UnitZ.NetworkGameplay.lastDeadArea.activeSelf);
            }

        }
        if (Player == null && UnitZ.playerManager && UnitZ.playerManager.PlayingCharacter)
            Player = UnitZ.playerManager.PlayingCharacter.gameObject;

        if (MiniMap != null && PlayerIndicator != null && Player != null)
        {
            if (!FixedPlayer)
            {
                Vector2 position = WorldPositionToMap(new Vector2(MiniMap.rect.width, MiniMap.rect.height), Player.transform.position);
                PlayerIndicator.anchoredPosition = position;
                Quaternion rota = PlayerIndicator.rotation;
                rota.eulerAngles = new Vector3(rota.eulerAngles.x, rota.eulerAngles.y, -Player.transform.rotation.eulerAngles.y);
                PlayerIndicator.rotation = rota;

            }
            else
            {
                MiniMap.anchoredPosition = WorldPositionToMap(new Vector2(-MiniMap.rect.width, -MiniMap.rect.height), Player.transform.position);
                PlayerIndicator.anchoredPosition = Vector2.zero;
                Quaternion rota = PlayerIndicator.rotation;
                rota.eulerAngles = new Vector3(rota.eulerAngles.x, rota.eulerAngles.y, -Player.transform.rotation.eulerAngles.y);
                PlayerIndicator.rotation = rota;
            }

        }
    }
}
