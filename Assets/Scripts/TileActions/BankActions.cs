using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BankActions : BaseTileActions {
	public override IEnumerator OnTile() {
		if(GameState.PlayerScript.CanLevelUp()) {
			GameState.PlayerScript.LevelUp();
			UIManager.Instance.Message.transform.FindChild("Text").GetComponent<Text>().text = "Level Up!";
			UIManager.Instance.Message.SetActive(true);
			yield return new WaitForSeconds(2f);
			UIManager.Instance.Message.SetActive(false);
		}
		yield break;
	}

	public override IEnumerator LandOnTile() {
		FinishTurn();
		yield break;
	}
}
