using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Toolbar : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI projectNameDisplay;
	[SerializeField] private GameObject exitPopup, savedPopup;

	public static Toolbar instance { get; private set; }


    private void Awake()
        => instance = this;


	public void SetProjName(string name)
		=> projectNameDisplay.text = name;

    public void Save()
	{
		File.WriteAllText($"{Application.persistentDataPath}/{ProjectManager.projectName}.json", ProjectManager.GetJson());
		savedPopup.SetActive(true);
	}

	public void Exit()
		=> exitPopup.SetActive(true);

	public void EnterViewMode()
		=> ViewMode.EnterViewMode();

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