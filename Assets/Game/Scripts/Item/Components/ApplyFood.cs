using UnityEngine;
using System.Collections;

//使用食物
public class ApplyFood : MonoBehaviour
{
	public byte DrinkPlus = 10;
	public byte HealthPlus = 10;
	
	void Start ()
	{
        HumanCharacter character;

		if (this.transform.root) {
			character = this.transform.root.GetComponent<HumanCharacter> ();
		} else {
			character = this.transform.GetComponent<HumanCharacter> ();
		}

		if (character) {
            character.UseHeal(HealthPlus);
            character.UsePill(DrinkPlus);
		}
		GameObject.Destroy (this.gameObject);
	}
}
