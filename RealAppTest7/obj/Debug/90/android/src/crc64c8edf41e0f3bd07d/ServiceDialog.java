package crc64c8edf41e0f3bd07d;


public class ServiceDialog
	extends android.app.DialogFragment
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreateDialog:(Landroid/os/Bundle;)Landroid/app/Dialog;:GetOnCreateDialog_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("RealAppTest7.ServiceDialog, RealAppTest7", ServiceDialog.class, __md_methods);
	}


	public ServiceDialog ()
	{
		super ();
		if (getClass () == ServiceDialog.class)
			mono.android.TypeManager.Activate ("RealAppTest7.ServiceDialog, RealAppTest7", "", this, new java.lang.Object[] {  });
	}

	public ServiceDialog (android.content.Context p0, int p1)
	{
		super ();
		if (getClass () == ServiceDialog.class)
			mono.android.TypeManager.Activate ("RealAppTest7.ServiceDialog, RealAppTest7", "Android.Content.Context, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1 });
	}


	public android.app.Dialog onCreateDialog (android.os.Bundle p0)
	{
		return n_onCreateDialog (p0);
	}

	private native android.app.Dialog n_onCreateDialog (android.os.Bundle p0);

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
