using CommunityToolkit.Mvvm.Messaging;

namespace datawedge_MAUI_SampleApp;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();

		WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
		{
			MainThread.BeginInvokeOnMainThread(() => { lbDisplayBarcodeData.Text += "\n"+m; });
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
}

