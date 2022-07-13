using System.Collections.Generic;
using UnityEngine;

public class BoardHighLight : MonoBehaviour {
  public static BoardHighLight Instance { set; get; }

  private static int WIDGHT = BoarManager.WIDGHT, HEIGHT = BoarManager.HEIGHT;

  public GameObject highlightPrefabMove,
    highlightPrefabAttack,
    highlightPrefabFusion,
    highlightsPrefabMovePattern,
    highlightsPrefabAttackPattern,
    highlightsPrefabFusionPattern,
    highlightCurrentPos,
    highlightsPrefabAllowedInvoke,
    highlightsPrefabSelectedToken,
    highlightPrefabCure_Stun,
    highlightPrefabTeleport,
    highlightPrefabObjectiveTeleport,
    highlightPrefabFootprints;

  private List<GameObject> highlightsMove,
    highlightsAttack,
    highlightsFusion,
    highlightsMovePattern,
    highlightsAttackPattern,
    highlightsFusionPattern,
    highlightsAllowedInvoke,
    highlightsSelectedToken,
    highlightsCure_Stun,
    highlightsTeleport,
    highlightsObjectiveTeleport,
    highlightsFootprints;

  private void Start() {
    Instance = this;
    highlightsAllowedInvoke = new List<GameObject>();
    highlightsMove = new List<GameObject>();
    highlightsAttack = new List<GameObject>();
    highlightsFusion = new List<GameObject>();

    highlightsMovePattern = new List<GameObject>();
    highlightsAttackPattern = new List<GameObject>();
    highlightsFusionPattern = new List<GameObject>();
    highlightsSelectedToken = new List<GameObject>();
    highlightsCure_Stun = new List<GameObject>();
    highlightsTeleport = new List<GameObject>();
    highlightsObjectiveTeleport = new List<GameObject>();
    highlightsFootprints = new List<GameObject>();
  }

  private GameObject GetHighlightObjectMove() {
    GameObject go = highlightsMove.Find(g => !g.activeSelf);

    if (go == null) {
      go = Instantiate(highlightPrefabMove);
      highlightsMove.Add(go);
    }

    return go;
  }

  private GameObject GetHighlightObjectAttack() {
    GameObject go = highlightsAttack.Find(g => !g.activeSelf);

    if (go == null) {
      go = Instantiate(highlightPrefabAttack);
      highlightsAttack.Add(go);
    }

    return go;
  }

  private GameObject GetHighlightObjectFusion() {
    GameObject go = highlightsFusion.Find(g => !g.activeSelf);

    if (go == null) {
      go = Instantiate(highlightPrefabFusion);
      highlightsFusion.Add(go);
    }

    return go;
  }

  private GameObject GetHighlightObjectPatternMove() {
    GameObject go = highlightsMovePattern.Find(g => !g.activeSelf);

    if (go == null) {
      go = Instantiate(highlightsPrefabMovePattern);
      highlightsMovePattern.Add(go);
    }

    return go;
  }

  private GameObject GetHighlightObjectPatternAttack() {
    GameObject go = highlightsAttackPattern.Find(g => !g.activeSelf);

    if (go == null) {
      go = Instantiate(highlightsPrefabAttackPattern);
      highlightsAttackPattern.Add(go);
    }

    return go;
  }

  private GameObject GetHighlightObjectPatternFusion() {
    GameObject go = highlightsFusionPattern.Find(g => !g.activeSelf);

    if (go == null) {
      go = Instantiate(highlightsPrefabFusionPattern);
      highlightsFusionPattern.Add(go);
    }

    return go;
  }

  private GameObject GetHighlightAllowedInvoke() {
    GameObject go = highlightsAllowedInvoke.Find(g => !g.activeSelf);

    if (go == null) {
      go = Instantiate(highlightsPrefabAllowedInvoke);
      highlightsAllowedInvoke.Add(go);
    }

    return go;
  }

  private GameObject GetHighlightSelectedToken() {
    GameObject go = highlightsSelectedToken.Find(g => !g.activeSelf);

    if (go == null) {
      go = Instantiate(highlightsPrefabSelectedToken);
      highlightsSelectedToken.Add(go);
    }

    return go;
  }

  private GameObject GetHighlightCure_Stun() {
    GameObject go = highlightsCure_Stun.Find(g => !g.activeSelf);

    if (go == null) {
      go = Instantiate(highlightPrefabCure_Stun);
      highlightsCure_Stun.Add(go);
    }

    return go;
  }

  private GameObject GetHighlightTeleport() {
    GameObject go = highlightsTeleport.Find(g => !g.activeSelf);

    if (go == null) {
      go = Instantiate(highlightPrefabTeleport);
      highlightsTeleport.Add(go);
    }

    return go;
  }

  private GameObject GetHighlightObjectiveTeleport() {
    GameObject go = highlightsObjectiveTeleport.Find(g => !g.activeSelf);

    if (go == null) {
      go = Instantiate(highlightPrefabObjectiveTeleport);
      highlightsTeleport.Add(go);
    }

    return go;
  }

  private GameObject GetHighlightFootprints() {
    GameObject go = highlightsFootprints.Find(g => !g.activeSelf);

    if (go == null) {
      go = Instantiate(highlightPrefabFootprints);
      highlightsFootprints.Add(go);
    }

    return go;
  }

