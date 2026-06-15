using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField] VisualTreeAsset temp_upgradeElement;
    UIDocument uIDocs;
    ScrollView scrollView;
    List<VisualElement> swordIcons = new List<VisualElement>();
    List<UpgradeElementUI> upgradeElements = new List<UpgradeElementUI>();


    void Awake()
    {
        uIDocs = GetComponent<UIDocument>();
        
    }

    void Initialize()
    {
        swordIcons = uIDocs.rootVisualElement.Q("sword-icon-container").Query<VisualElement>("upgrade-sword-icon-root").ToList();
        scrollView = uIDocs.rootVisualElement.Q<ScrollView>();
        scrollView.Clear();
        upgradeElements.Clear();

        List<UpgradeEntry> entries = StatContext.I.GetUpgradeEntries();
        for(int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var element = temp_upgradeElement.Instantiate();
            
            UpgradeElementUI upgradeElementUI = new UpgradeElementUI();
            upgradeElementUI.Initialize(element, entry);

            scrollView.Add(element);
            upgradeElements.Add(upgradeElementUI);
        }
    }


    void OnEnable() => Initialize();

    
}