using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

//装备武器
public class FPSWeaponEquipment : FPSItemEquipment
{

    private CharacterSystem character;
    private FPSController fpsController;
    private float timeTemp;
    private AudioSource audioSource;
    private Animator animator;
    [Header("Ammo")]
    public ItemData ItemUsed;
    public bool InfinityAmmo;
    public int ClipSize = 30;
    public byte BulletNum = 1;
    public int Ammo = 30;
    public int AmmoMax = 30;
    [HideInInspector]
    public int AmmoHave = 0;

    [Header("Firing")]
    public float AnimationSpeed = 1;
    public bool OnAnimationEvent;
    public int UsingType = 0;
    public bool HoldFire = true;
    public float FireRate = 0.09f;
    public byte Spread = 20;
    public byte Damage = 10;
    public float Force = 10;
    public Vector2 KickPower = Vector2.zero;
    public Vector3 AimPosition = new Vector3(-0.082f, 0.06f, 0);
    public byte MaxPenetrate = 1;
    public float Distance = 100;

    [Header("Sound / FX")]
    public AudioClip SoundFire;
    public AudioClip SoundReload;
    public AudioClip SoundReloadComplete;
    public AudioClip[] DamageSound;
    public GameObject MuzzleFX;
    public Transform MuzzlePoint;
    public GameObject ProjectileFX;
    public Transform Pointer;

    [Header("Other")]
    public float panicfire = 0;
    public float PanicFireMult = 0.1f;
    public float FOVZoom = 65;
    public float SpreadZoomMult = 1;
    public bool HideWhenZoom = false;
    private float animationSpeedTemp = 1;

    void Start()
    {
        reloading = false;
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

        if (character)
        {
            character.DamageSound = DamageSound;
            if (character.inventory)
                Pointer = character.inventory.Pointer;
            character.PrimaryWeaponDistance = Distance;
            character.gameObject.SendMessage("OnEquipChanged", UsingType, SendMessageOptions.DontRequireReceiver);
        }

        if (fpsController)
        {
            fpsController.zooming = false;
        }

        Hide(true);
        timeTemp = Time.time;
        if (animator)
        {
            animationSpeedTemp = animator.speed;
        }
    }

    public override void Trigger()
    {
        if (!HoldFire && OnFire1)
            return;

        if (character)
        {

            if (Time.time > timeTemp + FireRate)
            {
                Shoot();
                timeTemp = Time.time;
            }
        }
        base.Trigger();
    }

    public override void OnTriggerRelease()
    {
        base.OnTriggerRelease();
    }

    public override void Trigger2()
    {
        if (fpsController != null)
            fpsController.Zoom(FOVZoom, AimPosition);
        base.Trigger2();
    }

    public override void OnTrigger2Release()
    {
        base.OnTrigger2Release();
    }

    public void Shoot()
    {

        if (animator)
            animator.speed = AnimationSpeed;

        if (Ammo <= 0 && !InfinityAmmo)
            return;

        if (!OnAnimationEvent)
        {
            OnAction();
        }
        if (character != null)
        {
            character.AttackAnimation(UsingType);
        }
        if (animator)
        {
            animator.SetTrigger("shoot");
        }

    }

    private bool reloading;

    public override void Reload()
    {
        if (Ammo >= AmmoMax || Ammo <= 0 && AmmoHave <= 0)
            return;

        if (!reloading)
        {
            if (audioSource && SoundReload)
            {
                audioSource.PlayOneShot(SoundReload);
            }
        }

        if (ItemUsed != null)
        {
            int ammo = character.inventory.GetItemNum(ItemUsed);
            if (!InfinityAmmo)
            {
                if (character != null && character.inventory != null && ammo <= 0)
                {
                    return;
                }
            }
        }
        if (animator)
            animator.SetTrigger("reloading");

        reloading = true;

        base.Reload();
    }

