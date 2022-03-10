using R2API;
using UnityEngine;

namespace KookehsDropItemMod
{
    public class DropItemNotification : MonoBehaviour
    {
        private Notification notification;
        private float timeToDestroy = 3.0f;

        void Update()
        {
            if (notification != null)
            {
                timeToDestroy -= Time.deltaTime;
                if (timeToDestroy < 0)
                {
                    Destroy(notification);
                }
            }
        }

        private void OnDestroy()
        {
            if (notification != null)
            {
                Destroy(notification);
            }
        }

        public void SetNotification(string title, string description, Texture texture)
        {
            if (notification == null)
            {
                notification = gameObject.AddComponent<Notification>();
                notification.tag = "DropItem";
                notification.transform.SetParent(transform);
                notification.SetPosition(new Vector3((float)(Screen.width * 0.8), (float)(Screen.height * 0.25), 0));
            }
            notification.SetIcon(texture);
            notification.GetTitle = () => title;
            notification.GetDescription = () => description;
            timeToDestroy = 3.0f;
        }
    }
}
