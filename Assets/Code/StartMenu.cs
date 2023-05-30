using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
	[SerializeField] private TMP_InputField inputField;

	private static bool setRes;


    private void Awake()
    {
        if(setRes)
			return;

		setRes = true;
		Screen.fullScreen = false;
		Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
    }


    public void Load()
	{
		ProjectManager.projectName = inputField.text;
		ProjectManager.createNewProject = false;
		SceneManager.LoadScene(1);
	}

	public void Create()
	{
		ProjectManager.projectName = inputField.text;
		ProjectManager.createNewProject = true;
		SceneManager.LoadScene(1);
	}
}