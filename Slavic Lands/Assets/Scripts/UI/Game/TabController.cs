using UnityEngine;

namespace UI.Game
{
    public class TabController : MonoBehaviour
    {
        public void OpenPanel()
        {
            GetComponentInParent<GameUIController>().OpenPanel(gameObject, true);
        }
    }
}