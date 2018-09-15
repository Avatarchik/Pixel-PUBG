using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

//使用物品
public class FPSItemUsing : FPSItemEquipment
{
    public bool HoldFire = false;
    public GameObject Item;
    public float FireRate = 0.1f;
    public int UsingType = 0;
    public ItemData ItemUsed;
    public bool AutoUse;
    public bool InfinityAmmo;
    public bool OnAnimationEvent;
    public AudioClip SoundUse;
    private CharacterSystem character;
    private FPSController fpsController;
    private float timeTemp;
    private AudioSource audioSource;
    private Animator animator;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();
        if (this.transform.root)
        {
            character = this.transform.root.GetComponent<CharacterSystem>();
            fpsController = this.transform.root.GetComponent<FPSController>();
            if (character == null)
                character = this.transform.root.GetComponentInChildren<CharacterSystem>();
            if (fpsController == null)
                fpsController = this.transform.root.GetComponentInChildren<FPSController>();
        }
        else
        {
            character = this.transform.GetComponent<CharacterSystem>();
            fpsController = this.transform.GetComponent<FPSController>();
        }
        timeTemp = Time.time;
        if (AutoUse)
        {
            Use();
        }
    }

    void Update()
    {

    }

    void Use()
    {
        Debug.Log("Use item");
        if (ItemUsed != null)
        {
            if (!InfinityAmmo)
            {
                if (character != null && character.inventory != null && !character.inventory.CheckItem(ItemUsed, 1))
                {
                    return;
                }
            }
        }

        if (!OnAnimationEvent)
        {
            OnAction();
        }

        if (animator)
        {
            animator.SetInteger("shoot_type", UsingType);
            animator.SetTrigger("shoot");
        }
        if (character != null)
        {
            character.AttackAnimation(UsingType);
        }
    }

    public override void Trigger()
    {
        if (!HoldFire && OnFire1)
            return;

        if (character && fpsController)
        {
            if (Time.time > timeTemp + FireRate)
            {
                Use();
                timeTemp = Time.time;
            }
        }

        base.Trigger();
    }

    public override void OnAction()
    {

        if (Item)
        {
            if (ItemUsed != null)
            {
                if (!InfinityAmmo)
                {
                    if (character != null && character.inventory != null)
                    {
                        if (character.inventory.RemoveItem(ItemUsed, 1))
                        {
                            GameObject item = (GameObject)GameObject.Instantiate(Item, this.transform.position, this.transform.rotation);
                            item.transform.parent = character.transform;

                            if (SoundUse && audioSource)
                                if (audioSource.enabled)
                                    audioSource.PlayOneShot(SoundUse);

                            if (character != null && character.inventory != null)
                            {
                                character.inventory.EquipPreviousPrimary();
                            }
                        }
                    }
                }
            }
        }

        base.OnAction();
    }

}
