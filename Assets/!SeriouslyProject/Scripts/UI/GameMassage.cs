using UnityEngine;

public class GameMassage
{
    private static GameObject newMessage;

    public static void ButtonMassage(GameObject target, bool isShow, Sprite sprite, Vector3 offset = default)
    {
        if (isShow)
        {
            if (newMessage != null) return;

            newMessage = new GameObject("KeyMassage");
            newMessage.transform.position = target.transform.position + offset;
            newMessage.transform.localScale = new(2f, 2f, 0f);
            newMessage.transform.SetParent(target.transform);

            var sr = newMessage.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            sr.sortingLayerName = "Player";
            sr.sortingOrder = 500;
        }
        else
        {
            if (newMessage != null)
            {
                Object.Destroy(newMessage);
                newMessage = null;
            }
        }
    }


}
