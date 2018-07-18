using UnityEngine;

public class RootContext : DataContext {
	public EZData.MonoBehaviourContext defaultContext;
	
	void Awake() {
		if (defaultContext != null && _context == null)
			SetContext(defaultContext);
	}
	
	public void SetContext(EZData.IContext context) {
		_context = context;
	}
}
