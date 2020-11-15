/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

public class DamagePopup : MonoBehaviour {

    // Create a Damage Popup
    public static DamagePopup Create(Vector3 position, int damageAmount, bool isCriticalHit) {
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);

        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isCriticalHit);

        return damagePopup;
    }

    public static DamagePopup Create(Vector3 position, int damageAmount, bool isCriticalHit, int fontSize)
    {
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);

        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isCriticalHit);
        damagePopup.SetFontSize(fontSize);

        return damagePopup;
    }

    private static int sortingOrder;

    private const float DISAPPEAR_TIMER_MAX = 1f;
    private const float SPEED_MOVEMENT = 10;
    private const float MIN_FACTOR = 3;
    private const float MAX_FACTOR = 6;
    private const string WHITE_COLOR = "FFFFFF";

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private void Awake() {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount, bool isCriticalHit) {
        textMesh.SetText(damageAmount.ToString());
        if (!isCriticalHit) {
            // Normal hit
            textMesh.fontSize = 36;
            textColor = UtilsClass.GetColorFromString(WHITE_COLOR);//UtilsClass.GetColorFromString("FFC500");
        } else {
            // Critical hit
            textMesh.fontSize = 45;
            textColor = UtilsClass.GetColorFromString("FF2B00");
        }
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
        textMesh.outlineWidth = 0.1f;

        float factor = RandomGames.Utils.Utils.GetRandomNumberBetweenTwoNumbers(MIN_FACTOR, MAX_FACTOR);
        Debug.Log($"<color=magenta>factor</color>: {factor}, between [{MIN_FACTOR},{MAX_FACTOR}]");
        moveVector = Vector3.one * factor;  //moveVector = new Vector3(.7f, 1) * 60f;
    }

    public void SetFontSize(int size)
    {
        textMesh.fontSize = size;
    }

    public void SetCameraOrientation(Camera cam)
    {
        var camXform = cam.transform;
        var forward = transform.position - camXform.position;
        forward.Normalize();
        var up = Vector3.Cross(forward, camXform.right);
        transform.rotation = Quaternion.LookRotation(forward, up);
    }

    private void Update() {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * SPEED_MOVEMENT * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f) {
            // First half of the popup lifetime
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        } else {
            // Second half of the popup lifetime
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0) {
            // Start disappearing
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0) {
                Destroy(gameObject);
            }
        }
    }

}
