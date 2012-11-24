using System;

namespace LWSlideViewController
{
	public enum LWSlideViewControllerState {
		Normal = 0,
		Dragging,
		Peeking,
		DrilledDown,
	}

	public enum LWSlideViewControllerMode {
		NavigationBarOnly = 1,
		WholeView = 2,
		ControllerOnly = 3,
		AllViewController = 4
	}
}

