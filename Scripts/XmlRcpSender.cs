using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XmlRpc;
using System.Xml;
using System.Text;
using System.IO;


public class XmlRcpSender : MonoBehaviour {
  public XmlRcpSender Instance { set; get; }

  public Player player;

  public GameObject errorUserExist, errorFailConection, errorFailLogin;

  private string Url = "http://192.168.0.102:8069/xmlrpc/2", db = "projectW", pass = "46800", user = "aron";

  public void CreateUser(string game_user, string user_pass) {
    XmlRpcClient client = new XmlRpcClient();
    client.Url = Url;
    client.Path = "common";


    try {
      XmlRpcRequest requestLogin = new XmlRpcRequest("authenticate");
      requestLogin.AddParams(db, user, pass, XmlRpcParameter.EmptyStruct());

      XmlRpcResponse responseLogin = client.Execute(requestLogin);

      client.Path = "object";

      XmlRpcRequest requestSearch = new XmlRpcRequest("execute_kw");
      requestSearch.AddParams(db, responseLogin.GetInt(), pass, "res.partner", "search_read",
        XmlRpcParameter.AsArray(
          XmlRpcParameter.AsArray(
            XmlRpcParameter.AsArray("game_user", "=", game_user)
          )
        ),
        XmlRpcParameter.AsStruct(
          XmlRpcParameter.AsMember("fields", XmlRpcParameter.AsArray("game_user"))
        )
      );

      XmlRpcResponse responseSearch = client.Execute(requestSearch);

      string[] separatingStrings = { "game_user: ", "}" };

      string[] words = responseSearch.GetString().Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

      for (int i = 0; i < words.Length; i++) {
        if ((words[i].Equals(game_user))) {
          errorUserExist.SetActive(true);
          return;
        }
      }

      client.Path = "object";

      XmlRpcRequest requestCreate = new XmlRpcRequest("execute_kw");
      requestCreate.AddParams(db, responseLogin.GetInt(), pass, "res.partner", "create",
        XmlRpcParameter.AsArray(
          XmlRpcParameter.AsStruct(
            XmlRpcParameter.AsMember("name", game_user),
            XmlRpcParameter.AsMember("game_user", game_user),
            XmlRpcParameter.AsMember("user_pass", user_pass),
            XmlRpcParameter.AsMember("is_player", true)
          )
        )
      );

      XmlRpcResponse responseCreate = client.Execute(requestCreate);
    } catch (Exception e) {
      errorFailConection.SetActive(true);
      Debug.Log("Loguin error: " + e);
    }
  }

  public void Login(string game_user, string user_pass) {
    XmlRpcClient client = new XmlRpcClient();
    client.Url = Url;
    client.Path = "common";


    try {
      XmlRpcRequest requestLogin = new XmlRpcRequest("authenticate");
      requestLogin.AddParams(db, user, pass, XmlRpcParameter.EmptyStruct());

      XmlRpcResponse responseLogin = client.Execute(requestLogin);

      client.Path = "object";

      XmlRpcRequest requestSearch = new XmlRpcRequest("execute_kw");
      requestSearch.AddParams(db, responseLogin.GetInt(), pass, "res.partner", "search_read",
        XmlRpcParameter.AsArray(
          XmlRpcParameter.AsArray(
            XmlRpcParameter.AsArray("is_player", "=", true),
            XmlRpcParameter.AsArray("game_user", "=", game_user),
            XmlRpcParameter.AsArray("user_pass", "=", user_pass)
          )
        ),
        XmlRpcParameter.AsStruct(
          XmlRpcParameter.AsMember("fields", XmlRpcParameter.AsArray("name", "user_name", "game_coins", "user_pass"))
        )
      );

      XmlRpcResponse responseSearch = client.Execute(requestSearch);

      if (responseSearch.GetString().Equals("[]")) {
        errorFailLogin.SetActive(true);
        return;
      }

      //Cadena que devuelve Odoo (mejor no mostrar en la consola)
      //Debug.Log(responseSearch.GetString());

      string[] separatingUser = { "name: ", "," };
      string[] separatingCoins = { "game_coins: ", "," };
      string[] separatingPass = { "user_pass: ", "}" };

      string[] wordsSubstractUser = responseSearch.GetString().Split(separatingUser, System.StringSplitOptions.RemoveEmptyEntries);
      string[] wordsSubstractCoins = responseSearch.GetString().Split(separatingCoins, System.StringSplitOptions.RemoveEmptyEntries);
      string[] wordsSubstractPass = responseSearch.GetString().Split(separatingPass, System.StringSplitOptions.RemoveEmptyEntries);

      player.setGame_user(wordsSubstractUser[2]);
      player.setGameCoins(int.Parse(wordsSubstractCoins[3]));
      player.setPass(wordsSubstractPass[1]);

    } catch (Exception e) {
      errorFailConection.SetActive(true);
      Debug.Log("Loguin error: " + e);
    }
  }

