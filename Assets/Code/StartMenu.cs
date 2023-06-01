using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
	[SerializeField] private TMP_InputField inputField;


    public void Load()
	{
		if(string.IsNullOrEmpty(inputField.text))
			return;

		ProjectManager.projectName = inputField.text;
		ProjectManager.createNewProject = false;
		SceneManager.LoadScene(1);
	}

	public void Create()
	{
        if(string.IsNullOrEmpty(inputField.text))
            return;

        ProjectManager.projectName = inputField.text;
		ProjectManager.createNewProject = true;
		SceneManager.LoadScene(1);
	}
}