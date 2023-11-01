using UnityEngine;
using UnityEngine.UI;

public class BorderAnimation : MonoBehaviour
{
    public float animationSpeed = 1f;
    public float maxBorderSize = 1.1f;
    public float minBorderSize = 1f;
    private Image borderImage;
    private RectTransform rectTransform;
    private bool increasing = true;

    void Start()
    {
        borderImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        Debug.Log(borderImage);
    }

    void Update()
    {
        float scale = increasing ? rectTransform.localScale.x + animationSpeed * Time.deltaTime : rectTransform.localScale.x - animationSpeed * Time.deltaTime;

        // Reverse direction at min/max
        if (scale > maxBorderSize || scale < minBorderSize)
        {
            increasing = !increasing;
        }

        scale = Mathf.Clamp(scale, minBorderSize, maxBorderSize);
        rectTransform.localScale = new Vector3(scale, scale, scale);
    }
}
