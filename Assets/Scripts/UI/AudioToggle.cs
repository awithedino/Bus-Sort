using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AudioToggle : MonoBehaviour
{
    public enum AudioType { Music, SFX }
    public AudioType toggleType;

    [Header("UI References")]
    public Image backgroundImage;
    public Image iconImage;

    [Header("Visuals")]
    public Sprite onSprite;
    public Sprite offSprite;
    public Color onColor = new Color(0.9607844f, 0.8509805f, 0.1843137f);
    public Color offColor = Color.gray;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
        UpdateVisuals();
    }

    void OnButtonClicked()
    {
        if (toggleType == AudioType.Music) UIAudioManager.Instance.ToggleMusic();
        else UIAudioManager.Instance.ToggleSFX();
        
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        bool isOn = (toggleType == AudioType.Music) ? UIAudioManager.Instance.isMusicOn : UIAudioManager.Instance.isSfxOn;

        iconImage.sprite = isOn ? onSprite : offSprite;
        backgroundImage.color = isOn ? onColor : offColor;
    }
}