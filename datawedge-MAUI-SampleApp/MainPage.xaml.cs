using CommunityToolkit.Mvvm.Messaging;
using Java.Lang;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace datawedge_MAUI_SampleApp;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();

		WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
		{
			if(m.Length>2)
				MainThread.BeginInvokeOnMainThread(() => { lbDisplayBarcodeData.Text += "\n" + m; });
		});
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

    private void OnDWOFFClicked(object sender, EventArgs e)
    {
        WeakReferenceMessenger.Default.Send("11");
        WeakReferenceMessenger.Default.Send("SWITCHING OFF DW");
    }

    private void OnDWONClicked(object sender, EventArgs e)
    {
        WeakReferenceMessenger.Default.Send("22");
        WeakReferenceMessenger.Default.Send("SWITCHING ON DW");
    }    
	
	private void OnDWGetActiveProfile(object sender, EventArgs e)
    {
        WeakReferenceMessenger.Default.Send("33");
        WeakReferenceMessenger.Default.Send("GETTING ACTIVE PROFILE");
    }
}

