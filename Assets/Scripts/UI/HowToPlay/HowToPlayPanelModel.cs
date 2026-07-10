using UnityEngine;

[CreateAssetMenu(fileName = "HowToPlayPanelModel", menuName = "UIModel/HowToPlayPanel")]
public class HowToPlayPanelModel : ScriptableObject
{
    [Header("UIbind用")]
    public string step;
    public Sprite howToPlayImage;

    [Header("データ")]
    public Sprite[] howToPlayImages;
}