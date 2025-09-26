using Microsoft.Maui.Dispatching;

namespace App
{
    public partial class MainPage : ContentPage
    {
        // Timer for pomodoro
        private IDispatcherTimer _timer;
        
        // Default durations in minutes
        private const int WorkDurationMinutes = 25;
        private const int ShortBreakDurationMinutes = 5;
        private const int LongBreakDurationMinutes = 15;
        
        // Counters
        private TimeSpan _remainingTime;
        private int _pomodoroCount = 0;
        private bool _isWorkTime = true;
        
        // Timer state
        private bool _isRunning = false;

        public MainPage()
        {
            InitializeComponent();
            
            // Initialize the timer
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
            
            ResetTimer();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            // Update remaining time
            _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
            
            UpdateTimerDisplay();
            
            // Check if timer has finished
            if (_remainingTime.TotalSeconds <= 0)
            {
                TimerCompleted();
            }
        }

        private void UpdateTimerDisplay()
        {
            TimerLabel.Text = $"{_remainingTime.Minutes:D2}:{_remainingTime.Seconds:D2}";
        }

        private void TimerCompleted()
        {
            _timer.Stop();
            _isRunning = false;
            
            if (_isWorkTime)
            {
                // Work period completed
                _pomodoroCount++;
                _isWorkTime = false;
                
                // After every 4 pomodoros, take a long break
                if (_pomodoroCount % 4 == 0)
                {
                    _remainingTime = TimeSpan.FromMinutes(LongBreakDurationMinutes);
                    StateLabel.Text = "Long Break";
                }
                else
                {
                    _remainingTime = TimeSpan.FromMinutes(ShortBreakDurationMinutes);
                    StateLabel.Text = "Short Break";
                }
                
                DisplayAlert("Timer Completed", "Work period completed! Time for a break.", "OK");
            }
            else
            {
                // Break period completed
                _isWorkTime = true;
                _remainingTime = TimeSpan.FromMinutes(WorkDurationMinutes);
                StateLabel.Text = "Work Time";
                
                DisplayAlert("Timer Completed", "Break period completed! Back to work.", "OK");
            }
            
            UpdateTimerDisplay();
            UpdateButtonStates();
        }

        private void OnStartClicked(object sender, EventArgs e)
        {
            if (!_isRunning)
            {
                _timer.Start();
                _isRunning = true;
                UpdateButtonStates();
            }
        }

        private void OnPauseClicked(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                _timer.Stop();
                _isRunning = false;
                UpdateButtonStates();
            }
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            ResetTimer();
        }

        private void ResetTimer()
        {
            _timer.Stop();
            _isRunning = false;
            
            _isWorkTime = true;
            _remainingTime = TimeSpan.FromMinutes(WorkDurationMinutes);
            StateLabel.Text = "Work Time";
            
            UpdateTimerDisplay();
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            StartButton.IsEnabled = !_isRunning;
            PauseButton.IsEnabled = _isRunning;
        }
    }
}
