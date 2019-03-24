using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDoorHandler : MonoBehaviour
{
    public void BossDoorEnter()
    {
        SceneManager.LoadScene("Boss");
    }
}
