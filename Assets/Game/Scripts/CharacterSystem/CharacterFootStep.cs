using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterFootStep : MonoBehaviour
{
    private AudioSource audios;
    private CharacterSystem character;
    private float delay = 0;
    public AudioClip[] FoodSteps;
    public float Delay = 3;

    void Start()
    {
        character = this.gameObject.GetComponent<CharacterSystem>();
        audios = this.gameObject.GetComponent<AudioSource>();
    }

    void PlaySound()
    {
        if (character && (character.Motor != null && character.Motor.grounded || character.Motor == null))
        {
            //在地上才播放脚步声
            if (FoodSteps.Length > 0)
            {
                //随机播放脚步声
                audios.PlayOneShot(FoodSteps[Random.Range(0, FoodSteps.Length)]);
            }
        }
    }

    void Update()
    {
        if (!character)
            return;

        if (character.enabled && character.IsAlive && (character.Motor != null && character.Motor.grounded || character.Motor == null))
        {
            if (delay >= Delay)
            {
                PlaySound();
                delay = 0;
            }
        }
        if (delay < Delay)
        {
            //通过移动速度计算脚步频率
            delay += character.MoveVelocity.magnitude * Time.deltaTime;
        }
    }
}
