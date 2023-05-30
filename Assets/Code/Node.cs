using TMPro;
using UnityEngine;

[ExecuteAlways]
public class Node : MonoBehaviour
{
	[Header("Rect Transforms")]
	[SerializeField] private RectTransform nodeRect;
	[SerializeField] private RectTransform titleRect, descRect;
	[Header("Input Fields")]
	[SerializeField] private TMP_InputField titleField;
	[SerializeField] private TMP_InputField descField;
	[Header("TMPs")]
	[SerializeField] private TextMeshProUGUI titleTmp;
	[SerializeField] private TextMeshProUGUI descTmp;

	public string title
	{
		get => titleField.text;
		set => titleField.text = value;
	}


	private void Update()
	{
		UpdateVisuals();
	}

#if false
	private IEnumerator SmoothlyChangeSize(float time, Vector2 newSize)
	{
		Vector2 delta = newSize - nodeRect.sizeDelta, current = nodeRect.sizeDelta;
		float timePassed = 0f;

		while(timePassed <= time)
		{
			timePassed += Time.deltaTime;

			current += delta * Time.deltaTime;
			SetFinalSize(current);

			yield return null;
		}

		SetFinalSize(newSize);
	}
#endif

	private void SetFinalSize(Vector2 newSize)
	{
		nodeRect.sizeDelta = newSize;
		titleRect.sizeDelta = new(newSize.x, titleRect.sizeDelta.y);
	}


	public void UpdateVisuals()
	{
		Vector2 rv_desc = descTmp.GetRenderedValues(), rv_title = titleTmp.GetRenderedValues();

		Vector2 size = new(Mathf.Max(250f, rv_title.x + 50f, rv_desc.x + 50f),
						   Mathf.Max(125f, rv_desc.y + titleRect.sizeDelta.y + 25f));

		SetFinalSize(size);
    }
}