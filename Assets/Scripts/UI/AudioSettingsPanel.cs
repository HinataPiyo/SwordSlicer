using UnityEngine;
using UnityEngine.UIElements;

public class AudioSettingsPanel : UIModuleBase
{
    public class Entry
    {
        public Label name;
        public AudioType type;
        public float value;
        public Slider slider;
    }

    protected override void Initialize()
    {
        var entries = Root.Q<VisualElement>("slider-container").Query<VisualElement>("entry").ToList();
        for(int i = 0; i < entries.Count; i++)
        {
            var entry = new Entry() {
                name = entries[i].Q<Label>(),
                slider = entries[i].Q<Slider>(),
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