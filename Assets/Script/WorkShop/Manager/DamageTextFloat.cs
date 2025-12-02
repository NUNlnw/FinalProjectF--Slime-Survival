using TMPro;
using UnityEngine;

public class DamageTextFloat : MonoBehaviour
{
    public TMP_Text text;
    public float floatSpeed = 1f;
    public float lifeTime = 1f;

    public void Init(int amount)
    {
        // หา TMP_Text ถ้ายังไม่ได้เซ็ต
        if (text == null)
        {
            text = GetComponentInChildren<TMP_Text>();
        }

        if (text != null)
        {
            text.text = amount.ToString();
        }
        else
        {
            Debug.LogWarning("DamageTextFloat: ไม่เจอ TMP_Text บน prefab");
        }
    }

    void Update()
    {
        // ลอยขึ้นตามแกน Y
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
