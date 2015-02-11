using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IGameState {
	GameObject GameLogic { get; set; }
	ICollection<Constants.InputEvents> InputEvents { get; }
	IEnumerator Start();
	IEnumerator End();
	void MouseClick(InputEventArgs e);
	void MouseHeld(InputEventArgs e);
	void MouseUp(InputEventArgs e);
	void KeyClick(InputEventArgs e);
	void KeyHeld(InputEventArgs e);
	void KeyUp(InputEventArgs e);
}