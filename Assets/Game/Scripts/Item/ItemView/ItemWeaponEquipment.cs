using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]

public class ItemWeaponEquipment : ItemEquipment
{

    private CharacterSystem character;
    public GameObject MuzzleFX;
    public GameObject ProjectileFX;
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

        if (MuzzleFX)
        {
            GameObject fx = (GameObject)GameObject.Instantiate(MuzzleFX, this.transform.position, this.transform.rotation);
            fx.transform.parent = this.transform;
            GameObject.Destroy(fx, 2);
        }

        if (ProjectileFX && num <= 1)
        {
            System.Random random = new System.Random(seed);
            for (int b = 0; b < num; b++)
            {
                Vector3 dir = direction + new Vector3(random.Next(-spread, spread) * 0.001f, random.Next(-spread, spread) * 0.001f, random.Next(-spread, spread) * 0.001f);
                GameObject fx = (GameObject)GameObject.Instantiate(ProjectileFX, this.transform.position, this.transform.rotation);
                fx.transform.forward = dir;
            }

        }
        base.Action(direction, num, spread, seed);
    }
}
