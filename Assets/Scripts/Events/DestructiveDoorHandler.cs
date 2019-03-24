using UnityEngine;

public class DestructiveDoorHandler : MonoBehaviour
{

	public void DestroyMe()
    {
        GameObject.Destroy(this.gameObject);
    }

}
