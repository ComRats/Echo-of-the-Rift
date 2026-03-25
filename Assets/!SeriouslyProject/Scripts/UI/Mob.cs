using UnityEngine;

namespace EchoRift.UI
{
    [CreateAssetMenu(fileName = "VoiceProfile", menuName = "EchoRift/Voice Profile")]
    public class Mob : ScriptableObject
    {
        public string nickname;
        public string shortDescription;
        public string longDescription;
        public Sprite sprite;
    }
}
