package md5f28cd308db8490dcf79fe9aa89d12ed2;


public class FieldInfoViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("DesignLibrary_Tutorial.FieldInfoViewHolder, DesignLibrary_Tutorial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", FieldInfoViewHolder.class, __md_methods);
	}


	public FieldInfoViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == FieldInfoViewHolder.class)
			mono.android.TypeManager.Activate ("DesignLibrary_Tutorial.FieldInfoViewHolder, DesignLibrary_Tutorial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Views.View, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
