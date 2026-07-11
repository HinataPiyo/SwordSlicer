using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class AudioSettingsPanel : UIModuleBase
{
    public class Entry
    {
        public Label name;
        public AudioType type;
        public float value;
        public Slider slider;
    }

    readonly List<Entry> entries = new List<Entry>();

    protected override void Initialize()
    {
        entries.Clear();

        var entryRoots = Root.Q<VisualElement>("slider-container").Query<VisualElement>("entry").ToList();
        for(int i = 0; i < entryRoots.Count; i++)
        {
            var entry = new Entry() {
                name = entryRoots[i].Q<Label>(),
                slider = entryRoots[i].Q<Slider>(),
                type = (AudioType)i,
            };

            entry.name.text = GetAudioTypeName(entry.type);
            entry.slider.highValue = 1.2f;
            entry.slider.lowValue = 0f;
            entry.value = ServiceLocator.Get<IAudioService>().GetVolume(entry.type);
            entry.slider.SetValueWithoutNotify(entry.value);
            ServiceLocator.Get<IAudioService>().SetVolume(entry.type, entry.value);
            
            entry.slider.RegisterValueChangedCallback(evt =>{
                entry.value = evt.newValue;
                ServiceLocator.Get<IAudioService>().SetVolume(entry.type, entry.value);
            });

            entries.Add(entry);
        }

        ILoad loadService = ServiceLocator.Get<ILoad>();
        if(loadService != null)
        {
            loadService.OnLoad -= RefreshFromSavedData;
            loadService.OnLoad += RefreshFromSavedData;
            RefreshFromSavedData();
        }
    }

    void OnDestroy()
    {
        ILoad loadService = ServiceLocator.Get<ILoad>();
        if(loadService != null)
        {
            loadService.OnLoad -= RefreshFromSavedData;
        }
    }

    void RefreshFromSavedData()
    {
        var audioService = ServiceLocator.Get<IAudioService>();
        for(int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            entry.value = audioService.GetVolume(entry.type);
            entry.slider.SetValueWithoutNotify(entry.value);
        }
    }

    string GetAudioTypeName(AudioType type)
    {
        switch(type)
        {
            case AudioType.Master:
                return "Master";
            case AudioType.BGM:
                return "BGM";
            case AudioType.SE:
                return "SE";
            default:
                return "";
        }
    }
        

    public override void BindNavigation(IPanelNavigationController controller)
    {
        controller.BindBackButton(Root.Q<Button>("BackButton"));
    }
}