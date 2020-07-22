using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeybindFunction {
  I_INVENTORY, I_WEAPON_ABILITIES, I_SKILLS, I_INTERACT, I_CRAFTING, I_WEAPON_WHEEL,

  A_MOVEUP, A_MOVELEFT, A_MOVEDOWN, A_MOVERIGHT,
  A_FIRSTSKILL, A_SECONDSKILL, A_THIRDSKILL, A_FOURTHSKILL, A_SKILLMODIFIER,

  D_SELLEFT, D_SELRIGHT, D_ADDEXP, D_SAVECHAR, D_LOADCHAR, D_UNEQUIPITEM, D_GENERATEITEM, D_DRAW_AI, D_RESET_UI
  };

public class KeybindManager : MonoBehaviour
{
  public static Dictionary<KeybindFunction, KeyCode> Keybinds;

  void Awake()
  {
    Keybinds = new Dictionary<KeybindFunction, KeyCode>();
    //Keybinds.Add(KeybindFunction, KeyCode);

    #region Interfaces
    Keybinds.Add(KeybindFunction.I_INVENTORY, KeyCode.I);
    Keybinds.Add(KeybindFunction.I_SKILLS, KeyCode.H);
    Keybinds.Add(KeybindFunction.I_WEAPON_ABILITIES, KeyCode.P);
		Keybinds.Add(KeybindFunction.I_INTERACT, KeyCode.E);
		Keybinds.Add(KeybindFunction.I_CRAFTING, KeyCode.K);
		Keybinds.Add(KeybindFunction.I_WEAPON_WHEEL, KeyCode.Space);
    #endregion

    #region Actions
    //Movement
    Keybinds.Add(KeybindFunction.A_MOVEUP, KeyCode.W);
    Keybinds.Add(KeybindFunction.A_MOVELEFT, KeyCode.A);
    Keybinds.Add(KeybindFunction.A_MOVEDOWN, KeyCode.S);
    Keybinds.Add(KeybindFunction.A_MOVERIGHT, KeyCode.D);

    //Abilities
    Keybinds.Add(KeybindFunction.A_FIRSTSKILL, KeyCode.Alpha1);
    Keybinds.Add(KeybindFunction.A_SECONDSKILL, KeyCode.Alpha2);
    Keybinds.Add(KeybindFunction.A_THIRDSKILL, KeyCode.Alpha3);
    Keybinds.Add(KeybindFunction.A_FOURTHSKILL, KeyCode.Alpha4);
    Keybinds.Add(KeybindFunction.A_SKILLMODIFIER, KeyCode.LeftShift);
    #endregion

    #region Debug
    //Free binds available for debugging use.
    //Keybinds.Add(KeybindFunction.D_SELLEFT, KeyCode.LeftArrow);
    //Keybinds.Add(KeybindFunction.D_SELRIGHT, KeyCode.RightArrow);

    //Adding additional exp to all skills
    Keybinds.Add(KeybindFunction.D_ADDEXP, KeyCode.Alpha0);

    //Saving/Loading character data
    //Keybinds.Add(KeybindFunction.D_SAVECHAR, KeyCode.M);
    //Keybinds.Add(KeybindFunction.D_LOADCHAR, KeyCode.N);

    //Unequipping Items
    //Keybinds.Add(KeybindFunction.D_UNEQUIPITEM, KeyCode.X);

    //Generate new item in inventory
    //Keybinds.Add(KeybindFunction.D_GENERATEITEM, KeyCode.F);

		//Draw AI Grid
		Keybinds.Add(KeybindFunction.D_DRAW_AI, KeyCode.Alpha9);

		//Reset UI Layout to Default
		Keybinds.Add(KeybindFunction.D_RESET_UI, KeyCode.Alpha8);
    #endregion
  }
}
