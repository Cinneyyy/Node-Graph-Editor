using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorEditor : MonoBehaviour
{
	[SerializeField] private GameObject selectedIconMain, selectedIconAccent;
	[SerializeField] private Material[] sliderMats = new Material[3];
	[SerializeField] private Slider[] sliders = new Slider[3];
	[SerializeField] private TextMeshProUGUI mainColorDisplay, accentColorDisplay;

	[HideInInspector] public Node selectedNode;
	public Color mainColor
	{
		get => selectedNode.mainColor;
		set => selectedNode.mainColor = value;
	}
	public Color accentColor
	{
		get => selectedNode.accentColor;
		set => selectedNode.accentColor = value;
	}

	private bool editingMainColor;
	private Color color
	{
		get => editingMainColor ? mainColor : accentColor;
		set {
			if(editingMainColor) mainColor = value;
			else accentColor = value;
		}
	}
	private Color initMainColor, initAccentColor;

	public static ColorEditor instance { get; private set; }


    private void Awake()
		=> instance = this;

    private void Update()
    {
        if(selectedNode == null)
			return;

		color = new(sliders[0].value, sliders[1].value, sliders[2].value);

		mainColorDisplay.color = mainColor;
		accentColorDisplay.color = accentColor;

		UpdateSliderMats();
		selectedNode.UpdateColors();
    }

	private void UpdateSliderMats()
	{
		Color color = this.color;
        for(int i = 0; i < sliderMats.Length; i++)
        {
            Color col0 = color, col1 = color;

            col0[i] = 0f;
            col1[i] = 1f;

            sliderMats[i].SetColor("_Color0", col0);
            sliderMats[i].SetColor("_Color1", col1);
        }
    }


	public void SelectColorToEdit(bool mainColor)
	{
		if(ViewMode.active)
			return;

		editingMainColor = mainColor;

		selectedIconMain.SetActive(editingMainColor);
		selectedIconAccent.SetActive(!editingMainColor);

		Color col = color;
		for(int i = 0; i < sliders.Length; i++)
			sliders[i].value = col[i];

		UpdateSliderMats();
	}

	public void Done()
	{
		if(ViewMode.active)
			return;

		selectedNode.UpdateColors();
		gameObject.SetActive(false);
	}

	public void Cancel()
	{
		if(ViewMode.active)
			return;

		mainColor = initMainColor;
		accentColor = initAccentColor;
		selectedNode.UpdateColors();
		gameObject.SetActive(false);
	}

	public void OnOpen()
	{
		initMainColor = mainColor;
		initAccentColor = accentColor;

		SelectColorToEdit(true);
	}
}