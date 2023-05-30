using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Toolbar : MonoBehaviour
{
	public TextMeshProUGUI projectNameDisplay;
	public GameObject exitPopup, savedPopup;


    private void Start()
        => projectNameDisplay.text = ProjectManager.projectName;


    public void Save()
	{
		File.WriteAllText($"{Application.persistentDataPath}/{ProjectManager.projectName}.json", ProjectManager.GetJson());
		savedPopup.SetActive(true);
	}

	public void Exit()
		=> exitPopup.SetActive(true);

	public void EP_Cancel()
		=> exitPopup.SetActive(false);

	public void EP_SaveAndExit()
	{
		Save();
		SceneManager.LoadScene(0);
	}

	public void EP_ExitWithoutSaving()
		=> SceneManager.LoadScene(0);
}