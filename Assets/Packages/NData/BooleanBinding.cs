using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class BooleanBinding : Binding
{
	private readonly Dictionary<Type, EZData.Property> _properties = new Dictionary<Type, EZData.Property>();
	
	public enum CHECK_TYPE
	{
		BOOLEAN,
		EQUAL_TO_REFERENCE,
		GREATER_THAN_REFERENCE,
		LESS_THAN_REFERENCE,
		EMPTY,
		IS_LIST_SELECTION,
        ENUM,
        EQUAL_TO_STRING
	}
	
	public CHECK_TYPE CheckType = CHECK_TYPE.BOOLEAN;
	public double Reference = 0;
    [HideInInspector] public String StringReference;
	
	public bool DefaultValue = false;

	public bool Invert = false;

    public bool EvaluateExpression = false;

    // For EnumBinding only
    [HideInInspector] public String EnumType;
    [HideInInspector] public List<int> EnumValues;

	private bool _ignoreValueChange = false;
	
	private ItemDataContext _listItem;
    private bool _allTokensSubscribed = false;
    private List<Evaluator.Token> _evalTokens; 	

	public override void Awake()
	{
		base.Awake();

	    if (!EvaluateExpression) {
	        _properties.Add(typeof (bool), null);
	        _properties.Add(typeof (int), null);
	        _properties.Add(typeof (Enum), null);
	        _properties.Add(typeof (float), null);
	        _properties.Add(typeof (double), null);
	        _properties.Add(typeof (string), null);
	    }
	}
	
	void Update()
	{
		if (CheckType == CHECK_TYPE.IS_LIST_SELECTION && _listItem == null)
		{
			_listItem = Utils.GetComponentInParents<ItemDataContext>(gameObject);
			if (_listItem != null)
			{
				_listItem.OnSelectedChange += OnChange;
				OnChange();
			}
		} 
	}
		
	protected override void Unbind() {
		base.Unbind();

	    if (!EvaluateExpression) {
	        foreach (var p in _properties) {
	            if (p.Value != null) p.Value.OnChange -= OnChange;
	        }

	        var keys = _properties.Keys.ToArray();
	        foreach (var key in keys) _properties[key] = null;
	    
        } else {
	        if (_evalTokens != null) {
                foreach (var token in _evalTokens) {
                    var variable = token as Evaluator.Variable;
                    if (variable != null) variable.Unsubscribe(OnChange);
                }
                _evalTokens = null;
	            _allTokensSubscribed = false;
	        }
	    }
	}
	
	protected override void Bind() {
		base.Bind();

	    if (!EvaluateExpression) {
	        var context = GetContext(Path);
	        if (context != null) {
	            _properties[typeof (bool)] = context.FindProperty<bool>(Path, this);
	            _properties[typeof (int)] = context.FindProperty<int>(Path, this);
	            _properties[typeof (Enum)] = context.FindEnumProperty(Path, this);
	            _properties[typeof (float)] = context.FindProperty<float>(Path, this);
	            _properties[typeof (double)] = context.FindProperty<double>(Path, this);
	            _properties[typeof (string)] = context.FindProperty<string>(Path, this);
	        }

	        foreach (var p in _properties) {
	            if (p.Value != null) p.Value.OnChange += OnChange;
	        }
	    } else {
	        var tokens = Evaluator.ParseTokens(Path);
	        _evalTokens = Evaluator.ToReversePolishNotation(tokens);

	        foreach (var token in _evalTokens) {
	            var variable = token as Evaluator.Variable;
	            if (variable == null) continue;
                bool isOk = variable.Subscribe(this, OnChange);
	            if (!isOk) {
	                _allTokensSubscribed = false;
	                return;
	            }
	        }
	        _allTokensSubscribed = true;
	    }

	}
	
	protected override void OnChange() {
		base.OnChange();
		
		var newValue = DefaultValue;

	    if (!EvaluateExpression) {
	        if (CheckType == CHECK_TYPE.BOOLEAN) {
	            if (_properties[typeof (bool)] != null) {
	                newValue = ((EZData.Property<bool>) _properties[typeof (bool)]).GetValue();
	            }
	        } else if (CheckType == CHECK_TYPE.EMPTY) {
	            if (_properties[typeof (string)] != null) newValue = string.IsNullOrEmpty(((EZData.Property<string>) _properties[typeof (string)]).GetValue());
	        } else if (CheckType == CHECK_TYPE.IS_LIST_SELECTION) {
	            if (_listItem != null) newValue = _listItem.Selected;
            } else if (CheckType == CHECK_TYPE.ENUM) {
                if (_properties[typeof(int)] != null) {
                    int num = ((EZData.Property<int>)_properties[typeof(int)]).GetValue();
                    newValue = EnumValues.Contains(num);
                }
	        } else if (CheckType == CHECK_TYPE.EQUAL_TO_STRING) {
	            if (_properties[typeof (string)] != null) {
	                String val = ((EZData.Property<string>) _properties[typeof (string)]).GetValue();
	                newValue = (val == StringReference);
	            }
	        } else {
	            var val = 0.0;
	            if (_properties[typeof (int)] != null) val = ((EZData.Property<int>) _properties[typeof (int)]).GetValue();
	            if (_properties[typeof (Enum)] != null) val = ((EZData.Property<int>) _properties[typeof (Enum)]).GetValue();
	            if (_properties[typeof (float)] != null) val = ((EZData.Property<float>) _properties[typeof (float)]).GetValue();
	            if (_properties[typeof (double)] != null) val = ((EZData.Property<double>) _properties[typeof (double)]).GetValue();

	            switch (CheckType) {
	                case CHECK_TYPE.EQUAL_TO_REFERENCE:
	                    newValue = (val == Reference);
	                    break;
	                case CHECK_TYPE.GREATER_THAN_REFERENCE:
	                    newValue = (val > Reference);
	                    break;
	                case CHECK_TYPE.LESS_THAN_REFERENCE:
	                    newValue = (val < Reference);
	                    break;
	            }
	        }
	    } else {
	        if (_allTokensSubscribed) {
                newValue = Evaluator.EvaluateTokens(_evalTokens) != 0;
	        }
	    }

	    if (!_ignoreValueChange) {
	        ApplyNewValue(Invert ? (!newValue) : newValue);
	    }
	}
	
	protected virtual void ApplyInputValue(bool inputValue) {
		if (CheckType != CHECK_TYPE.BOOLEAN) return;
	    if (EvaluateExpression) return;
		
		inputValue = Invert ? (!inputValue) : inputValue;
		
		_ignoreValueChange = true;
		
		if (_properties[typeof(bool)] != null)
			((EZData.Property<bool>)_properties[typeof(bool)]).SetValue(inputValue);
		
		_ignoreValueChange = false;
	}
	
	protected virtual void ApplyNewValue(bool newValue) {
		Debug.LogError("Not supposed to be here for " + Path);
	}
}
