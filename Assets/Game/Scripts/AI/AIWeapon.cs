using UnityEngine;

//AI武器添加
public class AIWeapon : MonoBehaviour {

    public FPSWeaponEquipment EquippedWeapon;
    void Start () {
		
	}
    //给AI添加FPS武器
    public void OnAttached(GameObject item)
    {
        EquippedWeapon = item.GetComponent<FPSWeaponEquipment>();
        EquippedWeapon.InfinityAmmo = true;
        EquippedWeapon.IsVisible = false;
        EquippedWeapon.GetComponent<AudioSource>().enabled = false;
    }
}
