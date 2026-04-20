namespace StudyCallAIApi.Components.Func
{
    public class ChatMsg
    {
        public string Text { get; }
        public bool IsUser { get; }
        public bool IsThinking { get; }

        public ChatMsg(string str, bool isUser, bool isThinking = false)
        {
            Text = str;
            this.IsUser = isUser;
            this.IsThinking = isThinking;
        }
    }
}
