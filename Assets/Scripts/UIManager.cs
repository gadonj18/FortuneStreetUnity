using UnityEngine;
using System.Collections;

public class UIManager : Singleton<UIManager> {
	protected UIManager () {}

	private GameObject actionMenu;
	public GameObject ActionMenu {
		get { return actionMenu; }
	}
	private GameObject diceMoves;
	public GameObject DiceMoves {
		get { return diceMoves; }
	}
	private GameObject dice;
	public GameObject Dice {
		get { return dice; }
	}
	private GameObject yesNoMenu;
	public GameObject YesNoMenu {
		get { return yesNoMenu; }
	}
	private GameObject unownedPropertyInfo;
	public GameObject UnownedPropertyInfo {
		get { return unownedPropertyInfo; }
	}
	private GameObject playerScores;
	public GameObject PlayerScores {
		get { return playerScores; }
	}
	private GameObject message;
	public GameObject Message {
		get { return message; }
	}
	private GameObject settleDebtMenu;
	public GameObject SettleDebtMenu {
		get { return settleDebtMenu; }
	}

	public void Awake() {
		actionMenu = GameObject.Find("/UIOverlay/ActionMenu");
		diceMoves = GameObject.Find("/UIOverlay/DiceMoves");
		dice = GameObject.Find("/UICamera/Dice");
		yesNoMenu = GameObject.Find("/UIOverlay/YesNoMenu");
		unownedPropertyInfo = GameObject.Find("/UIOverlay/UnownedPropertyInfo");
		playerScores = GameObject.Find("/UIOverlay/PlayerScores");
		message = GameObject.Find("/UIOverlay/Message");
		settleDebtMenu = GameObject.Find("/UIOverlay/SettleDebtMenu");
	}

	public delegate void UIButtonHandler(UIEventArgs e);
	public static event UIButtonHandler RollButtonClick;
	public static event UIButtonHandler SellStockButtonClick;
	public static event UIButtonHandler SellShopButtonClick;
	public static event UIButtonHandler YesButtonClick;
	public static event UIButtonHandler NoButtonClick;

	public void RollButton_Click() {
		if(RollButtonClick != null) RollButtonClick(new UIEventArgs());
	}
	
	public void YesButton_Click() {
		if(YesButtonClick != null) YesButtonClick(new UIEventArgs());
	}
	
	public void NoButton_Click() {
		if(NoButtonClick != null) NoButtonClick(new UIEventArgs());
	}
	
	public void SellStockButton_Click() {
		if(SellStockButtonClick != null) SellStockButtonClick(new UIEventArgs());
	}
	
	public void SellShopButton_Click() {
		if(SellShopButtonClick != null) SellShopButtonClick(new UIEventArgs());
	}
}

public class UIEventArgs : System.EventArgs {
	private float time;
	public float Time {
		get { return time; }
	}

	public UIEventArgs() {
		time = UnityEngine.Time.unscaledTime;
	}
}