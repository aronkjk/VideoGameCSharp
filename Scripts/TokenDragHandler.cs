using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class TokenDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler {
  public BoarManager Board;
  public Text NumberCommonPool, NumberPoolP1, NumberPoolP2;
  public GameObject highlightSelected;
  public GameObject tokenP1, tokenP2, pos;
  GameObject instantiate;
  public int typeToken;
  private Vector3 screenPoint;
  private Vector3 offset;

  public void OnDrag(PointerEventData eventData) {
    if (!instantiate) return;
    Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
    Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
    instantiate.transform.position = curPosition;
  }

  public void OnEndDrag(PointerEventData eventData) {
    int iNum = Int32.Parse(NumberCommonPool.text);
    int iNumP1 = Int32.Parse(NumberPoolP1.text);
    int iNumP2 = Int32.Parse(NumberPoolP2.text);

    if (iNumP1 > 0 && BoarManager.Instance.turn) {
      BoarManager.Instance.SpawnToken(typeToken, NumberPoolP1);
    } else if (iNumP2 > 0 && !BoarManager.Instance.turn) {
      BoarManager.Instance.SpawnToken(typeToken, NumberPoolP2);
    } else {
      BoarManager.Instance.SpawnToken(typeToken, NumberCommonPool);
    }
  }

  public void OnPointerDown(PointerEventData eventData) {
    if (Input.touches.Length > 1) return;

    if (BoarManager.Instance.turn) instantiate = Instantiate(tokenP1, pos.transform.position, Quaternion.identity) as GameObject;
    else instantiate = Instantiate(tokenP2, pos.transform.position, Quaternion.identity) as GameObject;

    screenPoint = Camera.main.WorldToScreenPoint(instantiate.transform.position);
    offset = instantiate.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

    BoarManager.Instance.UnselectToken();
    highlightSelected.SetActive(true);

    if (BoarManager.Instance.turn) {
      BoardHighLight.Instance.HighlightAllowedInvokeP1();
    } else {
      BoardHighLight.Instance.HighlightAllowedInvokeP2();
    }
  }

  public void OnPointerUp(PointerEventData eventData) {
    Destroy(instantiate);
    highlightSelected.SetActive(false);
    BoardHighLight.Instance.hideHighLightsAllowdInvoke();
  }
}
