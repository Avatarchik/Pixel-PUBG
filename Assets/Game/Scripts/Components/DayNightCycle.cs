using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour
{

    public Light DirectionLight;
    public Texture2D SkyColors;
    public Texture2D LightColor;
    public Texture2D AmbientColor;
    public float TimePerDay = 2;
    [Range(0, 1)]
    public float Timer = 0;
    private float timeTemp = 0;
    private float fade = 0;
    public float MaxFade = 20;
    public bool FadeOnTime = true;
    public bool Freeze = false;
    public bool LightRotation = false;

    void Start()
    {
        timeTemp = Time.time;
        fade = Timer;
    }

    void Update()
    {
        if (!Freeze)
        {
            if (FadeOnTime)
            {
                float timeperfade = TimePerDay / MaxFade;
                if (Time.time >= timeTemp + timeperfade)
                {
                    timeTemp = Time.time;
                    fade += 1.0f / MaxFade;
                }
                if (fade > 1.0f)
                {
                    fade = 0;
                    Timer = 0;
                }

                Timer += (fade - Timer) / 10f;

            }
            else
            {
                if (Timer > 1)
                {
                    Timer = 0;
                }
                else
                {
                    Timer += (1.0f * Time.deltaTime) * (1.0f / TimePerDay);

                }
            }
        }
        if (LightRotation)
            DirectionLight.transform.rotation = Quaternion.Euler(new Vector3(360 * Timer, 0, 0));
        Color skyColor = SkyColors.GetPixelBilinear(Timer, 0);
        RenderSettings.skybox.SetColor("_Tint", skyColor);
        RenderSettings.fogColor = skyColor;
        RenderSettings.ambientLight = AmbientColor.GetPixelBilinear(Timer, 0);
        DirectionLight.color = LightColor.GetPixelBilinear(Timer, 0);



    }
}
