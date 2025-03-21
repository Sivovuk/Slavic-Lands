using UnityEngine;

namespace UI.Game
{
    public class TabController : MonoBehaviour
    {
        public void OpenPanel(bool open)
        {
            GetComponentInParent<GameUIController>().OpenPanel(gameObject, open);
        }

        public void OpenTab(bool open)
        {
            GetComponentInParent<GameUIController>().OpenTab(gameObject, open);
        }
    }
}