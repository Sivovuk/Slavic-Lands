using UnityEngine;

namespace UI.Game
{
    public class PanelAction : MonoBehaviour
    {
        public void OpenPanel()
        {
            GetComponentInParent<GameUIController>().OpenPanel(gameObject, true);
        }
    }
}