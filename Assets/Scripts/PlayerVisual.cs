using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    public void PlaySound(string name)
    {
        AudioManager.Instance.PlaySfx(name);
    }
}
