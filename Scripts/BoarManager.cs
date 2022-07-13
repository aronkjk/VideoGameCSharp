using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Android;
using System.Collections;
using System.Linq;

public class BoarManager : MonoBehaviour {
  public SFX_playing sound;
  public static BoarManager Instance { set; get; }
  public static int WIDGHT = 7, HEIGHT = 12;
  private bool[,] allowedMoves { set; get; }
  private bool[,] allowedAttack { set; get; }
  private bool[,] allowedFusion { set; get; }
  private bool[,] Cure_Stun { set; get; }

  private bool[,] movementPattern { set; get; }
  private bool[,] attackPattern { set; get; }
  private bool[,] fusionPattern { set; get; }
  public bool[,] footprints { set; get; }


  private const float TILE_SIZE = 1.0f;
  private const float TILE_OFFSET = 0.5f;

  private int selectionX = -1;
  private int selectionY = -1;

  public bool spawnPosLock { get; set; }
  public bool spawnLimitLock { get; set; }
  public bool spawnPoolLock { get; set; }
  public bool onMouseOverMenuHandler { get; set; }

  public float slotTime;

  public List<GameObject> tokensP1;
  public List<GameObject> tokensP2;

  public List<GameObject> activeTokens;
  private List<GameObject> inactiveTokens;
  private List<GameObject> activeTokensP1;
  private List<GameObject> activeTokensP2;

  public List<GameObject> txtRestTokens, txtRestTokensP1, txtRestTokensP2;
  public GameTimer timer;

  public Token[,] TokensInBoard { set; get; }
  private Token[,] DeadTokens { set; get; }

  private Token selectedToken, teleported, portal_base;

  private GameObject PosibleTokenFusion;
  public List<GameObject> PosibleTokensFusion { get; set; }

  public List<Slider> timeBarsP1;
  public List<Slider> timeBarsP2;
  public CountdownTurn timesP1;
  public CountdownTurn timesP2;

  private int x = -1, y = -1, typeToken = -1;

  float time;

  Color color_nullAction = new Color32(195, 195, 195, 150);
  Color color_moveAction = new Color32(255, 239, 73, 150);
  Color color_attackAction = new Color32(255, 78, 78, 150);
  Color color_fusionAction = new Color32(195, 76, 255, 150);
  Color color_specialAction = new Color32(61, 255, 247, 150);

  public int oneLessMaxSpawns;
  public int spawnsP1, spawnsP2;

  public bool turn { get; set; }
  public bool timing { get; set; }
  public int numTurn { get; set; }

  private Vector3 position;
  private float width;
  private float height;
  private int palabra;

  public Animator TokensMenuAnimator;

  [Header("Panels Actions")]
  public GameObject panelAttack;
  public GameObject panelMove, panelFusion, panelSpecialAbility;
  public GameObject panelTokenInfo, panelPossess, panelExpanse, panelGoUnestuned, panelInfoSpecialAbility;



  void Awake() {
    width = (float)Screen.width / 2.0f;
    height = (float)Screen.height / 2.0f;
    position = new Vector3(0.0f, 0.0f, 0.0f);
  }

  public void Start() {
    activeTokens = new List<GameObject>();
    inactiveTokens = new List<GameObject>();
    PosibleTokensFusion = new List<GameObject>();
    TokensInBoard = new Token[WIDGHT, HEIGHT];
    footprints = new bool[WIDGHT, HEIGHT];

    RestartGame();
    time = 2.12f;
    numTurn = 0;
    sound.playBattleMusic();
  }

  private void Update() {
    UpdateSelection(Camera.main);
    DrawBoard();

    if (timing) {
      time = time - (1 * Time.deltaTime);

      if (time <= 0) {
        EndTurn();
      }
    }

    if (Input.GetMouseButtonDown(0)) {
      if (selectionX >= 0 && selectionY >= 0) {
        if (selectedToken == null) {
          SelectToken(selectionX, selectionY);
        } else if (selectedToken.typeToken == 12 && teleported) {
          Teleport(selectionX, selectionY);
        } else if (selectedToken.action >= 0) {
          switch (selectedToken.action) {
          case 0:
          MoveToken(selectionX, selectionY);
          break;
          case 1:
          Attack(selectionX, selectionY);
          break;
          case 2:
          FusionateToken(selectionX, selectionY);
          break;
          case 3:
          SpecialAbility(selectionX, selectionY);
          break;
          }
        } else {
          if (onMouseOverMenuHandler || teleported) return;
          UnselectToken();
          SelectToken(selectionX, selectionY);
        }
      } else {
        if (onMouseOverMenuHandler) return;
        UnselectToken();
      }
    }

    if (selectedToken) {
      if (x != selectionX || y != selectionY) {
        x = selectionX;
        y = selectionY;

        if (selectionX >= 0 && selectionY >= 0) {
          if (typeToken == 6 && selectedToken.action == 1) {
            DrawAttackCatapult();
          }
          if (typeToken == 15 && selectedToken.action == 1) {
            DrawAttackVulcan();
          }
          if (typeToken == 10 && selectedToken.action == 1) {
            DrawAttackDragon();
          }
          if (typeToken < 5 && selectedToken.action == 2) {
            ShowTokenFusionated();
          }
          if (typeToken == 5 && selectedToken.action == 4) {
            BoardHighLight.Instance.hideHighLightsCure_Stun();

            Cure_Stun = selectedToken.Cure_Stun(x, y);

            BoardHighLight.Instance.HighlightAllowedCure_Stun(Cure_Stun);
          }
        }
      }
    }
  }

