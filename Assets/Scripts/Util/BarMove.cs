using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BarMove : MonoBehaviour
{
    public IEnumerator MoveBarAsync(Slider bar, float targetValue, float speed) {
        float t = 0f;
        float currentValue = bar.value;
        while (t <= 1) {
            t += speed * Time.deltaTime;
            bar.value = Mathf.Lerp(currentValue, targetValue, t);
            yield return null;
        }
    }
}
