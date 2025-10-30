using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;

namespace ScienceProgrammProject.Core.Scripts
{
    public static class PageTransition
    {
        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.RegisterAttached(
                "Transition",
                typeof(Storyboard),
                typeof(PageTransition),
                new PropertyMetadata(null, OnTransitionChanged));

        public static void SetTransition(DependencyObject obj, Storyboard value)
        {
            obj.SetValue(TransitionProperty, value);
        }

        public static Storyboard GetTransition(DependencyObject obj)
        {
            return (Storyboard)obj.GetValue(TransitionProperty);
        }

        private static void OnTransitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Frame frame)
            {
                frame.Navigated += (s, args) =>
                {
                    if (args.Content is FrameworkElement newContent)
                    {
                        var transition = GetTransition(frame);
                        if (transition != null)
                        {
                            transition.Begin(newContent);
                        }
                    }
                };
            }
        }
    }
}
