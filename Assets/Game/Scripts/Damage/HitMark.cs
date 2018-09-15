using UnityEngine;
using System.Collections;

public struct DamagePackage
{
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 Normal;
    public byte Damage;
    public int ID;
    public byte Team;
    public byte DamageType;
}

public class HitMark : MonoBehaviour
{

    public DamageManager DamageManage;
    public ItemSticker Armor;
    public GameObject HitFX;
    public float DamageMult = 1;
    // 8 must be a Hitbox Layer;
    public int HitboxPhysicLayer = 8;

    private void Awake()
    {
        this.gameObject.layer = HitboxPhysicLayer;
    }

    void Start()
    {
        if (this.transform.root)
        {
            DamageManage = this.transform.root.GetComponent<DamageManager>();
        }
        else
        {
            DamageManage = this.transform.GetComponent<DamageManager>();
        }
    }

    public void OnHit(DamagePackage pack)
    {
        if (DamageManage)
        {

            float damageReducer = 1;
            if (Armor != null)
            {
                ItemEquipment armor = Armor.GetEquipped();
                if (armor != null)
                    damageReducer = 1 - armor.Armor;
            }
            //造成伤害
            float alldamage = (byte)((pack.Damage * DamageMult) * damageReducer);

            if (alldamage > byte.MaxValue)
                alldamage = byte.MaxValue;

            DamageManage.ApplyDamage((byte)alldamage, pack.Direction, pack.ID, pack.Team);
            //在十字准线中显示击中效果
            if (UnitZ.gameManager != null && UnitZ.gameManager.PlayerNetID == pack.ID)
            {
                if (UnitZ.playerManager.PlayingCharacter != null && UnitZ.playerManager.PlayingCharacter.inventory != null)
                {
                    if (UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment != null)
                    {
                        if (UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment.GetComponent<Crosshair>())
                        {
                            UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment.GetComponent<Crosshair>().Hit();
                        }
                    }
                }
            }
        }

        //击中位置添加粒子特效
        ParticleFX(pack.Position, pack.Normal);

    }
    public void OnHitTest(DamagePackage pack)
    {
        if (DamageManage)
        {
            //在十字准线中显示击中效果
            if (UnitZ.gameManager != null && UnitZ.gameManager.PlayerNetID == pack.ID)
            {
                if (UnitZ.playerManager.PlayingCharacter != null && UnitZ.playerManager.PlayingCharacter.inventory != null)
                {
                    if (UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment != null)
                    {
                        if (UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment.GetComponent<Crosshair>())
                        {
                            UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment.GetComponent<Crosshair>().Hit();
                        }
                    }
                }
            }
        }

        ParticleFX(pack.Position, pack.Normal);

    }
    public void ParticleFX(Vector3 position, Vector3 normal)
    {
        if (HitFX)
        {
            GameObject fx = (GameObject)GameObject.Instantiate(HitFX, position, Quaternion.identity);
            fx.transform.forward = normal;
            GameObject.Destroy(fx, 3);
        }
    }
}
