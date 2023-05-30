using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
	[SerializeField] private TMP_InputField inputField;


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