  public void RestartGame() {
    Instance = this;

    UI_Controler.instance.setPause(false);

    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        TokensInBoard[i, j] = null;
      }
    }

    spawnPosLock = true;
    spawnLimitLock = true;
    spawnPoolLock = true;
    UI_Controler.instance.setCountdown(true);

    foreach (GameObject token in activeTokens) {
      Destroy(token);
    }

    activeTokens = new List<GameObject>();

    foreach (GameObject item in txtRestTokens) {
      item.GetComponent<Text>().text = "5";
    }
    foreach (GameObject item in txtRestTokensP1) {
      item.GetComponent<Text>().text = "0";
    }
    foreach (GameObject item in txtRestTokensP2) {
      item.GetComponent<Text>().text = "0";
    }

    turn = true;

    turn = true;
    timing = false;
    numTurn = 0;

    spawnsP1 = 1;
    spawnsP2 = 0;

    timesP1.time1 = slotTime;
    timesP1.time2 = 0;
    timesP1.time3 = 0;
    timesP1.time4 = 0;

    timesP2.time1 = slotTime;
    timesP2.time2 = 0;
    timesP2.time3 = 0;
    timesP2.time4 = 0;

    for (int i = 0; i < timeBarsP1.Count; i++) {
      timeBarsP1[i].value = 0;
      timeBarsP2[i].value = 0;
    }
    timeBarsP1[0].value = 15;
    timeBarsP2[0].value = 15;

    TokenInstance(tokensP1, 21, 3, 0);
    TokenInstance(tokensP2, 21, 3, HEIGHT - 1);
  }

  public void StartEndTurn() {
    timing = timing ? !timing : timing;
  }

  public void EndTurn() {
    turn = !turn;
    ResetCountDown();
    canvasManager.manageCanvasElements();
    timer.ManageTimePlayer();
    ManageTokens();
  }

  private void ResetCountDown() {
    timing = false;
    time = 2.12f;
    timeText.text = "2";
    timeText.gameObject.SetActive(false);
    if (numTurn <= 42) numTurn++;
  }

  private void ManageTokens() {
    if (selectedToken) UnselectToken();

    for (int i = 0; i < activeTokens.Count; i++) {
      Token t = activeTokens[i].GetComponent<Token>();

      if (t.burned) {
        t.burnedCountdown--;

        if (t.burnedCountdown < 0) {
          t.setLife(-1);
          t.burned = false;
          t.transform.GetChild(5).GetChild(2).gameObject.SetActive(false);

          if (t.life <= 0) TokenDies(t);
        }
      }

      if (activeTokens[i] && t.zenMode) {
        t.zenMode = false;
        t.transform.GetChild(5).GetChild(3).gameObject.SetActive(false);
      }

      if (activeTokens[i] && t.possessed) {
        t.possessed = false;
        t.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
      }

      if (activeTokens[i]) {
        t.movedOnce = false;
        t.attackOnce = false;
        t.possessed = false;

        if (t.maxActionPoints - t.actionPoints > 0) {
          t.setActionPoints(1);
        }
      }
    }
  }

  private void setAlphaTokens(float value) {
    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        if (TokensInBoard[i, j] && TokensInBoard[i, j] != selectedToken) {
          Color spriteColor = TokensInBoard[i, j].gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().color;
          spriteColor.a = value;
          TokensInBoard[i, j].gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().color = spriteColor;
        }
      }
    }
  }

  public bool avalibleAction(bool turn, bool isEnemy, bool isPossessed) {
    return (turn && isEnemy || !turn && !isEnemy) && !isPossessed;
  }

  public bool attck() {
    return true;
  }

  public void SelectToken(int x, int y) {
    onMouseOverMenuHandler = false;

    if (TokensInBoard[x, y] == null) return;

    BoardHighLight.Instance.HighlightSelectedToken(x, y);

    selectedToken = TokensInBoard[x, y];

    typeToken = selectedToken.typeToken;

    setAlphaTokens(0.5f);

    tokenMenu.SetActive(true);

    if (selectedToken.stunned) panelGoUnestuned.SetActive(true);
    else {
      if (selectedToken.doubleToken) {
        if (selectedToken.typeToken == 9) panelPossess.SetActive(true);
        else if (selectedToken.typeToken != 12 && selectedToken.typeToken != 16) panelAttack.SetActive(true);

        if (selectedToken.typeToken == 16) panelExpanse.SetActive(true);
        else if (selectedToken.typeToken != 12 && selectedToken.typeToken != 15) panelMove.SetActive(true);

        panelSpecialAbility.SetActive(true);
      } else {
        panelAttack.SetActive(true);
        panelMove.SetActive(true);
        panelFusion.SetActive(true);
      }
    }

    panelTokenInfo.SetActive(true);
    panelTokenInfo.transform.GetChild(0).GetComponent<Image>().sprite = selectedToken.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;
    panelTokenInfo.transform.GetChild(1).GetComponent<Text>().text = selectedToken.name;

    if (selectedToken.actionPoints <= 0 || selectedToken.isEnemy == turn) {

      panelAttack.GetComponent<Image>().color = color_nullAction;
      panelMove.GetComponent<Image>().color = color_nullAction;
      panelFusion.GetComponent<Image>().color = color_nullAction;
      panelSpecialAbility.GetComponent<Image>().color = color_nullAction;
      panelPossess.GetComponent<Image>().color = color_nullAction;
      panelExpanse.GetComponent<Image>().color = color_nullAction;
      panelGoUnestuned.GetComponent<Image>().color = color_nullAction;
    } else {
      panelAttack.GetComponent<Image>().color = color_attackAction;
      panelMove.GetComponent<Image>().color = color_moveAction;
      panelFusion.GetComponent<Image>().color = color_fusionAction;
      panelPossess.GetComponent<Image>().color = color_attackAction;
      panelExpanse.GetComponent<Image>().color = color_moveAction;
      panelGoUnestuned.GetComponent<Image>().color = color_fusionAction;

      if (selectedToken.actionPoints == selectedToken.maxActionPoints) panelSpecialAbility.GetComponent<Image>().color = color_specialAction;
      else panelSpecialAbility.GetComponent<Image>().color = color_nullAction;
    }

    TokensMenuAnimator.SetBool("Are selected token", true);
  }

  public void UnselectToken() {
    if (selectedToken) selectedToken.action = -1;
    if (PosibleTokenFusion) PosibleTokenFusion.SetActive(false);

    setAlphaTokens(1);
    HideMenuActions();
    BoardHighLight.Instance.HideHighlights();
    BoardHighLight.Instance.hideHighLightsSelectedToken();

    TokensMenuAnimator.SetBool("Are selected token", false);

    selectedToken = null;
  }

  public void SelectActionToken(int action) {
    if (!selectedToken) return;
    if (turn && selectedToken.isEnemy && !selectedToken.possessed) {
      UnselectToken();
      sound.playInvalid();
      return;
    }

    if (!turn && !selectedToken.isEnemy && !selectedToken.possessed) {
      UnselectToken();
      sound.playInvalid();
      return;
    }

    if (selectedToken.actionPoints < 1 && !selectedToken.possessed) {
      UnselectToken();
      sound.playInvalid();
      return;
    }

    BoardHighLight.Instance.HideHighlights();
    switch (action) {
    case 0:
      selectedToken.action = 0;
      allowedMoves = selectedToken.PossibleMove();
      movementPattern = selectedToken.MovementPattern();

      sound.playStartMove();

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (movementPattern[i, j] && allowedMoves[i, j]) {
            movementPattern[i, j] = false;
          }
        }
      }

      BoardHighLight.Instance.HighlightAllowedMoves(allowedMoves);
      BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);
      break;

    case 1:
      attackPattern = selectedToken.AttackPattern();

      if (selectedToken.typeToken == 11) sound.playBeastStartAttack();

      selectedToken.action = 1;

      if (selectedToken.typeToken == 15) break;

      allowedAttack = selectedToken.avalibleAttack();

      if (selectedToken.meleeAttack) {
        sound.playStartAttack();
      } else {
        sound.playStartArcherAttack();
      }

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (attackPattern[i, j] && allowedAttack[i, j]) {
            attackPattern[i, j] = false;
          }
        }
      }

      BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
      BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);
      break;

    case 2:
      sound.playStartFusion();
      ShowTokenFusionated();

      foreach (GameObject item in PosibleTokensFusion) {
        Destroy(item);
      }

      selectedToken.action = 2;
      allowedFusion = selectedToken.PossibleFusion();
      fusionPattern = selectedToken.FusionPattern();

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (fusionPattern[i, j] && allowedFusion[i, j]) {
            fusionPattern[i, j] = false;
          }
        }
      }

      BoardHighLight.Instance.HighlightAllowedFusion(allowedFusion);
      BoardHighLight.Instance.HighlightAllowedFusionPattern(fusionPattern);
      break;

    case 3:
      if (selectedToken.actionPoints < selectedToken.maxActionPoints) {
        UnselectToken();
        sound.playInvalid();
        return;
      }

      selectedToken.action = 3;

      if (selectedToken.meleeAttack) {
        sound.playStartAttack();
      } else {
        sound.playStartArcherAttack();
      }

      if (selectedToken.specialAbilityAttack) {
        attackPattern = selectedToken.PatternAttackSpecialAbility();
        allowedAttack = selectedToken.PossibleAttackSpecialAbility();

        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            if (attackPattern[i, j] && allowedAttack[i, j]) {
              attackPattern[i, j] = false;
            }
          }
        }

        BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
        BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);
      }

      if (selectedToken.specialAbilityMovement) {
        allowedMoves = selectedToken.PossibleMoveSpecialAbility();
        movementPattern = selectedToken.PatternMovementSpecialAbility();

        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            if (movementPattern[i, j] && allowedMoves[i, j]) {
              movementPattern[i, j] = false;
            }
          }
        }

        BoardHighLight.Instance.HighlightAllowedMoves(allowedMoves);
        BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);
      }

      if (selectedToken.typeToken == 20) {
        allowedMoves = selectedToken.PossibleMoveSpecialAbility();
        movementPattern = selectedToken.PatternMovementSpecialAbility();

        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            if (movementPattern[i, j] && allowedMoves[i, j]) {
              movementPattern[i, j] = false;
            }
          }
        }
        BoardHighLight.Instance.HighlightAllowedTeleport(allowedMoves);
        BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);
      }

      if (selectedToken.typeToken == 12) {
        allowedMoves = selectedToken.PossibleMoveSpecialAbility();
        movementPattern = selectedToken.PatternMovementSpecialAbility();

        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            if (movementPattern[i, j] && allowedMoves[i, j]) {
              movementPattern[i, j] = false;
            }
          }
        }
        BoardHighLight.Instance.HighlightObjectiveTeleport(allowedMoves);
        BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);
      }

      if (selectedToken.typeToken == 6) {
        movementPattern = selectedToken.GetComponent<Catapult>().PatternTeleportSpecialAbility();

        if (selectedToken.actionPoints > 0) {
          allowedMoves = selectedToken.GetComponent<Catapult>().PossibleTeleportSpecialAbility();
          for (int i = 0; i < WIDGHT; i++) {
            for (int j = 0; j < HEIGHT; j++) {
              if (movementPattern[i, j] && allowedMoves[i, j]) {
                movementPattern[i, j] = false;
              }
            }
          }
          BoardHighLight.Instance.HighlightAllowedTeleport(allowedMoves);
        }
        BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);

        movementPattern = selectedToken.PatternMovementSpecialAbility();

        if (selectedToken.actionPoints > 0) {
          allowedMoves = selectedToken.PossibleMoveSpecialAbility();
          for (int i = 0; i < WIDGHT; i++) {
            for (int j = 0; j < HEIGHT; j++) {
              if (movementPattern[i, j] && allowedMoves[i, j]) {
                movementPattern[i, j] = false;
              }
            }
          }
          BoardHighLight.Instance.HighlightObjectiveTeleport(allowedMoves);
        }
        BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);
      }
      break;

    case 4:
      if (selectedToken.possessed) {
        selectedToken.possessed = false;
        selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
        selectedToken.stunned = false;
        selectedToken.transform.GetChild(5).GetChild(1).gameObject.SetActive(false);
      } else {
        selectedToken.setActionPoints(-1);
        selectedToken.stunned = false;
        selectedToken.transform.GetChild(5).GetChild(1).gameObject.SetActive(false);
      }
      UnselectToken();
      break;
    }
  }
  public void HideMenuActions() {
    panelTokenInfo.SetActive(false);
    panelAttack.SetActive(false);
    panelMove.SetActive(false);
    panelFusion.SetActive(false);
    panelSpecialAbility.SetActive(false);
    panelGoUnestuned.SetActive(false);
    panelPossess.SetActive(false);
    panelExpanse.SetActive(false);
    panelInfoSpecialAbility.SetActive(false);
  }


  private void Attack(int x, int y) {
    selectedToken.attackOnce = true;
    if (selectedToken.typeToken == 15) {
      bool attackVulcan = true;
      for (int j = 0; j < HEIGHT; j++) {
        for (int i = 0; i < WIDGHT; i++) {
          if (allowedAttack[i, j]) {
            if (TokensInBoard[i, j] != null) {
              if (attackVulcan) {
                selectedToken.setActionPoints(-1);
                attackVulcan = false;
              }
              TokensInBoard[i, j].setLife(-1);

              if (TokensInBoard[i, j].life <= 0) TokenDies(TokensInBoard[i, j]);
            }
          }
        }
      }
    } else if (selectedToken.typeToken == 6) {
      bool attackCatapult = true;
      for (int j = 0; j < HEIGHT; j++) {
        for (int i = 0; i < WIDGHT; i++) {
          if (allowedAttack[i, j]) {
            if (TokensInBoard[i, j] != null) {
              if (attackCatapult) {
                selectedToken.setActionPoints(-1);
                attackCatapult = false;
              }
              TokensInBoard[i, j].setLife(-1);

              if (TokensInBoard[i, j].life <= 0) TokenDies(TokensInBoard[i, j]);
            }
          }
        }
      }
    } else if (selectedToken.typeToken == 10) {
      bool attackDragon = true;
      for (int j = 0; j < HEIGHT; j++) {
        for (int i = 0; i < WIDGHT; i++) {
          if (allowedAttack[i, j]) {
            if (attackDragon) {
              attackDragon = false;
              selectedToken.setActionPoints(-1);
            }
            if (TokensInBoard[i, j] != null) {
              TokensInBoard[i, j].setLife(-1);

              if (TokensInBoard[i, j].life <= 0) TokenDies(TokensInBoard[i, j]);
            }
          }
        }
      }
    } else {
      if (allowedAttack[x, y]) {
        Token victim = TokensInBoard[x, y];

        if (selectedToken.possessed) {
          selectedToken.possessed = false;
          selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
        } else selectedToken.setActionPoints(-1);

        if (selectedToken.GetComponent<Devil>()) {
          if (allowedAttack[x, y]) {
            TokensInBoard[x, y].possessed = true;
            selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(true);
            return;
          }
        } else {
          victim.setLife(-1);

          if (selectedToken.typeToken == 11) sound.playBeastAttacks();

          if (victim.life <= 0) {
            TokenDies(victim);

            if (selectedToken.typeToken == 13)
            {
              selectedToken.setActionPoints(1);
            }

            if (selectedToken.meleeAttack) {
              TokensInBoard[selectedToken.CurrentX, selectedToken.CurrentY] = null;

              selectedToken.gameObject.SetActive(false);
              selectedToken.gameObject.SetActive(true);

              selectedToken.transform.position = GetTile(x, y);

              selectedToken.setPosition(x, y);

              TokensInBoard[x, y] = selectedToken;
            }
          } else {
            if (selectedToken.meleeAttack) {
              int nX = -1, nY = -1;
              if (victim.CurrentX > selectedToken.CurrentX && victim.CurrentY > selectedToken.CurrentY) {
                nX = victim.CurrentX - 1;
                nY = victim.CurrentY - 1;
              } else if (victim.CurrentX < selectedToken.CurrentX && victim.CurrentY > selectedToken.CurrentY) {
                nX = victim.CurrentX + 1;
                nY = victim.CurrentY - 1;
              } else if (victim.CurrentX > selectedToken.CurrentX && victim.CurrentY < selectedToken.CurrentY) {
                nX = victim.CurrentX - 1;
                nY = victim.CurrentY + 1;
              } else if (victim.CurrentX < selectedToken.CurrentX && victim.CurrentY < selectedToken.CurrentY) {
                nX = victim.CurrentX + 1;
                nY = victim.CurrentY + 1;
              } else if (victim.CurrentX == selectedToken.CurrentX && victim.CurrentY > selectedToken.CurrentY) {
                nX = victim.CurrentX;
                nY = victim.CurrentY - 1;
              } else if (victim.CurrentX == selectedToken.CurrentX && victim.CurrentY < selectedToken.CurrentY) {
                nX = victim.CurrentX;
                nY = victim.CurrentY + 1;
              } else if (victim.CurrentX < selectedToken.CurrentX && victim.CurrentY == selectedToken.CurrentY) {
                nX = victim.CurrentX + 1;
                nY = victim.CurrentY;
              } else if (victim.CurrentX > selectedToken.CurrentX && victim.CurrentY == selectedToken.CurrentY) {
                nX = victim.CurrentX - 1;
                nY = victim.CurrentY;
              }

              TokensInBoard[selectedToken.CurrentX, selectedToken.CurrentY] = null;

              selectedToken.gameObject.SetActive(false);
              selectedToken.gameObject.SetActive(true);

              selectedToken.transform.position = GetTile(nX, nY);

              selectedToken.setPosition(nX, nY);

              TokensInBoard[nX, nY] = selectedToken;
            }
          }
        }
        sound.playSwordHit();
      }
    }
    UnselectToken();
  }


  private void TokenDies(Token victim) {
    if (victim.typeToken == 21) {
      RestartGame();
      return;
    }

    int situation;

    if (victim.typeToken == 12) {
      TokensInBoard[victim.relatedPortal.CurrentX, victim.relatedPortal.CurrentY] = null;

      inactiveTokens.Add(victim.relatedPortal.gameObject);
      activeTokens.Remove(victim.relatedPortal.gameObject);

      victim.relatedPortal.gameObject.SetActive(false);
    }

    TokensInBoard[victim.CurrentX, victim.CurrentY] = null;

    inactiveTokens.Add(victim.gameObject);
    activeTokens.Remove(victim.gameObject);

    victim.gameObject.SetActive(false);

    if (victim.isEnemy && turn) {
      situation = 0;
    } else if (!victim.isEnemy && !turn) {
      situation = 1;
    } else {
      situation = 2;
    }

    if (victim.typeToken < 5) SetNumberTokens(victim, situation);
    else {
      if (victim.typeToken == 5) SetNumberTokens(2, 4, situation);
      if (victim.typeToken == 6) SetNumberTokens(1, 1, situation);
      if (victim.typeToken == 7) SetNumberTokens(1, 0, situation);
      if (victim.typeToken == 8) SetNumberTokens(1, 3, situation);
      if (victim.typeToken == 9) SetNumberTokens(4, 4, situation);
      if (victim.typeToken == 10) SetNumberTokens(4, 0, situation);
      if (victim.typeToken == 11) SetNumberTokens(2, 0, situation);
      if (victim.typeToken == 12) SetNumberTokens(3, 4, situation);
      if (victim.typeToken == 13) SetNumberTokens(2, 2, situation);
      if (victim.typeToken == 14) SetNumberTokens(0, 3, situation);
      if (victim.typeToken == 15) SetNumberTokens(4, 1, situation);
      if (victim.typeToken == 16) SetNumberTokens(3, 3, situation);
      if (victim.typeToken == 17) SetNumberTokens(0, 0, situation);
      if (victim.typeToken == 18) SetNumberTokens(1, 2, situation);
    }
  }


  private void SetNumberTokens(Token victim, int situation) {
    switch (situation) {
    case 0:
    int iNumP1Pool = Int32.Parse(txtRestTokensP1[victim.typeToken].GetComponent<Text>().text);
    iNumP1Pool += 1;
    txtRestTokensP1[victim.typeToken].GetComponent<Text>().text = iNumP1Pool.ToString();
    break;

    case 1:
    int iNumP2Pool = Int32.Parse(txtRestTokensP2[victim.typeToken].GetComponent<Text>().text);
    iNumP2Pool += 1;
    txtRestTokensP2[victim.typeToken].GetComponent<Text>().text = iNumP2Pool.ToString();
    break;

    case 2:
    int iNumPPool = Int32.Parse(txtRestTokens[victim.typeToken].GetComponent<Text>().text);
    iNumPPool += 1;
    txtRestTokens[victim.typeToken].GetComponent<Text>().text = iNumPPool.ToString();
    break;
    }
  }
  private void SetNumberTokens(int typeToken1, int typeToken2, int situation) {
    switch (situation) {
    case 0:
    int iNumP1Pool1 = Int32.Parse(txtRestTokensP1[typeToken1].GetComponent<Text>().text);

    if (typeToken1 == typeToken2) {
      iNumP1Pool1 += 2;
      txtRestTokensP1[typeToken1].GetComponent<Text>().text = iNumP1Pool1.ToString();
    } else {
      int iNumP1Pool2 = Int32.Parse(txtRestTokensP1[typeToken2].GetComponent<Text>().text);
      iNumP1Pool1 += 1;
      iNumP1Pool2 += 1;
      txtRestTokensP1[typeToken1].GetComponent<Text>().text = iNumP1Pool1.ToString();
      txtRestTokensP1[typeToken2].GetComponent<Text>().text = iNumP1Pool2.ToString();
    }
    break;
    case 1:
    int iNumP2Pool1 = Int32.Parse(txtRestTokensP2[typeToken1].GetComponent<Text>().text);

    if (typeToken1 == typeToken2) {
      iNumP2Pool1 += 2;
      txtRestTokensP2[typeToken1].GetComponent<Text>().text = iNumP2Pool1.ToString();
    } else {
      int iNumP2Pool2 = Int32.Parse(txtRestTokensP2[typeToken2].GetComponent<Text>().text);
      iNumP2Pool1 += 1;
      iNumP2Pool2 += 1;
      txtRestTokensP2[typeToken1].GetComponent<Text>().text = iNumP2Pool1.ToString();
      txtRestTokensP2[typeToken2].GetComponent<Text>().text = iNumP2Pool2.ToString();
    }
    break;
    case 2:
    int iNumPPool1 = Int32.Parse(txtRestTokens[typeToken1].GetComponent<Text>().text);

    if (typeToken1 == typeToken2) {
      iNumPPool1 += 2;
      txtRestTokens[typeToken1].GetComponent<Text>().text = iNumPPool1.ToString();
    } else {
      int iNumPPool2 = Int32.Parse(txtRestTokens[typeToken2].GetComponent<Text>().text);
      iNumPPool1 += 1;
      iNumPPool2 += 1;
      txtRestTokens[typeToken1].GetComponent<Text>().text = iNumPPool1.ToString();
      txtRestTokens[typeToken2].GetComponent<Text>().text = iNumPPool2.ToString();
    }
    break;
    }
  }

  enum targetPos : int {
    right = 1,
    left = 2,
    top = 3,
    bottom = 4,
    topRight = 5,
    topLeft = 6,
    bottomRight = 7,
    bottomLeft = 8
  }

  private int kowTokenReference(int targetX, int targetY, int currentX, int currentY) {
    if (targetX > currentX && targetY == currentY) return (int)targetPos.right;
    else if (targetX < currentX && targetY == currentY) return (int)targetPos.left;
    else if (targetX == currentX && targetY > currentY) return (int)targetPos.top;
    else if (targetX == currentX && targetY < currentY) return (int)targetPos.bottom;
    else if (targetX > currentX && targetY > currentY) return (int)targetPos.topRight;
    else if (targetX < currentX && targetY > currentY) return (int)targetPos.topLeft;
    else if (targetX > currentX && targetY < currentY) return (int)targetPos.bottomRight;
    else if (targetX < currentX && targetY < currentY) return (int)targetPos.bottomLeft;
    else return 0;
  }

  private string[] knowTriggers(int targetX, int targetY) {
    int tokenReference = kowTokenReference(selectedToken.CurrentX, selectedToken.CurrentY, targetX, targetY);
    string[] triggers = new string[3];

    switch (tokenReference) {
    case 0:
    return null;

    case 1:
      triggers[0] = "left_current";
      triggers[1] = "right_target";
      triggers[2] = "Right_Target";

    case 2:
      triggers[0] = "right_current";
      triggers[1] = "left_target";
      triggers[2] = "Left_Target";

    case 3:
      triggers[0] = "bottom_current";
      triggers[1] = "top_target";
      triggers[2] = "Top_Target";

    case 4:
      triggers[0] = "top_curret";
      triggers[1] = "bottom_target";
      triggers[2] = "Bottom_Target";

    case 5:
      triggers[0] = "bottom_left_current";
      triggers[1] = "top_right_target";
      triggers[2] = "Top_Right_Target";

    case 6:
      triggers[0] = "bottom_right_current";
      triggers[1] = "top_left_target";
      triggers[2] = "Top_Left_Target";

    case 7:
      triggers[0] = "top_left_current";
      triggers[1] = "bottom_right_target";
      triggers[2] = "Bottom_Right_Target";

    case 8:
      triggers[0] = "top_right_current";
      triggers[1] = "bottom_left_target";
      triggers[2] = "Bottom_Left_Target";
    }

    return triggers;
  }

  private void FusionateToken(int x, int y) {
    Token targetToken = TokensInBoard[x, y];
    if (allowedFusion[x, y]) {
      if (!selectedToken.possessed) selectedToken.setActionPoints(-1);

      string[] triggers = knowTriggers(x, y);
      TokensInBoard[selectedToken.CurrentX, selectedToken.CurrentY].GetComponent<Animator>().SetTrigger(triggers[0]);
      TokensInBoard[targetToken.CurrentX, targetToken.CurrentY].GetComponent<Animator>().SetTrigger(triggers[1]);

      float clipLength = TokensInBoard[targetToken.CurrentX, targetToken.CurrentY].GetComponent<Animator>().runtimeAnimatorController.animationClips.
        First(clip => clip.name == triggers[2]).length;
      StartCoroutine(DelayedFusion(x, y, clipLength));
    }
  }

  IEnumerator DelayedFusion(int x, int y, float _delay = 0) {
    yield return new WaitForSeconds(_delay);
    Debug.Log(_delay);

    Token objective = TokensInBoard[x, y];

    selectedToken.gameObject.SetActive(false);
    objective.gameObject.SetActive(false);

    int ancestorMovements = selectedToken.actionPoints + objective.actionPoints;

    TokensInBoard[selectedToken.CurrentX, selectedToken.CurrentY] = null;
    TokensInBoard[objective.CurrentX, objective.CurrentY] = null;

    if (turn) TokenInstance(tokensP1, KnowFusionTypeToken(x, y, objective), x, y);
    else TokenInstance(tokensP2, KnowFusionTypeToken(x, y, objective), x, y);

    if (KnowFusionTypeToken(x, y, objective) == 12) {
      if (turn) TokenInstance(tokensP1, KnowFusionTypeToken(x, y, objective), selectedToken.CurrentX, selectedToken.CurrentY);
      else TokenInstance(tokensP2, KnowFusionTypeToken(x, y, objective), selectedToken.CurrentX, selectedToken.CurrentY);
    } else {
      if (ancestorMovements > TokensInBoard[x, y].actionPoints) {
        TokensInBoard[x, y].actionPoints = ancestorMovements;
        TokensInBoard[x, y].setActionPoints(0);
      }
    }

    sound.playFusion();

    if (selectedToken.possessed) {
      selectedToken.possessed = false;
      selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
    }
    UnselectToken();
  }

  private int KnowFusionTypeToken(int x, int y, Token objective) {
    int TypeToken = -1;

    if (objective.GetComponent<Horse>()) {
      if (selectedToken.GetComponent<Archer>()) {
        TypeToken = 7;
      } else if (selectedToken.GetComponent<Wizard>()) {
        TypeToken = 10;
      } else if (selectedToken.GetComponent<Infantry>()) {
        TypeToken = 11;
      } else if (selectedToken.GetComponent<Builder>()) {
        TypeToken = 14;
      } else if (selectedToken.GetComponent<Horse>()) {
        TypeToken = 17;
      }
    } else if (objective.GetComponent<Archer>()) {
      if (selectedToken.GetComponent<Horse>()) {
        TypeToken = 7;
      } else if (selectedToken.GetComponent<Wizard>()) {
        TypeToken = 15;
      } else if (selectedToken.GetComponent<Infantry>()) {
        TypeToken = 18;
      } else if (selectedToken.GetComponent<Builder>()) {
        TypeToken = 6;
      } else if (selectedToken.GetComponent<Archer>()) {
        TypeToken = 8;
      }
    } else if (objective.GetComponent<Wizard>()) {
      if (selectedToken.GetComponent<Archer>()) {
        TypeToken = 15;
      } else if (selectedToken.GetComponent<Horse>()) {
        TypeToken = 10;
      } else if (selectedToken.GetComponent<Infantry>()) {
        TypeToken = 5;
      } else if (selectedToken.GetComponent<Builder>()) {
        TypeToken = 12;
      } else if (selectedToken.GetComponent<Wizard>()) {
        TypeToken = 9;
      }
    } else if (objective.GetComponent<Infantry>()) {
      if (selectedToken.GetComponent<Archer>()) {
        TypeToken = 18;
      } else if (selectedToken.GetComponent<Wizard>()) {
        TypeToken = 5;
      } else if (selectedToken.GetComponent<Horse>()) {
        TypeToken = 11;
      } else if (selectedToken.GetComponent<Builder>()) {
        TypeToken = 20;
      } else if (selectedToken.GetComponent<Infantry>()) {
        TypeToken = 13;
      }
    } else if (objective.GetComponent<Builder>()) {
      if (selectedToken.GetComponent<Archer>()) {
        TypeToken = 6;
      } else if (selectedToken.GetComponent<Wizard>()) {
        TypeToken = 12;
      } else if (selectedToken.GetComponent<Infantry>()) {
        TypeToken = 20;
      } else if (selectedToken.GetComponent<Horse>()) {
        TypeToken = 14;
      } else if (selectedToken.GetComponent<Builder>()) {
        TypeToken = 16;
      }
    }

    return TypeToken;
  }
  private void ShowTokenFusionated() {
    allowedFusion = selectedToken.PossibleFusion();

    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        int iX = 1, iY = 1;
        if (selectionY == HEIGHT - 1) iY = 0;
        Vector3 pos = new Vector3(i + iX, 1, j + iY);

        if (!PosibleTokenFusion || !PosibleTokenFusion.activeSelf) {
          if (allowedFusion[i, j] && selectionX == i && selectionY == j) {
            if (turn) PosibleTokenFusion = Instantiate(tokensP1[KnowFusionTypeToken(selectionX, selectionY, TokensInBoard[selectionX, selectionY])], pos, Quaternion.identity) as GameObject;
            else PosibleTokenFusion = Instantiate(tokensP2[KnowFusionTypeToken(selectionX, selectionY, TokensInBoard[selectionX, selectionY])], pos, Quaternion.identity) as GameObject;
          }
        } else if (allowedFusion[i, j] && selectionX == i && selectionY == j) {
          PosibleTokenFusion.SetActive(false);
          if (turn) PosibleTokenFusion = Instantiate(tokensP1[KnowFusionTypeToken(selectionX, selectionY, TokensInBoard[selectionX, selectionY])], pos, Quaternion.identity) as GameObject;
          else PosibleTokenFusion = Instantiate(tokensP2[KnowFusionTypeToken(selectionX, selectionY, TokensInBoard[selectionX, selectionY])], pos, Quaternion.identity) as GameObject;
        } else if (!allowedFusion[i, j] && selectionX == i && selectionY == j) Destroy(PosibleTokenFusion);
      }
    }
  }
  public void ShowTokensFusionateds() {
    allowedFusion = selectedToken.PossibleFusion();

    for (int i = 0; i < WIDGHT; i++) {
      for (int j = 0; j < HEIGHT; j++) {
        int iX = 1, iY = 1;
        if (j == HEIGHT - 1) iY = 0;
        Vector3 pos = new Vector3(i + iX, 1, j + iY);

        if (!PosibleTokenFusion || !PosibleTokenFusion.activeSelf) {
          if (allowedFusion[i, j]) {
            if (turn) PosibleTokensFusion.Add(Instantiate(tokensP1[KnowFusionTypeToken(i, j, TokensInBoard[i, j])], pos, Quaternion.identity) as GameObject);
            else PosibleTokensFusion.Add(Instantiate(tokensP2[KnowFusionTypeToken(i, j, TokensInBoard[i, j])], pos, Quaternion.identity) as GameObject);
              FxAttractiveObject.transform.position = PosibleTokenFusion.transform.position;
          }
        }
      }
    }
  }

  private void MoveToken(int x, int y) {
    if (allowedMoves[x, y] && typeToken != 16 && typeToken != 19) {
      //selectedToken.movedOnce = true;

      selectedToken.LeaveTrace(x, y);

      if (!selectedToken.possessed) selectedToken.setActionPoints(-1);
      else {
        selectedToken.possessed = false;
        selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
      }

      TokensInBoard[selectedToken.CurrentX, selectedToken.CurrentY] = null;

      selectedToken.gameObject.SetActive(false);
      selectedToken.gameObject.SetActive(true);

      selectedToken.transform.position = GetTile(x, y);

      selectedToken.setPosition(x, y);

      TokensInBoard[x, y] = selectedToken;

      sound.playStep();
    } else if (allowedMoves[x, y]) {
      selectedToken.setActionPoints(-1);
      if (typeToken == 19) selectedToken.GetComponent<Battlement>().builder = false;
      TokenInstance(turn ? tokensP1 : tokensP2, 19, x, y);
    } else {
      sound.playInvalid();
    }
    UnselectToken();
  }


  private void SpecialAbility(int x, int y) {
    if (selectedToken.actionPoints < selectedToken.maxActionPoints && !selectedToken.zenMode) {
      sound.playInvalid();
      return;
    }

    switch (selectedToken.typeToken) {
    case 5:				
      AngelSpecialAbility(x, y);
      break;

    case 6:		
      CatapultSpecilAbility(x, y);
      break;

    case 7:
      CentaurSpecialAbility(x, y);
      break;

    case 8:
      CrossbowmanSpecialAbility(x, y);
      break;

    case 9:				
      DevilSpecialAbility(x, y);
      break;

    case 10:
      DragonSpecialAbility();
      break;

    case 11:													
      BeastSpecialAbility(x, y);
      break;

    case 12:
      PortalSpecilAbility(x, y);
      break;

    case 13:				
      NinjaSpecialAbility(x, y);
      break;

    case 14:
      RamSpecialAbility();
      break;

    case 15:
      VulcanSpecialAbility();
      break;

    case 16:
      WallSpecialAbility();
      break;

    case 17:
      UnicornSpecialAbility(x, y);
      break;

    case 18:
      LancerSpecialAbility(x, y);
      break;

    case 20:
      CannonSpecialAbility();
      break;
    }

    if (selectedToken.typeToken != 12 && !teleported) UnselectToken();
  }

  private void PortalSpecilAbility(int x, int y) {
    if (allowedMoves[x, y]) {
      teleported = TokensInBoard[x, y];
      teleported.gameObject.SetActive(false);

      Token t = selectedToken;
      selectedToken = TokensInBoard[t.relatedPortal.CurrentX, t.relatedPortal.CurrentY];

      BoardHighLight.Instance.HideHighlights();

      allowedMoves = selectedToken.GetComponent<Portal>().PossibleTeleportSpecialAbility();
      movementPattern = selectedToken.GetComponent<Portal>().PatternTeleportSpecialAbility();

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (movementPattern[i, j] && allowedMoves[i, j]) {
            movementPattern[i, j] = false;
          }
        }
      }

      BoardHighLight.Instance.HighlightObjectiveTeleport(allowedMoves);
      BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);
    }
  }
  private void Teleport(int x, int y) {
    if (allowedMoves[x, y]) {
      TokensInBoard[teleported.CurrentX, teleported.CurrentY] = null;

      teleported.gameObject.SetActive(true);

      teleported.transform.position = GetTile(x, y);

      teleported.setPosition(x, y);

      TokensInBoard[x, y] = teleported;

      teleported = null;

      sound.playStep();

      selectedToken.relatedPortal.setActionPoints(-selectedToken.maxActionPoints);
      if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
      if (selectedToken.possessed) selectedToken.possessed = false;

      UnselectToken();
    } else {
      teleported.gameObject.SetActive(true);
      teleported = null;
      UnselectToken();
    }
  }
  private void CannonSpecialAbility() {
    bool attacks = false;
    for (int j = 0; j < HEIGHT; j++) {
      for (int i = 0; i < WIDGHT; i++) {
        if (allowedAttack[i, j]) {
          if (TokensInBoard[i, j] != null) {
            if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
            else {
              selectedToken.possessed = false;
              selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
            }

            attacks = true;
            TokensInBoard[i, j].setLife(-1);

            if (TokensInBoard[i, j].life <= 0) TokenDies(TokensInBoard[i, j]);
          }
        }
      }
    }

    if (attacks) {
      int tX = selectedToken.CurrentX, tY = selectedToken.CurrentY;
      if (!selectedToken.isEnemy && tY == 0) return;
      else if (!selectedToken.isEnemy) {
        if (allowedMoves[tX, tY - 1]) {
          TokensInBoard[tX, tY] = null;

          selectedToken.gameObject.SetActive(false);
          selectedToken.gameObject.SetActive(true);

          selectedToken.transform.position = GetTile(tX, tY - 1);

          selectedToken.setPosition(tX, tY - 1);

          TokensInBoard[tX, tY - 1] = selectedToken;

          sound.playStep();
        }
      }

      if (selectedToken.isEnemy && tY == HEIGHT - 1) return;
      else if (selectedToken.isEnemy) {
        if (allowedMoves[tX, tY + 1]) {
          TokensInBoard[tX, tY] = null;

          selectedToken.gameObject.SetActive(false);
          selectedToken.gameObject.SetActive(true);

          selectedToken.transform.position = GetTile(tX, tY + 1);

          selectedToken.setPosition(tX, tY + 1);

          TokensInBoard[tX, tY + 1] = selectedToken;

          sound.playStep();
        }
      }
    }
  }

  private void RamSpecialAbility() { 
    for (int j = 0; j < HEIGHT; j++) {
      for (int i = 0; i < WIDGHT; i++) {
        if (allowedAttack[i, j]) {
          if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
          else {
            selectedToken.possessed = false;
            selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
          }

          if (TokensInBoard[i, j] != null) {
            if (TokensInBoard[i, j].typeToken == 21) {
              RestartGame();
              return;
            }

            TokensInBoard[i, j].setLife(-1);

            if (TokensInBoard[i, j].life <= 0) {
              TokenDies(TokensInBoard[i, j]);

              if (turn) {
                TokenInstance(tokensP1, 2, i, j);
                TokenInstance(tokensP1, 0, i, j - 1);
              } else {
                TokenInstance(tokensP2, 2, i, j);
                TokenInstance(tokensP2, 0, i, j + 1);
              }
            } else {
              if (turn) {
                TokenInstance(tokensP1, 2, i, j - 1);
                TokenInstance(tokensP1, 0, i, j - 2);
              } else {
                TokenInstance(tokensP2, 2, i, j + 1);
                TokenInstance(tokensP2, 0, i, j + 2);
              }
            }
          } else {
            if (turn) {
              TokenInstance(tokensP1, 2, i, j);
              TokenInstance(tokensP1, 0, i, j - 1);
            } else {
              TokenInstance(tokensP2, 2, i, j);
              TokenInstance(tokensP2, 0, i, j + 1);
            }
          }
          TokensInBoard[selectedToken.CurrentX, selectedToken.CurrentY] = null;

          inactiveTokens.Add(selectedToken.gameObject);
          activeTokens.Remove(selectedToken.gameObject);

          selectedToken.gameObject.SetActive(false);
        }
      }
    }
  }

  private void BeastSpecialAbility(int x, int y) {
    if (allowedMoves[x, y]) {
      if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
      else {
        selectedToken.possessed = false;
        selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
      }

      TokensInBoard[selectedToken.CurrentX, selectedToken.CurrentY] = null;

      selectedToken.gameObject.SetActive(false);
      selectedToken.gameObject.SetActive(true);

      selectedToken.transform.position = GetTile(x, y);

      selectedToken.setPosition(x, y);

      TokensInBoard[x, y] = selectedToken;

      sound.playBeastSpecialAbility();

      for (int j = 0; j < HEIGHT; j++) {
        for (int i = 0; i < WIDGHT; i++) {
          if (allowedAttack[i, j]) {
            if (y >= j && turn) {
              TokensInBoard[i, j].setLife(-1);

              if (TokensInBoard[i, j].life <= 0) TokenDies(TokensInBoard[i, j]);
            }

            if (y <= j && !turn) {
              TokensInBoard[i, j].setLife(-1);

              if (TokensInBoard[i, j].life <= 0) TokenDies(TokensInBoard[i, j]);
            }
          }
        }
      }
    }
  }

  private void DragonSpecialAbility() {
    for (int j = 0; j < HEIGHT; j++) {
      for (int i = 0; i < WIDGHT; i++) {
        if (allowedAttack[i, j]) {
          if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
          else {
            selectedToken.possessed = false;
            selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
          }

          if (TokensInBoard[i, j] != null) {
            TokensInBoard[i, j].setLife(-1);

            if (TokensInBoard[i, j].life <= 0) TokenDies(TokensInBoard[i, j]);
          }
        }
      }
    }
  }

  private void CrossbowmanSpecialAbility(int x, int y) {
    if (allowedAttack[x, y]) {
      if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
      else {
        selectedToken.possessed = false;
        selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
      }

      int damage = 3, fillDamage = 0;
      if (!selectedToken.isEnemy) {
        for (int h = 0; h < HEIGHT; h++) {
          for (int w = 0; w < WIDGHT; w++) {
            if (allowedAttack[w, h]) {
              damage -= fillDamage;
              if (damage <= 0) damage = 1;

              fillDamage = TokensInBoard[w, h].life;
              TokensInBoard[w, h].setLife(-damage);

              if (TokensInBoard[w, h].life <= 0) TokenDies(TokensInBoard[w, h]);
            }
          }
        }
      } else {
        for (int h = HEIGHT - 1; h >= 0; h--) {
          for (int w = 0; w < WIDGHT; w++) {
            if (allowedAttack[w, h]) {
              damage -= fillDamage;
              if (damage <= 0) damage = 1;

              fillDamage = TokensInBoard[w, h].life;
              TokensInBoard[w, h].setLife(-damage);

              if (TokensInBoard[w, h].life <= 0) TokenDies(TokensInBoard[w, h]);
            }
          }
        }
      }
    }
  }

  private void AngelSpecialAbility(int x, int y) {
    if (allowedAttack[x, y]) {
      if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
      else {
        selectedToken.possessed = false;
        selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
      }

      TokensInBoard[x, y].setLife(-1);

      if (TokensInBoard[x, y].life <= 0) TokenDies(TokensInBoard[x, y]);

      Cure_Stun = selectedToken.Cure_Stun(x, y);

      for (int h = 0; h < HEIGHT; h++) {
        for (int w = 0; w < WIDGHT; w++) {
          if (Cure_Stun[w, h]) {
            if (TokensInBoard[w, h] != null) {
              TokensInBoard[w, h].stunned = true;
              TokensInBoard[w, h].transform.GetChild(5).GetChild(1).gameObject.SetActive(true);

              TokensInBoard[w, h].setLife(1);
            }
          }
        }
      }
    }
  }
  private void DevilSpecialAbility(int x, int y) {
    if (x > -1 && x < WIDGHT && y > -1 && y < HEIGHT) {
      if (TokensInBoard[x, y] != null) {
        if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
        else {
          selectedToken.possessed = false;
          selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
        }

        TokensInBoard[x, y].actionPoints = 0;
        TokensInBoard[x, y].setActionPoints(0);
      }
    }
  }

  private void NinjaSpecialAbility(int x, int y) {
    if (allowedAttack[x, y]) {
      if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
      else {
        selectedToken.possessed = false;
        selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
      }

      TokensInBoard[x, y].setLife(-1);

      if (TokensInBoard[x, y].life <= 0) {
        TokenDies(TokensInBoard[x, y]);

        selectedToken.setActionPoints(1);
      }
    }
  }

  private void CatapultSpecilAbility(int x, int y) {
    allowedMoves = selectedToken.GetComponent<Catapult>().PossibleTeleportSpecialAbility();

    if (allowedMoves[x, y]) {
      Token t = TokensInBoard[selectedToken.CurrentX, selectedToken.CurrentY - 1];

      if (t) {
        t.gameObject.SetActive(false);

        TokensInBoard[t.CurrentX, t.CurrentY] = null;

        t.gameObject.SetActive(true);

        t.transform.position = GetTile(x, y);

        t.setPosition(x, y);

        TokensInBoard[x, y] = t;

        sound.playStep();

        if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
        if (selectedToken.possessed) selectedToken.possessed = false;
      }
    }
  }

  private void VulcanSpecialAbility() {
    bool attackVulcan = true;
    for (int j = 0; j < HEIGHT; j++) {
      for (int i = 0; i < WIDGHT; i++) {
        if (allowedAttack[i, j]) {
          if (TokensInBoard[i, j] != null) {
            attackVulcan = false;

            TokensInBoard[i, j].setLife(-1);
            TokensInBoard[i, j].burned = true;
            TokensInBoard[i, j].burnedCountdown = 1;
            TokensInBoard[i, j].transform.GetChild(5).GetChild(2).gameObject.SetActive(true);

            if (TokensInBoard[i, j].life <= 0) TokenDies(TokensInBoard[i, j]);
          }
        }
      }
    }

    if (!attackVulcan) {
      if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
      else selectedToken.possessed = false;
    }
  }

  private void WallSpecialAbility() {
    for (int j = 0; j < HEIGHT; j++) {
      for (int i = 0; i < WIDGHT; i++) {
        if (allowedMoves[i, j]) {
          if (turn) TokenInstance(tokensP1, 19, i, j);
          else TokenInstance(tokensP2, 19, i, j);

          if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
          else selectedToken.possessed = false;
        }
      }
    }
  }

  private void UnicornSpecialAbility(int x, int y) {
    if (!allowedAttack[x, y]) return;

    for (int j = 0; j < HEIGHT; j++) {
      for (int i = 0; i < WIDGHT; i++) {
        if (allowedAttack[i, j]) {
          if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
          else {
            selectedToken.possessed = false;
            selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
          }

          if (TokensInBoard[i, j] != null) {
            if (TokensInBoard[i, j].typeToken == 21) {
              RestartGame();
              return;
            }

            TokensInBoard[i, j].setLife(-1);

            if (TokensInBoard[i, j].life <= 0) {
              TokenDies(TokensInBoard[i, j]);

              if (turn) {
                TokenInstance(tokensP1, 2, i, j);
              } else {
                TokenInstance(tokensP2, 2, i, j);
              }
            } else {
              if (turn) {
                if (i > selectedToken.CurrentX) {
                  TokenInstance(tokensP1, 2, i - 1, j - 1);
                } else {
                  TokenInstance(tokensP1, 2, i + 1, j - 1);
                }
              } else {
                if (i > selectedToken.CurrentX) {
                  TokenInstance(tokensP2, 2, i - 1, j + 1);
                } else {
                  TokenInstance(tokensP2, 2, i + 1, j + 1);
                }
              }
            }
          } else {
            if (turn) {
              TokenInstance(tokensP1, 2, i, j);
            } else {
              TokenInstance(tokensP2, 2, i, j);
            }
          }
          TokensInBoard[selectedToken.CurrentX, selectedToken.CurrentY] = null;

          inactiveTokens.Add(selectedToken.gameObject);
          activeTokens.Remove(selectedToken.gameObject);

          selectedToken.gameObject.SetActive(false);
        }
      }
    }
  }

  private void CentaurSpecialAbility(int x, int y) {
    if (allowedMoves[x, y]) {
      if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
      else {
        selectedToken.possessed = false;
        selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
      }

      MoveToken(x, y);
    }
  }

  private void LancerSpecialAbility(int x, int y) {
    if (allowedAttack[x, y]) {
      TokenDies(TokensInBoard[x, y]);
      selectedToken.withoutWeapon = true;
      if (!selectedToken.possessed) selectedToken.setActionPoints(-selectedToken.maxActionPoints);
      else {
        selectedToken.possessed = false;
        selectedToken.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
      }
    }
  }

  private void UpdateSelection(Camera c) {
    if (!c) {
      return;
    }
    RaycastHit hit;

    if (Physics.Raycast(c.ScreenPointToRay(Input.mousePosition),
            out hit, 25f, LayerMask.GetMask("BoardFisic"))) {
      selectionX = (int)hit.point.x;
      selectionY = (int)hit.point.z;
    } else {
      selectionX = -1;
      selectionY = -1;
    }
  }

  public void TokenInstance(List<GameObject> tokens, int index, int x, int y) {
    GameObject instantiate = Instantiate(tokens[index], GetTile(x, y), Quaternion.AngleAxis(90, new Vector3(0, 1, 0))) as GameObject;
    instantiate.transform.SetParent(transform);
    TokensInBoard[x, y] = instantiate.GetComponent<Token>();
    TokensInBoard[x, y].setPosition(x, y);
    TokensInBoard[x, y].setActionPoints(0);
    TokensInBoard[x, y].LeaveTrace(x, y);
    activeTokens.Add(instantiate);

    if (TokensInBoard[x, y].typeToken == 12 && !portal_base) portal_base = TokensInBoard[x, y];
    else if (TokensInBoard[x, y].typeToken == 12) {
      TokensInBoard[x, y].relatedPortal = portal_base;
      portal_base.relatedPortal = TokensInBoard[x, y];

      portal_base = null;
    }

  }

  public void SpawnToken(int _typeToken, Text _num) {
    if (selectionX < 0 || selectionY < 0) return;

    if (TokensInBoard[selectionX, selectionY] != null) return;

    if (spawnPosLock) {

      if (turn) {
        if (selectionY != 0 && selectionY >= (numTurn / selectionY)) return;
      } else {
        if (selectionY != HEIGHT && selectionY < HEIGHT - numTurn / (HEIGHT - selectionY) - 1) return;
      }
    }

    if (spawnPoolLock) {
      if (turn && spawnsP1 <= 0) return;
      else if (!turn && spawnsP2 <= 0) return;
    }

    int iNum = Int32.Parse(_num.text);

    iNum -= 1;

    if (spawnLimitLock && iNum < 0) return;

    _num.text = iNum.ToString();
    sound.playStep();

    if (turn) {
      spawnsP1--;

      switch (_typeToken) {
      case 0:
        TokenInstance(tokensP1, 0, selectionX, selectionY);
        break;

      case 1:
        TokenInstance(tokensP1, 1, selectionX, selectionY);
        break;

      case 2:
      TokenInstance(tokensP1, 2, selectionX, selectionY);
        break;

        case 3:
        TokenInstance(tokensP1, 3, selectionX, selectionY);
        break;

      case 4:
        TokenInstance(tokensP1, 4, selectionX, selectionY);
        break;
      }
    } else {
      spawnsP2--;

      switch (_typeToken) {
      case 0:
        TokenInstance(tokensP2, 0, selectionX, selectionY);
        break;

      case 1:
        TokenInstance(tokensP2, 1, selectionX, selectionY);
        break;

      case 2:
        TokenInstance(tokensP2, 2, selectionX, selectionY);
        break;

      case 3:
        TokenInstance(tokensP2, 3, selectionX, selectionY);
        break;

      case 4:
        TokenInstance(tokensP2, 4, selectionX, selectionY);
        break;
      }
    }
  }

  public void SpawnToken() {
    if (turn) {
      if (spawnLimitLock) if (spawnsP1 <= 0) return;
      spawnsP1--;
      TokenInstance(tokensP1, 3, selectionX, selectionY);
    } else {
      spawnsP2--;
      TokenInstance(tokensP2, 3, selectionX, selectionY);
    }
  }

  private Vector3 GetTileCenter(float x, float y, float z) {
    Vector3 origin = Vector3.zero;
    origin.x += (TILE_SIZE * x) + TILE_OFFSET;
    origin.y += (TILE_SIZE * y) + TILE_OFFSET;
    origin.z += (TILE_SIZE * z) + TILE_OFFSET;

    return origin;
  }

  private Vector3 GetTile(int x, int y) {
    Vector3 origin = Vector3.zero;
    origin.x += (TILE_SIZE * x) + TILE_OFFSET;
    origin.z += (TILE_SIZE * y) + TILE_OFFSET;

    return origin;
  }

  private void DrawBoard() {
    Vector3 widhtLine = Vector3.right * WIDGHT;
    Vector3 heightLine = Vector3.forward * HEIGHT;

    for (int y = 0; y <= HEIGHT; y++) {
      Vector3 start = Vector3.forward * y;
      //Debug.DrawLine(start, start + widhtLine);

      for (int x = 0; x <= WIDGHT; x++) {
        start = Vector3.right * x;
        //Debug.DrawLine(start, start + heightLine);
      }
    }
  }

  public void DrawAttackPattern() {
    BoardHighLight.Instance.HideHighlights();

    attackPattern = selectedToken.AttackPattern();

    if (selectedToken.actionPoints > 0) {
      allowedAttack = selectedToken.avalibleAttack();

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (attackPattern[i, j] && allowedAttack[i, j]) {
            attackPattern[i, j] = false;
          }
        }
      }
      BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
    }
    BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);
  }

  public void DrawMovementPattern() {
    BoardHighLight.Instance.HideHighlights();

    movementPattern = selectedToken.MovementPattern();

    if (selectedToken.actionPoints > 0) {
      allowedMoves = selectedToken.PossibleMove();
      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (movementPattern[i, j] && allowedMoves[i, j]) {
            movementPattern[i, j] = false;
          }
        }
      }
      BoardHighLight.Instance.HighlightAllowedMoves(allowedMoves);
    }
    BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);
  }

  public void DrawFusionPattern() {
    BoardHighLight.Instance.HideHighlights();

    fusionPattern = selectedToken.FusionPattern();

    if (selectedToken.actionPoints > 0) {
      allowedFusion = selectedToken.PossibleFusion();

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (fusionPattern[i, j] && allowedFusion[i, j]) {
            fusionPattern[i, j] = false;
          }
        }
      }
      BoardHighLight.Instance.HighlightAllowedFusion(allowedFusion);
    }
    BoardHighLight.Instance.HighlightAllowedFusionPattern(fusionPattern);
  }

  public void DrawSpecialAbilityPattern() {
    BoardHighLight.Instance.HideHighlights();

    if (selectedToken.specialAbilityMovement) {
      movementPattern = selectedToken.PatternMovementSpecialAbility();

      if (selectedToken.actionPoints > 0) {
        allowedMoves = selectedToken.PossibleMoveSpecialAbility();

        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            if (movementPattern[i, j] && allowedMoves[i, j]) {
              movementPattern[i, j] = false;
            }
          }
        }
        BoardHighLight.Instance.HighlightAllowedMoves(allowedMoves);
      }
      BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);
    }

    if (selectedToken.specialAbilityAttack) {
      attackPattern = selectedToken.PatternAttackSpecialAbility();

      if (selectedToken.actionPoints > 0) {
        allowedAttack = selectedToken.PossibleAttackSpecialAbility();

        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            if (attackPattern[i, j] && allowedAttack[i, j]) {
              attackPattern[i, j] = false;
            }
          }
        }
        BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
      }
      BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);
    }

    if (selectedToken.typeToken == 20) {
      movementPattern = selectedToken.PatternMovementSpecialAbility();

      if (selectedToken.actionPoints > 0) {
        allowedMoves = selectedToken.PossibleMoveSpecialAbility();

        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            if (movementPattern[i, j] && allowedMoves[i, j]) {
              movementPattern[i, j] = false;
            }
          }
        }
        BoardHighLight.Instance.HighlightAllowedTeleport(allowedMoves);
      }
      BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);
    }

    if (selectedToken.typeToken == 12) {
      movementPattern = selectedToken.PatternMovementSpecialAbility();

      if (selectedToken.actionPoints > 0) {
        allowedMoves = selectedToken.PossibleMoveSpecialAbility();
        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            if (movementPattern[i, j] && allowedMoves[i, j]) {
              movementPattern[i, j] = false;
            }
          }
        }
        BoardHighLight.Instance.HighlightObjectiveTeleport(allowedMoves);
      }
      BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);
    }

    if (selectedToken.typeToken == 6) {
      movementPattern = selectedToken.GetComponent<Catapult>().PatternTeleportSpecialAbility();

      if (selectedToken.actionPoints > 0) {
        allowedMoves = selectedToken.GetComponent<Catapult>().PossibleTeleportSpecialAbility();
        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            if (movementPattern[i, j] && allowedMoves[i, j]) {
              movementPattern[i, j] = false;
            }
          }
        }
        BoardHighLight.Instance.HighlightAllowedTeleport(allowedMoves);
      }
      BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);

      movementPattern = selectedToken.PatternMovementSpecialAbility();

      if (selectedToken.actionPoints > 0) {
        allowedMoves = selectedToken.PossibleMoveSpecialAbility();
        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            if (movementPattern[i, j] && allowedMoves[i, j]) {
              movementPattern[i, j] = false;
            }
          }
        }
        BoardHighLight.Instance.HighlightObjectiveTeleport(allowedMoves);
      }
      BoardHighLight.Instance.HighlightAllowedMovesPattern(movementPattern);
    }


    if (selectedToken.typeToken == 7) {
      if (selectedToken.actionPoints > 0) {
        DrawFootprints();
        allowedMoves = selectedToken.GetComponent<Centaur>().PossibleMoveSpecialAbility();
        BoardHighLight.Instance.HighlightObjectiveTeleport(allowedMoves);
      }

      return;
    }

  }
  private void DrawAttackCatapult() {
    attackPattern = selectedToken.AttackPattern();

    if (selectionX == selectedToken.CurrentX + 4 && selectionY == selectedToken.CurrentY ||
      selectionX == selectedToken.CurrentX + 5 && selectionY == selectedToken.CurrentY ||
      selectionX == selectedToken.CurrentX + 6 && selectionY == selectedToken.CurrentY ||
      selectionX == selectedToken.CurrentX + 5 && selectionY == selectedToken.CurrentY - 1 ||
      selectionX == selectedToken.CurrentX + 5 && selectionY == selectedToken.CurrentY + 1) {
      BoardHighLight.Instance.HideHighlights();
      allowedAttack = selectedToken.GetComponent<Catapult>().PossibleAttackRange3();

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (attackPattern[i, j] && allowedAttack[i, j]) {
            attackPattern[i, j] = false;
          }
        }
      }
      BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
      BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);

    } else if (selectionX == selectedToken.CurrentX - 4 && selectionY == selectedToken.CurrentY ||
        selectionX == selectedToken.CurrentX - 5 && selectionY == selectedToken.CurrentY ||
        selectionX == selectedToken.CurrentX - 6 && selectionY == selectedToken.CurrentY ||
        selectionX == selectedToken.CurrentX - 5 && selectionY == selectedToken.CurrentY - 1 ||
        selectionX == selectedToken.CurrentX - 5 && selectionY == selectedToken.CurrentY + 1) {
      BoardHighLight.Instance.HideHighlights();
      allowedAttack = selectedToken.GetComponent<Catapult>().PossibleAttackRange4();

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (attackPattern[i, j] && allowedAttack[i, j]) {
            attackPattern[i, j] = false;
          }
        }
      }

      BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
      BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);

    } else if (selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY - 4 ||
        selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY - 5 ||
        selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY - 6 ||
        selectionX == selectedToken.CurrentX + 1 && selectionY == selectedToken.CurrentY - 5 ||
        selectionX == selectedToken.CurrentX - 1 && selectionY == selectedToken.CurrentY - 5) {
      BoardHighLight.Instance.HideHighlights();
      allowedAttack = selectedToken.GetComponent<Catapult>().PossibleAttackRange2();

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (attackPattern[i, j] && allowedAttack[i, j]) {
            attackPattern[i, j] = false;
          }
        }
      }

      BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
      BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);

    } else if (selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY + 4 ||
        selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY + 5 ||
        selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY + 6 ||
        selectionX == selectedToken.CurrentX + 1 && selectionY == selectedToken.CurrentY + 5 ||
        selectionX == selectedToken.CurrentX - 1 && selectionY == selectedToken.CurrentY + 5) {
      Debug.Log("Dispuesto a pintar R4");
      BoardHighLight.Instance.HideHighlights();
      allowedAttack = selectedToken.GetComponent<Catapult>().PossibleAttackRange1();

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (attackPattern[i, j] && allowedAttack[i, j]) {
            attackPattern[i, j] = false;
          }
        }
      }

      BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
      BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);
    } else {
      BoardHighLight.Instance.HideHighlights();
      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          allowedAttack[i, j] = false;
        }
      }
      BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);
    }
  }
  private void DrawAttackVulcan() {
    attackPattern = selectedToken.AttackPattern();

    if (selectionX == selectedToken.CurrentX + 1 && selectionY == selectedToken.CurrentY ||
      selectionX == selectedToken.CurrentX - 1 && selectionY == selectedToken.CurrentY ||
      selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY + 1 ||
      selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY - 1 ||
      selectionX == selectedToken.CurrentX + 1 && selectionY == selectedToken.CurrentY + 1 ||
      selectionX == selectedToken.CurrentX + 1 && selectionY == selectedToken.CurrentY - 1 ||
      selectionX == selectedToken.CurrentX - 1 && selectionY == selectedToken.CurrentY + 1 ||
      selectionX == selectedToken.CurrentX - 1 && selectionY == selectedToken.CurrentY - 1) {
      BoardHighLight.Instance.HideHighlights();
      allowedAttack = selectedToken.GetComponent<Vulcan>().PossibleAttackRange1();

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (attackPattern[i, j] && allowedAttack[i, j]) {
            attackPattern[i, j] = false;
          }
        }
      }

      BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
      BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);

    } else if (selectionX == selectedToken.CurrentX + 2 && selectionY == selectedToken.CurrentY ||
          selectionX == selectedToken.CurrentX - 2 && selectionY == selectedToken.CurrentY ||
          selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY + 2 ||
          selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY - 2 ||
          selectionX == selectedToken.CurrentX + 2 && selectionY == selectedToken.CurrentY + 1 ||
          selectionX == selectedToken.CurrentX + 2 && selectionY == selectedToken.CurrentY - 1 ||
          selectionX == selectedToken.CurrentX - 2 && selectionY == selectedToken.CurrentY + 1 ||
          selectionX == selectedToken.CurrentX - 2 && selectionY == selectedToken.CurrentY - 1 ||
          selectionX == selectedToken.CurrentX - 1 && selectionY == selectedToken.CurrentY + 2 ||
          selectionX == selectedToken.CurrentX - 1 && selectionY == selectedToken.CurrentY - 2 ||
          selectionX == selectedToken.CurrentX + 1 && selectionY == selectedToken.CurrentY + 2 ||
          selectionX == selectedToken.CurrentX + 1 && selectionY == selectedToken.CurrentY - 2) {
      BoardHighLight.Instance.HideHighlights();
      allowedAttack = selectedToken.GetComponent<Vulcan>().PossibleAttackRange2();

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (attackPattern[i, j] && allowedAttack[i, j]) {
            attackPattern[i, j] = false;
          }
        }
      }

      BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
      BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);

    } else if (selectionX == selectedToken.CurrentX - 3 && selectionY == selectedToken.CurrentY - 3 ||
          selectionX == selectedToken.CurrentX + 3 && selectionY == selectedToken.CurrentY + 3 ||
          selectionX == selectedToken.CurrentX - 3 && selectionY == selectedToken.CurrentY + 3 ||
          selectionX == selectedToken.CurrentX + 3 && selectionY == selectedToken.CurrentY - 3 ||
          selectionX == selectedToken.CurrentX - 4 && selectionY == selectedToken.CurrentY ||
          selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY - 4 ||
          selectionX == selectedToken.CurrentX + 4 && selectionY == selectedToken.CurrentY ||
          selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY + 4) {
      BoardHighLight.Instance.HideHighlights();
      allowedAttack = selectedToken.GetComponent<Vulcan>().PossibleAttackRange3();

      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (attackPattern[i, j] && allowedAttack[i, j]) {
            attackPattern[i, j] = false;
          }
        }
      }

      BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
      BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);

    } else {
      BoardHighLight.Instance.HideHighlights();
      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          allowedAttack[i, j] = false;
        }
      }
      BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);
    }
  }

  private void DrawAttackDragon() {
    attackPattern = selectedToken.AttackPattern();
    if (!selectedToken.isEnemy) {
      if (selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY + 1 ||
      selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY + 2 ||
      selectionX == selectedToken.CurrentX - 1 && selectionY == selectedToken.CurrentY + 2 ||
      selectionX == selectedToken.CurrentX + 1 && selectionY == selectedToken.CurrentY + 2 ||
      selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY + 3 ||
      selectionX == selectedToken.CurrentX - 1 && selectionY == selectedToken.CurrentY + 3 ||
      selectionX == selectedToken.CurrentX - 2 && selectionY == selectedToken.CurrentY + 3 ||
      selectionX == selectedToken.CurrentX + 1 && selectionY == selectedToken.CurrentY + 3 ||
      selectionX == selectedToken.CurrentX + 2 && selectionY == selectedToken.CurrentY + 3) {
        BoardHighLight.Instance.HideHighlights();
        allowedAttack = selectedToken.GetComponent<Dragon>().PossibleAttackRange1();

        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            if (attackPattern[i, j] && allowedAttack[i, j]) {
              attackPattern[i, j] = false;
            }
          }
        }
        BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
        BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);

      } else {
        BoardHighLight.Instance.HideHighlights();
        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            allowedAttack[i, j] = false;
          }
        }
        BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);
      }
    } else {
      if (selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY - 1 ||
        selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY - 2 ||
        selectionX == selectedToken.CurrentX - 1 && selectionY == selectedToken.CurrentY - 2 ||
        selectionX == selectedToken.CurrentX + 1 && selectionY == selectedToken.CurrentY - 2 ||
        selectionX == selectedToken.CurrentX && selectionY == selectedToken.CurrentY - 3 ||
        selectionX == selectedToken.CurrentX - 1 && selectionY == selectedToken.CurrentY - 3 ||
        selectionX == selectedToken.CurrentX - 2 && selectionY == selectedToken.CurrentY - 3 ||
        selectionX == selectedToken.CurrentX + 1 && selectionY == selectedToken.CurrentY - 3 ||
        selectionX == selectedToken.CurrentX + 2 && selectionY == selectedToken.CurrentY - 3) {
        BoardHighLight.Instance.HideHighlights();
        allowedAttack = selectedToken.GetComponent<Dragon>().PossibleAttackRange1();

        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            if (attackPattern[i, j] && allowedAttack[i, j]) {
              attackPattern[i, j] = false;
            }
          }
        }
        BoardHighLight.Instance.HighlightAllowedAttack(allowedAttack);
        BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);

      } else {
        BoardHighLight.Instance.HideHighlights();
        for (int i = 0; i < WIDGHT; i++) {
          for (int j = 0; j < HEIGHT; j++) {
            allowedAttack[i, j] = false;
          }
        }
        BoardHighLight.Instance.HighlightAllowedAttackPattern(attackPattern);
      }
    }
  }

  public void DrawFootprints() {
    BoardHighLight.Instance.HideHighlights();
    footprints = new bool[WIDGHT, HEIGHT];

    for (int t = 0; t < activeTokens.Count; t++) {
      for (int i = 0; i < WIDGHT; i++) {
        for (int j = 0; j < HEIGHT; j++) {
          if (activeTokens[t].GetComponent<Token>().boardFootprints[i, j]) {
            footprints[i, j] = true;
          }
        }
      }
    }

    BoardHighLight.Instance.HighlightFootpreints(footprints);
  }

  public Token SetSelectedToken() {
    return selectedToken;
  }
}