using UnityEngine;
using System.Collections;

public class ItemToggleEquipment : ItemEquipment
{

    private CharacterSystem character;
    public GameObject ItemToggle;
    public bool IsActive;
    private AudioSource audioSource;
    public AudioClip SoundFire;
    public AudioClip[] DamageSound;

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        if (this.transform.root)
        {
            character = this.transform.root.GetComponent<CharacterSystem>();
        }
        else
        {
            character = this.transform.GetComponent<CharacterSystem>();
        }
        if (character)
            character.DamageSound = DamageSound;
    }

    public override void Action(Vector3 direction, byte num, byte spread, byte seed)
    {
        if (audioSource && SoundFire)
        {
            if (audioSource.enabled)
                audioSource.PlayOneShot(SoundFire);
        }
        if (ItemToggle)
        {
            ItemToggle.gameObject.SetActive(IsActive);
        }
        base.Action(direction, num, spread, seed);
    }
}
