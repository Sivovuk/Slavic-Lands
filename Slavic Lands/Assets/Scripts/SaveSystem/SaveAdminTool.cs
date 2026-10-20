using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// Development-only save utilities. Add this component to a scene object when testing.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public sealed class SaveAdminTool : MonoBehaviour
    {
        [field: SerializeField]
        public bool ResetingSaveDataOnLoad { get; private set; }

        private void Awake()
        {
            if (!ResetingSaveDataOnLoad)
                return;

            SaveManager.DeleteAllSavedData();
            ResetingSaveDataOnLoad = false;
        }
    }
}
