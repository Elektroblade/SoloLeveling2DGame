using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public static DamagePopup Create(Vector3 position, string damageAmount, int isCriticalHit) {
        Transform damagePopupTransform = Instantiate(GameManager.Instance.pfDamagePopup, position, Quaternion.identity);

        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isCriticalHit);

        return damagePopup;
    }

    private static int sortingOrder;

    private const float DISAPPEAR_TIMER_MAX = 1f;
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private void Awake() {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(string damageAmount, int isCriticalHit) {
        textMesh.SetText(damageAmount.ToString());
        if (isCriticalHit == 0)
        {
            // Normal hit
            textMesh.fontSize = 10;
            textColor = new Color(255f/256, 197f/256, 0f, 255f/256);
        } else if (isCriticalHit > 0) {
            // Critical hit
            textMesh.fontSize = 14;
            textColor = new Color(255f/256, 43f/256, 0f, 255f/256);
        } else if (isCriticalHit == -2) {
            // Reanimated hit
            textMesh.fontSize = 4;
            textColor = new Color(46f/256, 186f/256, 239f, 127f/256);
        } else {
            // Enemy hit
            textMesh.fontSize = 5;
            textColor = new Color(10f/256, 10f/256, 10f, 255f/256);
        }
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

        moveVector = new Vector3(1, 1) * 5f;
    }

    private void Update() {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 0.8f * Time.deltaTime;

        float moveYSpeed = 2f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f) {
            // First half of the popup lifetime
            float increaseScaleAmount = 0.5f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        } else {
            // Second half of the popup lifetime
            float decreaseScaleAmount = 0.5f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // Start disappearing

            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
