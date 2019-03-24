using UnityEngine;

public class DoubleButtonHandler : MonoBehaviour
{
    private bool hitButtonUp;
    private bool hitButtonDown;

	public void DoHitButtonUp()
	{
	    this.hitButtonUp = true;
	}
    
    public void DoHitButtonDown()
    {
        this.hitButtonDown = true;
    }

    private void Update()
    {
        if (hitButtonDown && hitButtonUp)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            hitButtonDown = false;
            hitButtonUp = false;
        }
    }
}
