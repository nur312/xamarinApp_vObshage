package md5f385ba3f651b46c9daf445d016a0d98d;


public class ScheduledAlarmHandler
	extends android.content.BroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onReceive:(Landroid/content/Context;Landroid/content/Intent;)V:GetOnReceive_Landroid_content_Context_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("appUI.Droid.Persistents.ScheduledAlarmHandler, appUI.Android", ScheduledAlarmHandler.class, __md_methods);
	}


	public ScheduledAlarmHandler ()
	{
		super ();
		if (getClass () == ScheduledAlarmHandler.class)
			mono.android.TypeManager.Activate ("appUI.Droid.Persistents.ScheduledAlarmHandler, appUI.Android", "", this, new java.lang.Object[] {  });
	}


	public void onReceive (android.content.Context p0, android.content.Intent p1)
	{
		n_onReceive (p0, p1);
	}

	private native void n_onReceive (android.content.Context p0, android.content.Intent p1);

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