    public override void ReloadComplete()
    {
        int ammoused = ClipSize;
        if (AmmoMax - Ammo < ammoused)
            ammoused = AmmoMax - Ammo;

        if (character != null && character.inventory != null && ItemUsed != null)
        {
            if (AmmoHave <= 0)
            {
                reloading = false;
                return;
            }
            if (AmmoHave < ammoused)
            {
                ammoused = AmmoHave;
            }
            character.inventory.RemoveItem(ItemUsed, ammoused);
        }
        Ammo += ammoused;

        if (Ammo >= AmmoMax)
        {
            reloading = false;
        }
        else
        {
            Reload();
        }
        if (audioSource && SoundReloadComplete)
        {
            audioSource.PlayOneShot(SoundReloadComplete);
        }
        base.ReloadComplete();
    }

    private float spreadmult;

    void Update()
    {
        if (!reloading && Ammo <= 0)
        {
            Reload();
        }

        Hide(true);
        spreadmult = 1;

        if (fpsController)
        {
            fpsController.PlayerView.FPSCamera.hideWhenScoping = HideWhenZoom;
            if (HideWhenZoom)
            {
                if (fpsController.zooming)
                {
                    Hide(false);
                    spreadmult = SpreadZoomMult;
                }
            }
        }

        if (panicfire < 0.01f)
            panicfire = 0;

        panicfire += (0 - panicfire) * 5 * Time.deltaTime;

        if (IsVisible)
        {
            if (animator)
                animator.SetInteger("shoot_type", UsingType);

            if (character != null && character.inventory != null && ItemUsed != null)
            {
                AmmoHave = character.inventory.GetItemNum(ItemUsed);
            }
            if (CollectorSlot != null)
            {
                CollectorSlot.NumTag = Ammo;
            }

            if (!InfinityAmmo)
            {
                Info = Ammo + "/" + AmmoHave;
            }
            else
            {
                Info = "Infinity";
            }
        }
    }

    public void SetCollectorSlot(ItemCollector collector)
    {
        CollectorSlot = collector;
        if (collector.NumTag != -1)
            Ammo = collector.NumTag;
    }

    public override void OnAction()
    {
        if (animator)
            animator.speed = animationSpeedTemp;

        if (Ammo > 0 || InfinityAmmo)
        {
            if (!InfinityAmmo)
                Ammo -= 1;

            if (BulletNum <= 0)
                BulletNum = 1;

            byte spread = (byte)(Spread * spreadmult);
            byte seed = (byte)Random.Range(0, 255 * spreadmult);

            if (!MuzzlePoint)
            {
                MuzzlePoint = this.transform;
            }

            if (IsVisible)
            {
                if (MuzzleFX)
                {
                    GameObject fx = (GameObject)GameObject.Instantiate(MuzzleFX, MuzzlePoint.position, MuzzlePoint.rotation);
                    fx.transform.parent = this.transform;
                    GameObject.Destroy(fx, 2);
                }

                if (audioSource && SoundFire && IsVisible)
                {
                    if (audioSource.enabled)
                        audioSource.PlayOneShot(SoundFire);
                }

                for (int i = 0; i < BulletNum; i++)
                {
                    if (Pointer)
                    {
                        Vector3 direction = (Pointer.forward + (new Vector3(Random.Range(-spread, spread) * 0.001f, Random.Range(-spread, spread) * 0.001f, Random.Range(-spread, spread) * 0.001f) * spreadmult));
                        if (ProjectileFX && fpsController)
                        {
                            GameObject.Instantiate(ProjectileFX, Pointer.position + (direction * 5), Quaternion.LookRotation(direction));
                        }
                    }
                }
            }
            if (fpsController)
                fpsController.Kick(KickPower);

            if (character != null)
            {
                if (Pointer)
                {
                    if (panicfire < 0.5f)
                    {
                        if (BulletNum == 1)
                        {
                            spread = 0;
                        }
                    }

                    //我们可以使用所有参数调用DoDamage。但是，它在网络上发送了太多参数
                    //character.DoDamage(Pointer.position, Pointer.forward, BulletNum, spread, (byte)Random.Range(0, 255), (byte)Damage, Distance, MaxPenetrate, character.NetID, character.Team);
                    //所以我们需要在物品管理器中注册所有武器，所以我们只发送物品索引。
                    character.DoDamageByItemIndex(Pointer.position, Pointer.forward, ItemIndex, seed);
                }
            }
            panicfire += 5;
        }
        base.OnAction();

    }
}
