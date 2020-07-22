using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityScrollbar : MonoBehaviour {
	VerticalLayoutGroup group;
  Scrollbar bar;
  private void Start()
  {
    group = GetComponent<VerticalLayoutGroup>();
    bar = transform.parent.Find("Scrollbar").GetComponent<Scrollbar>();
    bar.numberOfSteps = 256;
  }
  private void Update()
  {
    if (bar.IsInteractable()){
      bar.value -= (Input.mouseScrollDelta.y * Time.deltaTime * 6f);
      bar.value = Mathf.Clamp01(bar.value);
    }
  }

  public void UpdateScroll(){
    RectOffset offsetTemp = new RectOffset(group.padding.left, group.padding.right, group.padding.top, group.padding.bottom);
    offsetTemp.top = (int)(-bar.value * (60 * (group.transform.childCount - 1.25)));
    group.padding = offsetTemp;
  }
}
