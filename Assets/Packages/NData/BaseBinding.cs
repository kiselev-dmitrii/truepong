using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class BaseBinding : MonoBehaviour, EZData.IBinding
{
	public abstract IList<string> ReferencedPaths { get; }
	
	private Dictionary<int, DataContext> _contexts = new Dictionary<int, DataContext>();
	private Dictionary<int, string> _masterPaths = new Dictionary<int, string>();
	
	private static MasterPath GetParentMasterPath(GameObject gameObject)
	{
		var p = gameObject;
		
		if (p.GetComponent<DataContext>() != null)
			return null;
		
		MasterPath component = null;
		while (p != null && component == null)
		{
			if (p.GetComponent<DataContext>() != null)
				return null;
			
			component = p.GetComponent<MasterPath>();
			p = (p.transform.parent == null) ? null : p.transform.parent.gameObject;
		}
		return component;
	}
	
	protected void UpdateMasterPath() {
	    if (this == null || gameObject == null) {
	        Debug.LogWarning("UpdateMasterPath: this is null");
            return;
	    }

		_masterPaths.Clear();
		var p = gameObject;
		
		var lastAddedPath = "";
		var lastAddedDepth = -1;
		var depth = 0;
		
		while (p != null)
		{
			if (p.GetComponent<DataContext>() != null)
				depth++;
			
			var component = p.GetComponent<MasterPath>();
			if (component != null)
			{
				for (var d = lastAddedDepth + 1; d < depth; ++d)
				{
					_masterPaths.Add(d, lastAddedPath);
				}
				lastAddedPath = component.GetFullPath();
				lastAddedDepth = depth;
				_masterPaths.Add(lastAddedDepth, lastAddedPath);
			    if (component.IsRelative) break;
			}
			p = (p.transform.parent == null) ? null : p.transform.parent.gameObject;
		}
	}
	
	protected virtual void Bind()
	{
	}
	
	protected virtual void Unbind()
	{
	}
	
	protected virtual void OnChange()
	{
	}
	
	protected bool _updated = false;
	
	public virtual void UpdateBinding()
	{
		UpdateMasterPath();
		Unbind();
		Bind();
		OnChange();
		_updated = true;
	}
	
	public virtual void Awake()
	{
		
	}
	
	public virtual void Start()
	{
		if (!_updated)
			UpdateBinding();
	}
	
	public void OnContextChange()
	{
		UpdateBinding();
	}
	
	void OnDestroy()
	{
		Unbind();
	}
	
	public DataContext GetContext()
	{
		Debug.LogWarning("GetContext without arguments is deprecated,\n"+
			"you should use GetContext(Path) instead,\n"+
			"otherwise your binding will not support context scope modifiers.");
		return GetContext("#0");
	}

	public DataContext GetContext(string path)
	{
		DataContext context = null;
		if (_contexts.TryGetValue(Utils.GetPathDepth(path), out context))
			return context;
		var depthToGo = Utils.GetPathDepth(path);
		context = Utils.FindRootContext(gameObject, depthToGo);
		if (context != null)
			_contexts.Add(depthToGo, context);
		return context;
	}
	
	public string GetFullCleanPath(string path)
	{
		var depthToGo = Utils.GetPathDepth(path);
		var cleanPath = Utils.GetCleanPath(path);
		
		string masterPath;
		if (_masterPaths.TryGetValue(depthToGo, out masterPath) && !string.IsNullOrEmpty(masterPath))
			return masterPath + "." + cleanPath;
		
		return cleanPath;
	}
	
	private void ClearNullProperties(Dictionary<System.Type, EZData.Property> properties)
	{
		var nullKeys = new List<System.Type>();
		foreach (var p in properties)
		{
			if (p.Value == null)
				nullKeys.Add(p.Key);
		}
		foreach(var k in nullKeys)
			properties.Remove(k);
	}


    private bool FindAndAddProperty<T>(BaseBinding binding, DataContext context, String path, Dictionary<System.Type, EZData.Property> refProps) {
        var prop = context.FindProperty<T>(path, this);
        if (prop != null) {
            refProps.Add(typeof (T), prop);
            return true;
        } else {
            return false;
        }
    }

	protected void FillTextProperties(Dictionary<System.Type, EZData.Property> properties, string path)
	{
		var context = GetContext(path);
		if (context == null)
		{
			Debug.LogWarning("NguiTextBinding.UpdateBinding - context is null");
			return;
		}

	    if (FindAndAddProperty<string>(this, context, path, properties)) return;
        if (FindAndAddProperty<int>(this, context, path, properties)) return;
        if (FindAndAddProperty<float>(this, context, path, properties)) return;

        //Rare used
        if (FindAndAddProperty<double>(this, context, path, properties)) return;
        if (FindAndAddProperty<decimal>(this, context, path, properties)) return;
        if (FindAndAddProperty<long>(this, context, path, properties)) return;
        if (FindAndAddProperty<ulong>(this, context, path, properties)) return;
        if (FindAndAddProperty<uint>(this, context, path, properties)) return;
        if (FindAndAddProperty<short>(this, context, path, properties)) return;
        if (FindAndAddProperty<ushort>(this, context, path, properties)) return;
        if (FindAndAddProperty<sbyte>(this, context, path, properties)) return;
        if (FindAndAddProperty<byte>(this, context, path, properties)) return;
        if (FindAndAddProperty<DateTime>(this, context, path, properties)) return;
	}
	
	public static object GetTextValue(Dictionary<System.Type, EZData.Property> properties)
	{
		if (properties.ContainsKey(typeof(string)))
			return ((EZData.Property<string>)(properties[typeof(string)])).GetValue();
		if (properties.ContainsKey(typeof(double)))
			return ((EZData.Property<double>)(properties[typeof(double)])).GetValue();
		if (properties.ContainsKey(typeof(float)))
			return ((EZData.Property<float>)(properties[typeof(float)])).GetValue();
#if !UNITY_FLASH
		if (properties.ContainsKey(typeof(decimal)))
			return ((EZData.Property<decimal>)(properties[typeof(decimal)])).GetValue();
#endif
		if (properties.ContainsKey(typeof(long)))
			return ((EZData.Property<long>)(properties[typeof(long)])).GetValue();
		if (properties.ContainsKey(typeof(ulong)))
			return ((EZData.Property<ulong>)(properties[typeof(ulong)])).GetValue();
		if (properties.ContainsKey(typeof(int)))
			return ((EZData.Property<int>)(properties[typeof(int)])).GetValue();
		if (properties.ContainsKey(typeof(uint)))
			return ((EZData.Property<uint>)(properties[typeof(uint)])).GetValue();
		if (properties.ContainsKey(typeof(short)))
			return ((EZData.Property<short>)(properties[typeof(short)])).GetValue();
		if (properties.ContainsKey(typeof(ushort)))
			return ((EZData.Property<ushort>)(properties[typeof(ushort)])).GetValue();
		if (properties.ContainsKey(typeof(sbyte)))
			return ((EZData.Property<sbyte>)(properties[typeof(sbyte)])).GetValue();
		if (properties.ContainsKey(typeof(byte)))
			return ((EZData.Property<byte>)(properties[typeof(byte)])).GetValue();
		if (properties.ContainsKey(typeof(DateTime)))
			return ((EZData.Property<DateTime>)(properties[typeof(DateTime)])).GetValue();
		
		return string.Empty;
	}
	
	protected void SetTextValue(Dictionary<System.Type, EZData.Property> properties, string val)
	{
		if (properties.ContainsKey(typeof(string)))
		{
			((EZData.Property<string>)(properties[typeof(string)])).SetValue(val);
		}
		if (properties.ContainsKey(typeof(double)))
		{
			double v = 0;
			if (double.TryParse(val, out v))
				((EZData.Property<double>)(properties[typeof(double)])).SetValue(v);
		}
		if (properties.ContainsKey(typeof(float)))
		{
			float v = 0;
			if (float.TryParse(val, out v))
				((EZData.Property<float>)(properties[typeof(float)])).SetValue(v);
		}
#if !UNITY_FLASH
		if (properties.ContainsKey(typeof(decimal)))
		{
			decimal v = 0;
			if (decimal.TryParse(val, out v))
				((EZData.Property<decimal>)(properties[typeof(decimal)])).SetValue(v);
		}
#endif
		if (properties.ContainsKey(typeof(long)))
		{
			long v = 0;
			if (long.TryParse(val, out v))
				((EZData.Property<long>)(properties[typeof(long)])).SetValue(v);
		}
		if (properties.ContainsKey(typeof(ulong)))
		{
			ulong v = 0;
			if (ulong.TryParse(val, out v))
				((EZData.Property<ulong>)(properties[typeof(ulong)])).SetValue(v);
		}
		if (properties.ContainsKey(typeof(int)))
		{
			int v = 0;
			if (int.TryParse(val, out v))
				((EZData.Property<int>)(properties[typeof(int)])).SetValue(v);
		}
		if (properties.ContainsKey(typeof(uint)))
		{
			uint v = 0;
			if (uint.TryParse(val, out v))
				((EZData.Property<uint>)(properties[typeof(uint)])).SetValue(v);
		}
		if (properties.ContainsKey(typeof(short)))
		{
			short v = 0;
			if (short.TryParse(val, out v))
				((EZData.Property<short>)(properties[typeof(short)])).SetValue(v);
		}
		if (properties.ContainsKey(typeof(ushort)))
		{
			ushort v = 0;
			if (ushort.TryParse(val, out v))
				((EZData.Property<ushort>)(properties[typeof(ushort)])).SetValue(v);
		}
		if (properties.ContainsKey(typeof(sbyte)))
		{
			sbyte v = 0;
			if (sbyte.TryParse(val, out v))
				((EZData.Property<sbyte>)(properties[typeof(sbyte)])).SetValue(v);
		}
		if (properties.ContainsKey(typeof(byte)))
		{
			byte v = 0;
			if (byte.TryParse(val, out v))
				((EZData.Property<byte>)(properties[typeof(byte)])).SetValue(v);
		}
	}
	
	protected void FillNumericProperties(Dictionary<System.Type, EZData.Property> properties, string path)
	{
		var context = GetContext(path);
		if (context == null)
		{
			Debug.LogWarning("FillNumericProperties - context is null");
			return;
		}
		
		properties.Add(typeof(float), context.FindProperty<float>(path, this));
		properties.Add(typeof(double), context.FindProperty<double>(path, this));
#if !UNITY_FLASH
		properties.Add(typeof(decimal), context.FindProperty<decimal>(path, this));
#endif
		properties.Add(typeof(long), context.FindProperty<long>(path, this));
		properties.Add(typeof(ulong), context.FindProperty<ulong>(path, this));
		properties.Add(typeof(int), context.FindProperty<int>(path, this));
		properties.Add(typeof(uint), context.FindProperty<uint>(path, this));
		properties.Add(typeof(short), context.FindProperty<short>(path, this));
		properties.Add(typeof(ushort), context.FindProperty<ushort>(path, this));
		properties.Add(typeof(sbyte), context.FindProperty<sbyte>(path, this));
		properties.Add(typeof(byte), context.FindProperty<byte>(path, this));
		
		ClearNullProperties(properties);
	}
	
	protected static double GetNumericValue(Dictionary<System.Type, EZData.Property> properties)
	{
		if (properties.ContainsKey(typeof(double)))
			return ((EZData.Property<double>)(properties[typeof(double)])).GetValue();
		if (properties.ContainsKey(typeof(float)))
			return ((EZData.Property<float>)(properties[typeof(float)])).GetValue();
#if !UNITY_FLASH
		if (properties.ContainsKey(typeof(decimal)))
			return (double)((EZData.Property<decimal>)(properties[typeof(decimal)])).GetValue();
#endif
		if (properties.ContainsKey(typeof(long)))
			return ((EZData.Property<long>)(properties[typeof(long)])).GetValue();
		if (properties.ContainsKey(typeof(UInt64)))
			return ((EZData.Property<ulong>)(properties[typeof(ulong)])).GetValue();
		if (properties.ContainsKey(typeof(int)))
			return ((EZData.Property<int>)(properties[typeof(int)])).GetValue();
		if (properties.ContainsKey(typeof(uint)))
			return ((EZData.Property<uint>)(properties[typeof(uint)])).GetValue();
		if (properties.ContainsKey(typeof(short)))
			return ((EZData.Property<short>)(properties[typeof(short)])).GetValue();
		if (properties.ContainsKey(typeof(ushort)))
			return ((EZData.Property<ushort>)(properties[typeof(ushort)])).GetValue();
		if (properties.ContainsKey(typeof(sbyte)))
			return ((EZData.Property<sbyte>)(properties[typeof(sbyte)])).GetValue();
		if (properties.ContainsKey(typeof(byte)))
			return ((EZData.Property<byte>)(properties[typeof(byte)])).GetValue();
		return 0;
	}
	
	protected static void SetNumericValue(Dictionary<System.Type, EZData.Property> properties, double val)
	{
		if (properties.ContainsKey(typeof(double)))
			((EZData.Property<double>)(properties[typeof(double)])).SetValue(val);
		if (properties.ContainsKey(typeof(float)))
			((EZData.Property<float>)(properties[typeof(float)])).SetValue((float)val);
#if !UNITY_FLASH
		if (properties.ContainsKey(typeof(decimal)))
			((EZData.Property<decimal>)(properties[typeof(decimal)])).SetValue((decimal)val);
#endif
		if (properties.ContainsKey(typeof(long)))
			((EZData.Property<long>)(properties[typeof(long)])).SetValue((long)val);
		if (properties.ContainsKey(typeof(ulong)))
			((EZData.Property<ulong>)(properties[typeof(ulong)])).SetValue((ulong)val);
		if (properties.ContainsKey(typeof(int)))
			((EZData.Property<int>)(properties[typeof(int)])).SetValue((int)val);
		if (properties.ContainsKey(typeof(uint)))
			((EZData.Property<uint>)(properties[typeof(uint)])).SetValue((uint)val);
		if (properties.ContainsKey(typeof(short)))
			((EZData.Property<short>)(properties[typeof(short)])).SetValue((short)val);
		if (properties.ContainsKey(typeof(ushort)))
			((EZData.Property<ushort>)(properties[typeof(ushort)])).SetValue((ushort)val);
		if (properties.ContainsKey(typeof(sbyte)))
			((EZData.Property<sbyte>)(properties[typeof(sbyte)])).SetValue((sbyte)val);
		if (properties.ContainsKey(typeof(byte)))
			((EZData.Property<byte>)(properties[typeof(byte)])).SetValue((byte)val);
	}
}
