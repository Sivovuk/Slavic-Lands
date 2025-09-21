using UnityEngine;

namespace Data.Interaction
{
    /// <summary>
    /// ScriptableObject to store dialogue data for NPCs, interactions, etc.
    /// Allows designers to input multiple dialogue lines directly in the editor.
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Interaction/Dialogue")]
    public class DialogueDataSO : ScriptableObject
    {
        // Array of sentences that make up the dialogue.
        // TextArea allows editing multiple lines per entry in the inspector (min 2, max 5 lines shown)
        [TextArea(2,5)]
        public string[] Sentences;
    }
}