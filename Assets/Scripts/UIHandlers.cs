using UnityEngine.SceneManagement;

public class UIHandlers : BaseMonoBehaviour
{
    public void UGUI_OnResetButtonPress()
    {
        SceneManager.LoadScene(0);
    }
}
