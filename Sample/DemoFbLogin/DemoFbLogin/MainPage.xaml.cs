using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomosTechies.Facebook;
using Xamarin.Forms;

namespace DemoFbLogin
{
	public partial class MainPage : ContentPage
	{
	    private readonly IFacebookManagerService _facebookService;

	    public MainPage()
		{
			InitializeComponent();
		    _facebookService = DependencyService.Get<IFacebookManagerService>();
		}

	    private async void Login_Clicked(object sender, EventArgs e)
	    {
	        var resp = await _facebookService.Login();

	        this.FbStatus.Text = resp.IsCorrect ? 
	            $"{resp.User.FirstName} {resp.User.LastName} \n {resp.User.Email}" : 
	            resp.Message;
	    }
	}
}
