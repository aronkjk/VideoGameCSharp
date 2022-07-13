using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelector : MonoBehaviour {
  public GameObject tokenPrefab;

  public Image imageExpo, imageMenu;
  private Sprite _currentSkin;
  public List<Image> expo_skins;

  public List<Toggle> toggle;

  private void Start() {
    _currentSkin = tokenPrefab.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;

    for (int i = 0; i < expo_skins.Count; i++) {
      if (expo_skins[i].sprite == imageExpo.sprite) {
        toggle[i].isOn = true;
      }
    }
  }

  private void Awake() {
    imageExpo.sprite = _currentSkin;
  }

  public void selectSkin(int pos) {
    for (int i = 0; i < expo_skins.Count; i++) {
      if (i == pos) {
        imageExpo.sprite = expo_skins[i].sprite;
      }
    }
  }

  public void equipSkin() {
    tokenPrefab.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = imageExpo.sprite;
    imageMenu.sprite = imageExpo.sprite;
    tokenPrefab.GetComponent<Token>().TypeSkin = 7;
    _currentSkin = imageExpo.sprite;

    for (int i = 0; i < expo_skins.Count; i++) {
      if (expo_skins[i].sprite == imageExpo.sprite) {
        toggle[i].isOn = true;
        tokenPrefab.GetComponent<Token>().TypeSkin = i;
      }
    }
  }
}


