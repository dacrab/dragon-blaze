using UnityEngine;

namespace Core.Input
{
    public class InputProvider : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;

        private void Update()
        {
            if (inputReader != null)
            {
                inputReader.ProcessInput();
            }
        }
    }
}
