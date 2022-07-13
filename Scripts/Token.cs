using UnityEngine;
using System.Collections.Generic;

public class Token : MonoBehaviour {
  public int CurrentX { set; get; }
  public int CurrentY { set; get; }
  public int action { set; get; }
  public int TypeSkin;
  public bool movedOnce { set; get; }
  public bool attackOnce { set; get; }
  public int typeToken;
  public int life, maxLife, actionPoints, maxActionPoints, trail;
  public bool isEnemy, meleeAttack, doubleToken, specialAbilityAttack, specialAbilityMovement;
  public bool possessed { set; get; }
  public bool stunned { set; get; }
  public bool burned { set; get; }
  public int burnedCountdown { set; get; }
  public bool zenMode { set; get; }
  public bool withoutWeapon { set; get; }
  public static int WIDGHT = BoarManager.WIDGHT, HEIGHT = BoarManager.HEIGHT;
  public Token relatedPortal { set; get; }
  public string name { set; get; }
  public string infoSpecialAbility { set; get; }
  private List<int> footprints = new List<int>();
  public bool[,] boardFootprints = new bool[WIDGHT, HEIGHT];

  private void Start() {
    possessed = false;
    stunned = false;
    burned = false;
    withoutWeapon = false;
    zenMode = false;
    action = -1;
  }

  public void setPosition(int x, int y) {
    CurrentX = x;
    CurrentY = y;
  }

  public void setActionPoints(int _variationPoints) {
    actionPoints += _variationPoints;

    if (actionPoints > maxActionPoints) actionPoints = maxActionPoints;
    else if (actionPoints < 0) actionPoints = 0;

    int aux = actionPoints;

    for (int i = 0; i < maxActionPoints; i++) {
      aux--;
      if (aux >= 0) {
        gameObject.transform.GetChild(3).GetChild(i).GetChild(0).gameObject.SetActive(true);
      } else {
        gameObject.transform.GetChild(3).GetChild(i).GetChild(0).gameObject.SetActive(false);
      }
    }
  }

  public void setLife(int _variationDamage) {
    life += _variationDamage;

    if (life > maxLife) life = maxLife;
    else if (life < 0) life = 0;

    int aux = life;

    for (int i = 0; i < maxLife; i++) {
      aux--;
      if (aux >= 0) {
        gameObject.transform.GetChild(2).GetChild(0).GetChild(i).gameObject.SetActive(true);
      } else {
        gameObject.transform.GetChild(2).GetChild(0).GetChild(i).gameObject.SetActive(false);
      }
    }
  }

  public virtual void Moves(int x, int y, ref bool[,] r) {
    Token t;
    if (x >= 0 && x < WIDGHT && y >= 0 && y < HEIGHT) {
      t = BoarManager.Instance.TokensInBoard[x, y];
      if (t == null) {
        r[x, y] = true;
      }
    }
  }

  public virtual void MovementPattern(int x, int y, ref bool[,] r) {
    if (x >= 0 && x < WIDGHT && y >= 0 && y < HEIGHT) {
      r[x, y] = true;
    }
  }

  public virtual void Attacks(int x, int y, ref bool[,] r) {
    Token t;

    if (x >= 0 && x < WIDGHT && y >= 0 && y < HEIGHT) {
      t = BoarManager.Instance.TokensInBoard[x, y];

      if (t != null) {
        r[x, y] = true;
      }
    }
  }

  public virtual void AttackPattern(int x, int y, ref bool[,] r) {
    if (x >= 0 && x < WIDGHT && y >= 0 && y < HEIGHT) {
      r[x, y] = true;
    }
  }

  public virtual void Fusion(int x, int y, ref bool[,] r) {
    Token t;
    if (x >= 0 && x < WIDGHT && y >= 0 && y < HEIGHT) {
      t = BoarManager.Instance.TokensInBoard[x, y];

      if (t != null) {
        if (!t.isEnemy && t.typeToken < 5 && !isEnemy) {
          r[x, y] = true;
        } else if (t.isEnemy && t.typeToken < 5 && isEnemy) {
          r[x, y] = true;
        }
      }
    }
  }

  public virtual void FusionPattern(int x, int y, ref bool[,] r) {
    if (x >= 0 && x < WIDGHT && y >= 0 && y < HEIGHT) {
      r[x, y] = true;
    }
  }

  public virtual void AttackSpecialAbility(int x, int y, ref bool[,] r) {
    Token t;

    if (x >= 0 && x < WIDGHT && y >= 0 && y < HEIGHT) {
      t = BoarManager.Instance.TokensInBoard[x, y];

      if (t != null) {
        r[x, y] = true;
      }
    }
  }

  public virtual void AttackSpecialAbilityPattern(int x, int y, ref bool[,] r) {
    if (x >= 0 && x < WIDGHT && y >= 0 && y < HEIGHT) {
      r[x, y] = true;
    }
  }

  public virtual void MovementSpecialAbility(int x, int y, ref bool[,] r) {
    Token t;
    if (x >= 0 && x < WIDGHT && y >= 0 && y < HEIGHT) {
      t = BoarManager.Instance.TokensInBoard[x, y];
      if (t == null) {
        r[x, y] = true;
      }
    }
  }

  public virtual void MovementSpecialAbilityPattern(int x, int y, ref bool[,] r) {
    if (x >= 0 && x < WIDGHT && y >= 0 && y < HEIGHT) {
      r[x, y] = true;
    }
  }

  public virtual bool[,] PossibleMove() {
    return new bool[WIDGHT, HEIGHT];
  }

  public virtual bool[,] avalibleAttack() {
    return new bool[WIDGHT, HEIGHT];
  }

  public virtual bool[,] PossibleFusion() {
    return new bool[WIDGHT, HEIGHT];
  }

  public virtual bool[,] MovementPattern() {
    return new bool[WIDGHT, HEIGHT];
  }

  public virtual bool[,] AttackPattern() {
    return new bool[WIDGHT, HEIGHT];
  }

  public virtual bool[,] FusionPattern() {
    return new bool[WIDGHT, HEIGHT];
  }

  public virtual bool[,] PossibleAttackSpecialAbility() {
    return new bool[WIDGHT, HEIGHT];
  }

  public virtual bool[,] PatternAttackSpecialAbility() {
    return new bool[WIDGHT, HEIGHT];
  }

  public virtual bool[,] PossibleMoveSpecialAbility() {
    return new bool[WIDGHT, HEIGHT];
  }

  public virtual bool[,] PatternMovementSpecialAbility() {
    return new bool[WIDGHT, HEIGHT];
  }

  public virtual bool[,] Cure_Stun(int x, int y) {
    return new bool[WIDGHT, HEIGHT];
  }

  public void LeaveTrace(int x, int y) {
    footprints.Add(x);
    footprints.Add(y);
    boardFootprints[x, y] = true;

    if (footprints.Count > trail) {
      boardFootprints[footprints[0], footprints[1]] = false;
      footprints.RemoveRange(0, 2);
    }
  }
}
