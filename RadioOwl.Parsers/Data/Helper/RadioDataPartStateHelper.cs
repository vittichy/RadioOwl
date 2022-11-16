using System.Windows.Media;

namespace RadioOwl.Parsers.Data.Helper
{
    /// <summary>
    /// Helper pro <see cref="RadioDataPartState"/>
    /// </summary>
    public class RadioDataPartStateHelper
    {
        /// <summary>
        /// Převod <see cref="RadioDataPartState"/> na <see cref="Color"/>
        /// </summary>
        public Color ToColor(RadioDataPartState radioDataPartState)
        {
            Color color;
            switch (radioDataPartState)
            {
                case RadioDataPartState.None:
                    color = Colors.White;
                    break;
                case RadioDataPartState.Parse:
                    color = Colors.Yellow;
                    break;
                case RadioDataPartState.Started:
                    color = Colors.Orange;
                    break;
                case RadioDataPartState.Finnished:
                    color = Colors.LightGreen;
                    break;
                case RadioDataPartState.Error:
                    color = Colors.Red;
                    break;
                case RadioDataPartState.FileAlreadyExists:
                    color = Colors.PaleVioletRed;
                    break;
                default:
                    color = Colors.Blue;
                    break;
            }
            return color;
        }

        /// <summary>
        /// Převod <see cref="RadioDataPartState"/> na <see cref="Brush"/>
        /// </summary>
        public Brush ToBrush(RadioDataPartState radioDataPartState)
        {
            return new SolidColorBrush(ToColor(radioDataPartState));
        }
    }
}
