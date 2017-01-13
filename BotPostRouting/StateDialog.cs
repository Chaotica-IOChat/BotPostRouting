namespace BotPostRouting
{
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class StateDialog : IDialog<object>
    {
        private const string HelpMessage = "\n * Type /lessons to get started.";

        internal FauxConversationController Convo;
        internal PointInConversation currentConvoPoint;

        internal bool convoStarted = false;
        internal bool InConvoMode = false;

        public async Task StartAsync(IDialogContext context)
        {

            if(!convoStarted)
            {
                convoStarted = true;
            }

            await context.PostAsync($"Welcome to Chaotica - IOChat");
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            string userName;

            if (!context.UserData.TryGetValue(ContextConstants.UserNameKey, out userName))
            {
                PromptDialog.Text(context, this.ResumeAfterPrompt, "Before we get started, please tell me your name?");
                return;
            }

            else if(!InConvoMode)
            {

                Convo = new FauxConversationController();

                currentConvoPoint = Convo.ConversationPoints[0];
                InConvoMode = true;
            }
            
            
            else
            {
                //Keep track of routing successful
                bool isRouted = false;
                foreach (ConversationRoute route in currentConvoPoint.Routes)
                {
                    //Perform route
                    Router.Route(route, message.Text.ToLower(), ConversationRouteType.SEARCH_MATCH, true, async () =>
                    {
                        //Routed = true
                        isRouted = true;

                        //Set point in conversation = route PointInConversation
                        currentConvoPoint = route.PIC;

                        //Post route response
                        await context.PostAsync($"{userName}, {route.PIC.ResponseText}");

                        return true;
                    });
                }

                //Routing failed / not found
                if (!isRouted)
                {
                    if (currentConvoPoint.BadRouteConvoPoint != null)
                    {
                        await context.PostAsync($"{currentConvoPoint.BadRouteConvoPoint.ResponseText}");
                        currentConvoPoint = currentConvoPoint.BadRouteConvoPoint;
                    }else
                    {
                        await context.PostAsync($"{userName} - I am sorry, but I did not understand.");
                    }
                    
                }
            }

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ResumeAfterPrompt(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var userName = await result;

                await context.PostAsync($"Welcome {userName}! {HelpMessage}");

                context.UserData.SetValue(ContextConstants.UserNameKey, userName);
            }
            catch (TooManyAttemptsException)
            {
            }

            context.Wait(this.MessageReceivedAsync);
        }
    }
}