  public void HighlightSelectedToken(int x, int y) {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (i == x && j == y) {
          GameObject go = GetHighlightSelectedToken();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        }
      }
    }
  }

  public void HighlightAllowedInvokeP1() {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (j == 0) {
          GameObject go = GetHighlightAllowedInvoke();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        } else if (j < BoarManager.Instance.numTurn / j) {
          GameObject go = GetHighlightAllowedInvoke();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        }
      }
    }
  }

  public void HighlightAllowedInvokeP2() {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (j > 0 && j >= HEIGHT - BoarManager.Instance.numTurn / (HEIGHT - j) - 1) {
          GameObject go = GetHighlightAllowedInvoke();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        }
      }
    }
  }

  public void HighlightAllowedMoves(bool[,] moves) {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (moves[i, j]) {
          GameObject go = GetHighlightObjectMove();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        }
      }
    }
  }

  public void HighlightAllowedAttack(bool[,] attack) {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (attack[i, j]) {
          GameObject go = GetHighlightObjectAttack();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        }
      }
    }
  }

  public void HighlightAllowedFusion(bool[,] fusion) {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (fusion[i, j]) {
          GameObject go = GetHighlightObjectFusion();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        }
      }
    }
  }

  public void HighlightAllowedCure_Stun(bool[,] cure_stun) {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (cure_stun[i, j]) {
          GameObject go = GetHighlightCure_Stun();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        }
      }
    }
  }

  public void HighlightAllowedMovesPattern(bool[,] movesPattern) {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (movesPattern[i, j]) {
          GameObject go = GetHighlightObjectPatternMove();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        }
      }
    }
  }

  public void HighlightAllowedAttackPattern(bool[,] attackPattern) {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (attackPattern[i, j]) {
          GameObject go = GetHighlightObjectPatternAttack();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        }
      }
    }
  }

  public void HighlightAllowedFusionPattern(bool[,] fusionPattern) {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (fusionPattern[i, j]) {
          GameObject go = GetHighlightObjectPatternFusion();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        }
      }
    }
  }

  public void HighlightAllowedTeleport(bool[,] moves) {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (moves[i, j]) {
          GameObject go = GetHighlightTeleport();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        }
      }
    }
  }

  public void HighlightObjectiveTeleport(bool[,] objective) {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (objective[i, j]) {
          GameObject go = GetHighlightObjectiveTeleport();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
        }
      }
    }
  }

  public void HighlightAttackPattern(bool[,] attackPattern) {
    for (int i = 0; i < HEIGHT; i++) {
      for (int j = 0; j < WIDGHT; j++) {
        if (attackPattern[i, j]) {
          GameObject go = GetHighlightObjectPatternAttack();
          go.SetActive(true);
          go.transform.position = new Vector3(-580 + i + 0.5f, -5 + j + 0.5f, 6);
          go.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }
      }
    }
  }

  public void HighlightMovementPattern(bool[,] movesPattern) {
    for (int i = 0; i < HEIGHT; i++) {
      for (int j = 0; j < WIDGHT; j++) {
        if (movesPattern[i, j]) {
          GameObject go = GetHighlightObjectPatternMove();
          go.SetActive(true);
          go.transform.position = new Vector3(-580 + i + 0.5f, -5 + j + 0.5f, 6);
          go.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }
      }
    }
  }

  public void HighlightFusionPattern(bool[,] fusionPattern) {
    for (int i = 0; i < HEIGHT; i++) {
      for (int j = 0; j < WIDGHT; j++) {
        if (fusionPattern[i, j]) {
          GameObject go = GetHighlightObjectPatternFusion();
          go.SetActive(true);
          go.transform.position = new Vector3(-580 + i + 0.5f, -5 + j + 0.5f, 6);
          go.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }
      }
    }
  }

  public void HighlightFootpreints(bool[,] trails) {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (trails[i, j]) {
          GameObject go = GetHighlightFootprints();
          go.SetActive(true);
          go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
          go.transform.rotation = Quaternion.Euler(0f, Random.Range(170, 200), 0f);
        }
      }
    }
  }

  public void HideHighlights() {
    foreach (GameObject go in highlightsMove) {
      go.SetActive(false);
    }

    foreach (GameObject go in highlightsAttack) {
      go.SetActive(false);
    }

    foreach (GameObject go in highlightsFusion) {
      go.SetActive(false);
    }

    foreach (GameObject go in highlightsMovePattern) {
      go.SetActive(false);
    }

    foreach (GameObject go in highlightsAttackPattern) {
      go.SetActive(false);
    }

    foreach (GameObject go in highlightsFusionPattern) {
      go.SetActive(false);
    }

    foreach (GameObject go in highlightsCure_Stun) {
      go.SetActive(false);
    }

    foreach (GameObject go in highlightsTeleport) {
      go.SetActive(false);
    }

    foreach (GameObject go in highlightsObjectiveTeleport) {
      go.SetActive(false);
    }

    foreach (GameObject go in highlightsFootprints) {
      go.SetActive(false);
    }
  }

  public void hideHighLightsAllowdInvoke() {
    foreach (GameObject go in highlightsAllowedInvoke) {
      go.SetActive(false);
    }
  }

  public void hideHighLightsSelectedToken() {
    foreach (GameObject go in highlightsSelectedToken) {
      go.SetActive(false);
    }
  }

  public void hideHighLightsCure_Stun() {
    foreach (GameObject go in highlightsCure_Stun) {
      go.SetActive(false);
    }
  }
}
