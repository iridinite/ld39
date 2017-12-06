using UnityEngine;

public class ScreenShaker : MonoBehaviour {

    private static float shaketime;
    private static float shaketimemax;
    private static float shakeintensity;

    public static void SetShake(float time, float intensity) {
        shaketime = 0f;
        shaketimemax = time;
        shakeintensity = intensity;
    }

    private void FixedUpdate() {
        if (shaketime >= shaketimemax || shakeintensity <= 0f) {
            transform.localPosition = Vector3.zero;
            return;
        }

        float power = (shaketimemax - shaketime) / shaketimemax;
        Vector3 delta = Random.onUnitSphere * power * shakeintensity;
        transform.localPosition = delta;

        shaketime += Time.fixedDeltaTime;
    }

}
