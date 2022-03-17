using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class HealthToColor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    private HealthComponent m_healthComponent;

    private void Start()
    {
        m_healthComponent = GetComponent<HealthComponent>();
        m_healthComponent.OnTakeDamage.AddListener(UpdateColor);
        UpdateColor();
    }

    private void OnDestroy()
    {
        m_healthComponent.OnTakeDamage.RemoveListener(UpdateColor);
    }

    private void UpdateColor()
    {
        float t = m_healthComponent.GetCurrentHealth() / m_healthComponent.GetMaxHealth();
        float h = Mathf.Lerp(0.45f, 1.0f, t);
        float s = Mathf.Lerp(0.3f, 0.5f, t);
        float v = Mathf.Lerp(0.3f, 0.6f, t);
        m_spriteRenderer.color = Color.HSVToRGB(h, s, v);
    }
}
