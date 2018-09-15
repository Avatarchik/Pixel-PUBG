using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

//装备放置
public class FPSItemPlacing : FPSItemEquipment
{
	public bool HoldFire = false;
	public GameObject Item;
	public GameObject ItemIndicator;
	public float FireRate = 0.1f;
	public int UsingType = 0;
	public ItemData ItemUsed;
	public bool InfinityAmmo;
	public bool OnAnimationEvent;
	public bool PlacingNormal = true;
	public AudioClip SoundPlaced;
	public float Ranged = 4;
	public string[] KeyPair = {""};
	private CharacterSystem character;
	private FPSController fpsController;
	private float timeTemp;
	private AudioSource audioSource;
	private Animator animator;
	private GameObject preplacingObject;

	void Start ()
	{
		animator = this.GetComponent<Animator> ();
		audioSource = this.GetComponent<AudioSource> ();
		if (this.transform.root) {
			character = this.transform.root.GetComponent<CharacterSystem> ();
			fpsController = this.transform.root.GetComponent<FPSController> ();
			if (character == null)
				character = this.transform.root.GetComponentInChildren<CharacterSystem> ();
			if (fpsController == null)
				fpsController = this.transform.root.GetComponentInChildren<FPSController> ();
		} else {
			character = this.transform.GetComponent<CharacterSystem> ();
			fpsController = this.transform.GetComponent<FPSController> ();
		}

		timeTemp = Time.time;
		
		if (ItemIndicator) {
			preplacingObject = (GameObject)GameObject.Instantiate (ItemIndicator.gameObject, this.transform.position, ItemIndicator.transform.rotation);
		}
	}
	
	void OnDestroy ()
	{
		if (preplacingObject)
			GameObject.Destroy (preplacingObject);
	}

	void Update ()
	{
		
		if (preplacingObject != null) {
			RaycastHit surface = GoundPlacing ();
			if (surface.distance != 0) {
				preplacingObject.SetActive (true);
				
				if (objectToSnap != null) {
					preplacingObject.transform.position = objectToSnap.transform.position;
					preplacingObject.transform.rotation = objectToSnap.transform.rotation;
				} else {
					preplacingObject.transform.position = surface.point;
					if (PlacingNormal) {
						preplacingObject.transform.rotation = Quaternion.LookRotation (surface.normal);
					}	
				}
			} else {
				preplacingObject.SetActive (false);
			}
		}
	}
	
	void Use ()
	{
		if (ItemUsed != null) {
			if (!InfinityAmmo) {
				if (character != null && character.inventory != null && !character.inventory.CheckItem (ItemUsed, 1)) {
					return;	
				}
			}
		}
		
		if (!OnAnimationEvent) {
			OnAction ();
		}
				
		if (animator) {
			animator.SetInteger ("shoot_type", UsingType);
			animator.SetTrigger ("shoot");
		}
		if (character != null) {
			character.AttackAnimation (UsingType);
		}
		
	}

	public override void Trigger ()
	{
		if (!HoldFire && OnFire1)
			return;
		if (character && fpsController) {
			if (Time.time > timeTemp + FireRate) {
				Use ();
				timeTemp = Time.time;
			}
		}
		base.Trigger ();
	}

	public override void OnAction ()
	{
		RaycastHit surface = GoundPlacing ();
		
		if (surface.distance != 0) {
			if (Item) {
				Vector3 point = surface.point;
				Quaternion placeRotation = Item.gameObject.transform.rotation;
				
				if (PlacingNormal) {
					placeRotation = Quaternion.LookRotation (surface.normal);
				}
				
				if (objectToSnap != null) {
					point = objectToSnap.transform.position;
					placeRotation = objectToSnap.transform.rotation;
				}

				character.CmdRequestSpawnObject(point,placeRotation,ItemID,"");

				if (SoundPlaced && audioSource)
					if(audioSource.enabled)
						audioSource.PlayOneShot (SoundPlaced);
				
				
			}
			if (ItemUsed != null) {
				if (!InfinityAmmo) {
					if (character != null && character.inventory != null && !character.inventory.RemoveItem (ItemUsed, 1)) {
						return;	
					}
				}
			}
		}
		base.OnAction ();
	}

	GameObject objectToSnap;

	RaycastHit GoundPlacing ()
	{
		
		float raySize = Ranged;
		RaycastHit[] casterhits = Physics.RaycastAll (fpsController.PlayerView.FPSCamera.RayPointer.position, fpsController.PlayerView.FPSCamera.RayPointer.forward, raySize);
		for (int i=0; i<casterhits.Length; i++) {
			PlacingArea placing = casterhits [i].collider.GetComponent<PlacingArea> ();
			if (placing && placing.KeyPairChecker (KeyPair)) {
				if (placing.Snap) {
					objectToSnap = placing.gameObject;
				}
				if (casterhits [i].collider && placing) {
					return casterhits [i];
				}
			}
		}
		objectToSnap = null;
		return new RaycastHit ();
	}
	
	
}
