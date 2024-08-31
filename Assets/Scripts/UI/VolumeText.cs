using UnityEngine;
using UnityEngine.UI;

public class VolumeText : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private string volumeName;
    [SerializeField] private string textIntro; // Sound: or Music:
    #endregion

    #region Private Fields
    private Text txt;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        InitializeComponents();
    }

    private void Update()
    {
        UpdateVolume();
    }
    #endregion

    #region Public Methods
    public void UpdateVolume()
    {
        float volumeValue = GetVolumeValue();
        UpdateVolumeText(volumeValue);
    }
    #endregion

    #region Private Methods
    private void InitializeComponents()
    {
        txt = GetComponent<Text>();
    }

    private float GetVolumeValue()
    {
        return PlayerPrefs.GetFloat(volumeName) * 100;
    }

    private void UpdateVolumeText(float volumeValue)
    {
        txt.text = $"{textIntro}{volumeValue:F0}";
    }
    #endregion
}