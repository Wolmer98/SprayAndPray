using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverPopup : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text m_header;
    [SerializeField] private TMPro.TMP_Text m_description;

    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public async void ShowHoverPopup(string header, string description)
    {
        gameObject.SetActive(true);
        m_header.text = header;
        m_description.text = description;

        await System.Threading.Tasks.Task.Delay(20);

        LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
    }

    public void HideHoverPopup()
    {
        gameObject.SetActive(false);
    }
}
