using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LightController : MonoBehaviour
{
    public Animator playerAnimator;
    public Light2D globalLight;

    public Color hurtColor = Color.red;
    public Color healColor = Color.blue;

    public float flashDuration = 0.2f;

    [Range(0f, 10f)]
    public float extraIntensity = 0.5f;

    private Color originalColor;
    private float originalIntensity;

    private void Start()
    {
        if (globalLight != null)
        {
            originalColor = globalLight.color;
            originalIntensity = globalLight.intensity;
        }
    }

    private void Update()
    {
        if (playerAnimator != null && globalLight != null)
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
            {
                StopCoroutine("FlashLight");
                StartCoroutine(FlashLight());
            }

            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Heal"))
            {
                StopCoroutine("HealLight");
                StartCoroutine(HealLight());
            }
        }
    }

    private IEnumerator FlashLight()
    {
        globalLight.color = hurtColor;
        globalLight.intensity = originalIntensity + extraIntensity;

        yield return new WaitForSeconds(flashDuration);

        globalLight.color = originalColor;
        globalLight.intensity = originalIntensity;
    }

    private IEnumerator HealLight()
    {
        globalLight.color = healColor;
        globalLight.intensity = originalIntensity + extraIntensity;

        yield return new WaitForSeconds(flashDuration);

        globalLight.color = originalColor;
        globalLight.intensity = originalIntensity;
    }

}
