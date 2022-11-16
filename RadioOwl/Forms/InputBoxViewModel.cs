using Caliburn.Micro;
using System;

namespace RadioOwl.Forms
{
    /// <summary>
    /// Pprimitivni input box s jednim vstupem pres caliburn
    /// </summary>
    class InputBoxViewModel : Screen 
    {
        private string _question;
        /// <summary>
        /// Text dotazu
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
        /// Text odpovedi
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
        /// Titulek okna
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
        /// Zda form skoncil OK buttonem
        /// </summary>
        public bool IsOkPressed { get; private set; }

        /// <summary>
        /// Akce pod ButtonOk
        /// </summary>
        public void ButtonOk()
        {
            throw new NotImplementedException("TODO Po prechodu na .net fw nezna TryCloseAsync?");

            //IsOkPressed = true;
            //_ = TryCloseAsync(true);
        }

        /// <summary>
        /// Ponstruktor - protected jelikoz chci instancovat tridu jen z mistni staticke Execute fce
        /// </summary>
        protected InputBoxViewModel(string title, string question,  string answer = null)
        {
            Title = title;
            Question = question;
            Answer = answer;
        }
        
        /// <summary>
        /// Zridi form a hned vraci string vysledek
        /// </summary>
        public static string ExecuteModal(string title, string question, string answer = null)
        {
            throw new NotImplementedException("TODO Po prechodu na .net fw nezna ShowDialogAsync?");
            //var inputBoxViewModel = new InputBoxViewModel(title, question, answer);
            //new WindowManager().ShowDialogAsync(inputBoxViewModel);
            //return inputBoxViewModel.IsOkPressed ? inputBoxViewModel.Answer : null;
        }
    }
}
