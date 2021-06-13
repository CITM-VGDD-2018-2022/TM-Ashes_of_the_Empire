using System;
using DiamondEngine;

public class BlackFade : DiamondComponent
{
    public enum STATE
    {
        NONE,
        FADE_IN,
        FADE_OUT
    }

    public static Action onFadeInCompleted;
    public static Action onFadeOutCompleted;

    public Material fade;

    private float current_alpha = 0.0f;

    private static STATE state;

    public float fadeOutSpeed = 1.0f;
    public float fadeInSpeed = 1.0f;

    private static float fadeSpeedMult = 1f;

    private void Awake()
    {
        fade = gameObject.GetComponent<Material>();
        fade.SetFloatUniform("fadeValue", current_alpha);

    }
    public void Update()
    {
        switch (state)
        {
            case STATE.NONE:
                break;
            case STATE.FADE_IN:
                FadeIn();
                break;
            case STATE.FADE_OUT:
                FadeOut();
                break;
        }
    }

    private void FadeIn()
    {
        if (current_alpha <= 1.1f)
        {
            current_alpha += Time.deltaTime * fadeInSpeed * fadeSpeedMult;
            if (current_alpha > 1.1f)
            {
                current_alpha = 1.1f;
                state = STATE.NONE;
                onFadeInCompleted?.Invoke();
            }
            fade.SetFloatUniform("fadeValue", current_alpha);
        }
    }

    private void FadeOut()
    {
        if (current_alpha > 0)
        {
            current_alpha -= Time.deltaTime * fadeOutSpeed * fadeSpeedMult;
            if (current_alpha < 0)
            {
                current_alpha = 0;
                state = STATE.NONE;
                onFadeOutCompleted?.Invoke();
                fadeSpeedMult = 1f;
            }
            fade.SetFloatUniform("fadeValue", current_alpha);
        }
    }

    public static void StartFadeIn()
    {
        state = STATE.FADE_IN;
    }

    public static void StartFadeIn(Action action)
    {
        onFadeInCompleted = action;
        state = STATE.FADE_IN;
    }

    public static void StartFadeOut()
    {
        state = STATE.FADE_OUT;
    }

    public static void SetFadeSpdMult(float spdMult)
    {
        fadeSpeedMult = spdMult;
    }

    public static void StartFadeOut(Action action)
    {
        onFadeOutCompleted = action;
        state = STATE.FADE_OUT;
    }

}