using UnityEngine;
using UnityEngine.UIElements;

public class ElapsedUI : MonoBehaviour
{
    UIDocument uiDoc;
    Label elapsedTimeText;

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        elapsedTimeText = uiDoc.rootVisualElement.Q<VisualElement>("elapsed-time-container").Q<Label>("time");
        elapsedTimeText.style.display = DisplayStyle.Flex;
    }

    public void UpdateElapsedTime(float elapsedTime)
    {
        elapsedTimeText.text = $"経過時間: {elapsedTime:F1}秒";
    }
}