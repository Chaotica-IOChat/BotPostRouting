using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotPostRouting
{
    [Serializable()]
    public class ConversationUnroutedTarget
    {
        public String ResponseText = "";
    }

    public enum ConversationRouteType
    {
        EXACT_MATCH,
        SEARCH_MATCH
    }

    [Serializable()]
    public class ConversationRoute
    {
        public int ID = 0;
        public String TargetText = "";

        public async void Route(String _GET, ConversationRouteType routeType, bool wildcard_allowed = true, Func<Task<bool>> func = null)
        {
            if (!wildcard_allowed)
            {
                if (_GET.ToLower().Contains(PIC.TargetText.ToLower()))
                {
                    if(func != null)
                        await func();
                }
            }

            else
            {
                if (_GET.ToLower().Contains(PIC.TargetText.ToLower()) || PIC.TargetText == "*")
                {
                    if (func != null)
                        await func();
                }
            }
        }

        public PointInConversation PIC;
    }

    [Serializable()]
    public class PointInConversation
    {
        public int ID = 0;
        public String TargetText = "";
        public String ResponseText = "";
        
        public ObservableCollection<ConversationRoute> Routes = new ObservableCollection<ConversationRoute>();
        public PointInConversation RouteForwarder = null, BadRouteConvoPoint = null;
    }

    [Serializable()]
    public class FauxConversationController 
    {
        public ObservableCollection<PointInConversation> ConversationPoints = new ObservableCollection<PointInConversation>();
        
        public FauxConversationController()
        {

            // - - - - - Conversation points - - - - - \\

            PointInConversation PIC = new PointInConversation();
            PIC.ID = 1;
            PIC.TargetText = "/home"; //Wildcard
            PIC.ResponseText = "You are home!";
            
            PointInConversation PIC2 = new PointInConversation();
            PIC2.ID = 2;
            PIC2.TargetText = "/lessons"; //Wildcard
            PIC2.ResponseText = "You are viewing lessons";

            // - - - - - Route1 - - - - - \\

            ConversationRoute route = new ConversationRoute();
            route.ID = 1;
            route.PIC = PIC2;

            PIC.Routes.Add(route);

            // - - - - - Route2 - - - - - \\

            ConversationRoute route2 = new ConversationRoute();
            route2.ID = 2;
            route2.PIC = PIC;

            PIC2.Routes.Add(route2);
            
            // - - - - - Add conversation points - - - - - \\

            ConversationPoints.Add(PIC);
            ConversationPoints.Add(PIC2);
        }
    }
}