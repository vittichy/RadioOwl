using Caliburn.Micro;

namespace RadioOwl.ViewModels
{
    /// <summary>
    /// primitivni input box s jednim vstupem pres caliburn
    /// </summary>
    class InputBoxViewModel : Screen 
    {
        private string _question;
        /// <summary>
        /// text dotazu
        /// </summary>
        public string Question
        {
            get { return _question; }
            set
            {
                _question = value;
                NotifyOfPropertyChange(() => Question);
            }
        }


        private string _answer;
        /// <summary>
        /// text odpovedi
        /// </summary>
        public string Answer
        {
            get { return _answer; }
            set
            {
                _answer = value;
                NotifyOfPropertyChange(() => Answer);
            }
        }


        private string _title;
        /// <summary>
        /// titulek okna
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }
      

        /// <summary>
        /// zda form skoncil OK buttonem
        /// </summary>
        public bool IsOkPressed { get; private set; }


        /// <summary>
        /// akce pod ButtonOk
        /// </summary>
        public void ButtonOk()
        {
            IsOkPressed = true;
            TryClose(true);
        }

        
        /// <summary>
        /// konstruktor - protected jelikoz chci instancovat tridu jen z mistni staticke Execute fce
        /// </summary>
        protected InputBoxViewModel(string title, string question,  string answer = null)
        {
            Title = title;
            Question = question;
            Answer = answer;
        }

        
        /// <summary>
        /// zridi form a hned vraci string vysledek
        /// </summary>
        public static string ExecuteModal(string title, string question, string answer = null)
        {
            var inputBoxViewModel = new InputBoxViewModel(title, question, answer);
            new WindowManager().ShowDialog(inputBoxViewModel);
            return inputBoxViewModel.IsOkPressed ? inputBoxViewModel.Answer : null;
        }

    }
}
