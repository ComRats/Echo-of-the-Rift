using UnityEngine;

namespace EchoRift.UI
{
    [CreateAssetMenu(fileName = "Mob", menuName = "EchoRift/Mobs")]
    public class Mob : ScriptableObject
    {
        public string nickname;
        public string shortDescription;
        [TextArea]
        public string longDescription;
        public Sprite sprite;
    }
}
