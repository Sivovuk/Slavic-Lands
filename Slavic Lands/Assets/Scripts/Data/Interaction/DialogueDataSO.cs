using UnityEngine;

namespace Data.Interaction
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Interaction/Dialogue")]
    public class DialogueDataSO : ScriptableObject
    {
        [TextArea(2,5)]
        public string[] Sentences;
    }
}