  public void BuyGameCoins(bool lowPurchase, bool highPurchase) {
    XmlRpcClient client = new XmlRpcClient();
    client.Url = Url;
    client.Path = "common";

    try {
      XmlRpcRequest requestLogin = new XmlRpcRequest("authenticate");
      requestLogin.AddParams(db, user, pass, XmlRpcParameter.EmptyStruct());

      XmlRpcResponse responseLogin = client.Execute(requestLogin);

      if (lowPurchase) {
        client.Path = "object";

        XmlRpcRequest requestCreateOrder = new XmlRpcRequest("execute_kw");
        requestCreateOrder.AddParams(db, user, pass, "sale.order", "create",
          XmlRpcParameter.AsArray(
            XmlRpcParameter.AsStruct(
              XmlRpcParameter.AsMember("partner_id", player.getGameUser()),
              XmlRpcParameter.AsMember("product_uom", 1),
              XmlRpcParameter.AsMember("bom_id", 30),
              XmlRpcParameter.AsMember("product_qty", 1.0)
            )
          )
        );

        XmlRpcResponse responseCreateOrder = client.Execute(requestCreateOrder);

        client.Path = "object";

        XmlRpcRequest requestCreateOrderLine = new XmlRpcRequest("execute_kw");
        requestCreateOrderLine.AddParams(db, user, pass, "sale.order.line", "create",
          XmlRpcParameter.AsArray(
            XmlRpcParameter.AsStruct(
              XmlRpcParameter.AsMember("name", "New Purchase"),
              XmlRpcParameter.AsMember("product_qty", 1),
              XmlRpcParameter.AsMember("date_planned", "00/00/0000"),
              XmlRpcParameter.AsMember("order_id", "new_order")
            )
          )
        );

        XmlRpcResponse responseCreateOrderLine = client.Execute(requestCreateOrderLine);
      }

      if (highPurchase) {
        client.Path = "object";

        XmlRpcRequest requestCreateOrder = new XmlRpcRequest("execute_kw");
        requestCreateOrder.AddParams(db, user, pass, "sale.order", "create",
          XmlRpcParameter.AsArray(
            XmlRpcParameter.AsStruct(
              XmlRpcParameter.AsMember("partner_id", player.getGameUser()),
              XmlRpcParameter.AsMember("product_uom", 1),
              XmlRpcParameter.AsMember("bom_id", 31),
              XmlRpcParameter.AsMember("product_qty", 1.0)
            )
          )
        );

        XmlRpcResponse responseCreateOrder = client.Execute(requestCreateOrder);

        client.Path = "object";

        XmlRpcRequest requestCreateOrderLine = new XmlRpcRequest("execute_kw");
        requestCreateOrderLine.AddParams(db, user, pass, "sale.order.line", "create",
          XmlRpcParameter.AsArray(
            XmlRpcParameter.AsStruct(
              XmlRpcParameter.AsMember("name", "NAME TEXT"),
              XmlRpcParameter.AsMember("product_qty", 1),
              XmlRpcParameter.AsMember("date_planned", "......."),
              XmlRpcParameter.AsMember("order_id", "new_order")
            )
          )
        );

        XmlRpcResponse responseCreateOrderLine = client.Execute(requestCreateOrderLine);
      }
    } catch (Exception e) {
      Debug.Log("PurchaseManager ERROR: " + e);
    }
  }
}
