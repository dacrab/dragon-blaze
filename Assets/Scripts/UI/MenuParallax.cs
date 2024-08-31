using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private float offsetMultiplier = 1f;
    [SerializeField] private float smoothTime = 0.3f;
    #endregion

    #region Private Fields
    private Vector2 startPosition;
    private Vector3 velocity;
    #endregion

    #region Unity Lifecycle Methods
    private void Start()
    {
        InitializeStartPosition();
    }

    private void Update()
    {
        UpdateParallaxPosition();
    }
    #endregion

    #region Private Methods
    private void InitializeStartPosition()
    {
        startPosition = transform.position;
    }

    private void UpdateParallaxPosition()
    {
        Vector2 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 targetPosition = startPosition + (offset * offsetMultiplier);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
    #endregion
}
