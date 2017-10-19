using System.Windows;
using Microsoft.Gestures;
using Microsoft.Gestures.Endpoint;
using System.Windows.Media;
using System;
using System.Collections.Generic;

namespace Microsoft.Gestures.Samples.RotateSample
{
    public partial class MainWindow : Window
    {
        private GesturesServiceEndpoint _gesturesService;
        private Gesture _rotateGesture;
        private int _rotateTimes = 0;




        public MainWindow()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            List<Finger> fingers = new List<Finger>();

            fingers.Add(Finger.Index);
            fingers.Add(Finger.Middle);
            fingers.Add(Finger.Ring);
        //    fingers.Add(Finger.Pinky);

            // Step 1: Connect to Microsoft Gestures service
            _gesturesService = GesturesServiceEndpointFactory.Create();
            _gesturesService.StatusChanged += (s, arg) => Dispatcher.Invoke(() => GesturesServiceStatus.Text = $"[{arg.Status}]");
            Closed += (s, arg) => _gesturesService?.Dispose();
            await _gesturesService.ConnectAsync();

            // Step 2: Define your custom gesture 
            // Start with defining the first pose, ...
            var hold = new HandPose("Hold", new FingerPose(new[] { Finger.Thumb, Finger.Index, Finger.Ring }, FingerFlexion.Open, PoseDirection.Right),
                                            new FingertipDistanceRelation(fingers, RelativeDistance.NotTouching, Finger.Thumb),
                                            new FingertipPlacementRelation(fingers, RelativePlacement.Above, Finger.Thumb));
            // ... define the second pose, ...
            var rotate = new HandPose("Rotate", new FingerPose(new[] { Finger.Thumb, Finger.Index, Finger.Ring }, FingerFlexion.Open, PoseDirection.Forward),
                                                new FingertipDistanceRelation(fingers, RelativeDistance.NotTouching, Finger.Thumb),
                                                new FingertipPlacementRelation(fingers, RelativePlacement.Above, Finger.Thumb));

            // ... finally define the gesture using the hand pose objects defined above forming a simple state machine: hold -> rotate
            _rotateGesture = new Gesture("RotateRight", hold, rotate);
             
            _rotateGesture.Triggered += (s, args) => Dispatcher.Invoke(() => Arrow.RenderTransform = new RotateTransform(++_rotateTimes * 90, Arrow.ActualWidth / 2, Arrow.ActualHeight / 2));

            var xamlGesture =_rotateGesture.ToXaml();

            Console.Write(xamlGesture);

            // Step 3: Register the gesture (When window focus is lost (gained) the service will automatically unregister (register) the gesture)
            //         To manually control the gesture registration, pass 'isGlobal: true' parameter in the function call below
            await _gesturesService.RegisterGesture(_rotateGesture);

        }

        private void OnAnimatedHelpEnded(object sender, RoutedEventArgs e)
        {
            animatedHelp.Position = new TimeSpan(0, 0, 1);
            animatedHelp.Play();
        }
    }
}
