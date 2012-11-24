using System;

namespace LWSlideViewController
{
	enum LWSlideViewControllerState {
		Normal = 0,
		Dragging,
		Peeking,
		DrilledDown,
		Searching
	}

	enum LWSlideViewControllerMode {
		NavigationBarOnly = 1,
		WholeView = 2,
		ControllerOnly = 3,
		AllViewController = 4
	}
}

