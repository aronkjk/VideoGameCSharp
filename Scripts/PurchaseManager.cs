using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseManager : MonoBehaviour {
  public List<GameObject> samllItems;
  public GameObject bigItem, panelInsufficientCoins, PanelPurchaseGameCoins;
  public Player player;
  public XmlRcpSender xmlrcp;
  public Text txt_cash;
  private Text price;

  public void setPurchase(int typeAction) {
    price = transform.GetChild(4).GetComponent<Text>();

    if (typeAction == 4) {
      transform.GetChild(1).gameObject.SetActive(false);
      transform.GetChild(2).gameObject.SetActive(true);

      for (int i = 0; i < bigItem.transform.GetChild(1).childCount; i++) {
        transform.GetChild(2).GetChild(i).GetComponent<Image>().sprite = bigItem.transform.GetChild(1).GetChild(i).GetComponent<Image>().sprite;
      }

      price.text = bigItem.transform.GetChild(3).GetChild(0).GetComponent<Text>().text;
    } else {
      for (int i = 0; i < samllItems.Count; i++) {
        if (typeAction == i) {
          transform.GetChild(1).gameObject.SetActive(true);
          transform.GetChild(2).gameObject.SetActive(false);

          transform.GetChild(1).GetComponent<Image>().sprite = samllItems[i].transform.GetChild(0).GetComponent<Image>().sprite;
          price.text = samllItems[i].transform.GetChild(1).GetChild(0).GetComponent<Text>().text;
        }
      }
    }

    if (int.Parse(price.text) > player.getGame_coins()) {
      panelInsufficientCoins.SetActive(true);
    } else {
      if (panelInsufficientCoins.activeSelf) panelInsufficientCoins.SetActive(false);
    }
  }

  public void unlockSkin() {
    if (int.Parse(price.text) < player.getGame_coins()) {
      int substractPrice = player.getGame_coins() - int.Parse(price.text);
      player.setGameCoins(substractPrice);
      txt_cash.text = player.getGame_coins().ToString();
    }
  }

  public void buyCoins() {
    xmlrcp.BuyGameCoins(true, false);
  }

  public void goPurchaseGameCoins(bool open) {
    PanelPurchaseGameCoins.SetActive(open);
  }
}
