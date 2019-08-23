using System;
using System.Diagnostics;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace RandPad
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private int _cursor = 0;
        private int count = 0;
        private string question;
        private bool isButtonEventHooked = false;
        private Stopwatch stopWatch;
        private JavaList<Button> buttons;
        private TimeSpan topRecord;
        private TimeSpan totalElapsed;
        private ToggleButton proToggleButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.keypad_layout);

            Initialize();
            Randomize();
        }

        private void Initialize()
        {
            topRecord = TimeSpan.MaxValue;
            totalElapsed = new TimeSpan(0);
            stopWatch = new Stopwatch();
            buttons = new JavaList<Button>();
            buttons.Add(FindViewById<Button>(Resource.Id.button0));
            buttons.Add(FindViewById<Button>(Resource.Id.button1));
            buttons.Add(FindViewById<Button>(Resource.Id.button2));
            buttons.Add(FindViewById<Button>(Resource.Id.button3));
            buttons.Add(FindViewById<Button>(Resource.Id.button4));
            buttons.Add(FindViewById<Button>(Resource.Id.button5));
            buttons.Add(FindViewById<Button>(Resource.Id.button6));
            buttons.Add(FindViewById<Button>(Resource.Id.button7));
            buttons.Add(FindViewById<Button>(Resource.Id.button8));
            buttons.Add(FindViewById<Button>(Resource.Id.button9));
            buttons.Add(FindViewById<Button>(Resource.Id.button10));

            proToggleButton = FindViewById<ToggleButton>(Resource.Id.toggleButton1);
            proToggleButton.Click += delegate
            {
                Randomize();
            };
        }

        private void Randomize()
        {
            stopWatch.Reset();
            _cursor = 0;
            question = Get10DigitRandomDecimalNumbers(false);
            
            
            var keypadMapping = proToggleButton.Checked ? Get10DigitRandomDecimalNumbers(true) : "0789456123.";
            var numberTextView = FindViewById<TextView>(Resource.Id.textView1);
            
            numberTextView.Text = question;


            for (var i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i];
                button.Text = keypadMapping[i].ToString();

                if (!isButtonEventHooked)
                {
                    button.Click += delegate
                    {
                        OnKeyPadClicked(numberTextView, button);
                    };
                }
            }

            isButtonEventHooked = true;

        }
        public void OnKeyPadClicked (TextView textView, Button button)
        {
            if(_cursor == 0 && !stopWatch.IsRunning)
            {
                stopWatch.Start();
            }
            if (question[_cursor].ToString().Equals(button.Text))
            {
                _cursor++;
                textView.TextFormatted = Html.FromHtml($"<font color='grey'>{question.Substring(0, _cursor)}</font>{question.Substring(_cursor)}", FromHtmlOptions.ModeLegacy);
            } else
            {
                _cursor = 0;
                textView.Text = question;
                ShowToast("Ha! Wrong key!!");
            }

            if(_cursor == question.Length)
            {
                stopWatch.Stop();
                ShowToast($"Your record: {GetSeconds(stopWatch.Elapsed)}");
                if(topRecord > stopWatch.Elapsed)
                {
                    topRecord = stopWatch.Elapsed;
                    var topRecordTextView = FindViewById<TextView>(Resource.Id.textView2);
                    topRecordTextView.Text = $"Top Record: {GetSeconds(topRecord)}";
                }
                totalElapsed += stopWatch.Elapsed;

                var averageTextView = FindViewById<TextView>(Resource.Id.textView3);
                averageTextView.Text = $"Average: {GetSeconds(totalElapsed / ++count)}";

                var countTextView = FindViewById<TextView>(Resource.Id.textView4);
                countTextView.Text = $"Solved: {count}";
                Randomize();
            }
        }

        private string GetSeconds(TimeSpan timeSpan)
        {
             return $"{timeSpan.TotalSeconds:F}";
        }
        
        private void ShowToast(string message, ToastLength toastLength = ToastLength.Short)
        {
            Toast.MakeText(Application.Context, message, toastLength).Show();
        }

        private string Get10DigitRandomDecimalNumbers(bool isForKeyMapping)
        {
            var rand = new Random();
            var numbers = string.Empty;
            
            while (numbers.Length < 10)
            {
                numbers = string.Concat(numbers, GetNextUniqueRandomNumber(rand, numbers));
            }

            int dotIndex;

            if (isForKeyMapping)
            {
                dotIndex = rand.Next(0, 10);
            } else
            {
                dotIndex = rand.Next(1, 9);
            }

            numbers = numbers.Insert(dotIndex, ".");

            return numbers;
        }

        private string GetNextUniqueRandomNumber(Random rand, string numbers)
        {
            string randomNumber;
            do
            {
                randomNumber = (rand.Next() % 10).ToString();

            } while (numbers.Contains(randomNumber));

            return randomNumber;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}

