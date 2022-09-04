using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Microsoft.Xna.Framework;

namespace Casino.Android
{
    [Activity(
        Label = "Casino",
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.SensorLandscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden
    )]
    public class Activity1 : AndroidGameActivity
    {
        private Game1 _game;
        private View _view;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _game = new Game1();
            _view = _game.Services.GetService(typeof(View)) as View;

            _game.showKeyBoard = ShowKeyboard;
            _game.hideKeyBoard = HideKeyboard;
            _view.KeyPress += OnKeyPressActivity;

            SetContentView(_view);
            _game.Run();
        }

        private void ShowKeyboard()
        {
            var pView = _game.Services.GetService<View>();
            var inputMethodManager = Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.ShowSoftInput(pView, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        private void HideKeyboard()
        {
            var pView = _game.Services.GetService<View>();
            var inputMethodManager = Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.HideSoftInputFromWindow(pView.WindowToken, HideSoftInputFlags.None);
        }

        private void OnKeyPressActivity(object sender, View.KeyEventArgs e)
        {
            if(e.Event.Action.ToString().Equals("Down"))
            {
                if (e.KeyCode.ToString().Equals("ShiftLeft"))
                {
                    return;
                }
                else if(isKeyCodeNumber(e.KeyCode))
                {
                    _game.handleKeyboardInput(numberAndSymbolKeyCodeToString(e));
                }
                else if(e.KeyCode.ToString().Equals("Del"))
                {
                    _game.handleKeyboardInput(e.KeyCode.ToString());
                }
                else if(e.KeyCode.ToString().Equals("Period"))
                {
                    _game.handleKeyboardInput(".");
                }
                else if(e.KeyCode.ToString().Equals("Space"))
                {
                    _game.handleKeyboardInput(" ");
                }
                else if(isKeyCodeLetter(e.KeyCode))
                {
                    _game.handleKeyboardInput(e.KeyCode.ToString());
                }
            }
        }

        private bool isKeyCodeNumber(Keycode i_keycode)
        {
            int intRepresentationOf0Key = 7;
            int intRepresentationOf9Key = 16;
            return (int)i_keycode >= intRepresentationOf0Key && (int)i_keycode <= intRepresentationOf9Key;
        }

        private bool isKeyCodeLetter(Keycode i_keyCode)
        {
            int intRepresentationOfAKey = 29;
            int intRepresentationOfZKey = 54;
            return (int)i_keyCode >= intRepresentationOfAKey && (int)i_keyCode <= intRepresentationOfZKey;
        }

        private string numberAndSymbolKeyCodeToString(View.KeyEventArgs e)
        {
            if(e.Event.IsShiftPressed)
            {
                switch((int)e.KeyCode - 7)
                {
                    case 1:
                        return "!";
                    case 2:
                        return "@";
                    case 3:
                        return "#";
                    case 4:
                        return "$";
                    case 5:
                        return "%";
                    case 6:
                        return "^";
                    case 7:
                        return "&";
                    case 8:
                        return "*";
                    case 9:
                        return "(";
                    case 0:
                        return ")";
                    default:
                        return "";
                }
            }
            else
            {
                return ((int)e.KeyCode - 7).ToString();
            }
        }
    }
}
