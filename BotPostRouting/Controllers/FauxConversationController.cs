using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotPostRouting
{

    public static class Router
    {
        public static void Route(ConversationRoute route, String _GET, ConversationRouteType routeType, bool wildcard_allowed = true, Func<Task<bool>> func = null)
        {
            route.Route(_GET, routeType, wildcard_allowed, func);
        }
    }

    [Serializable()]
    public class ConversationUnroutedTarget
    {
        public String ResponseText = "";
    }

    public enum ConversationRouteType
    {
        DEFAULT=1,
        EXACT_MATCH=2,
        SEARCH_MATCH=3
    }

    [Serializable()]
    public class ConversationRoute
    {

        private  int _ID = 0;
        public int ID
        {
            get
            {
                return _ID;
            }
        }
        
        public ConversationRouteType OverrideConversationRouteType = ConversationRouteType.DEFAULT;

        public async void Route(String _GET, ConversationRouteType routeType, bool wildcard_allowed = true, Func<Task<bool>> func = null)
        {
            String[] _TargetTextArray = PIC.TargetText.Split(';');

            foreach (String _GETValue in _TargetTextArray)
            {
                if(OverrideConversationRouteType != ConversationRouteType.DEFAULT)
                {
                    if (OverrideConversationRouteType == ConversationRouteType.EXACT_MATCH)
                    {
                        if (_GET.ToLower() == _GETValue.ToLower() || wildcard_allowed == true && PIC.TargetText == "*")
                        {
                            if (func != null)
                                await func();
                        }
                    }
                    else
                    {
                        if (_GET.ToLower().Contains(_GETValue.ToLower()) || wildcard_allowed == true && PIC.TargetText == "*")
                        {
                            if (func != null)
                                await func();
                        }
                    }
                }
                else
                {
                    if(routeType == ConversationRouteType.EXACT_MATCH)
                    {
                        if (_GET.ToLower() == _GETValue.ToLower() || wildcard_allowed == true && PIC.TargetText == "*")
                        {
                            if (func != null)
                                await func();
                        }
                    }
                    else
                    {
                        if (_GET.ToLower().Contains(_GETValue.ToLower()) || wildcard_allowed == true && PIC.TargetText == "*")
                        {
                            if (func != null)
                                await func();
                        }
                    }
                }
            }
        }

        private PointInConversation _PIC;
        public PointInConversation PIC
        {
            get
            {
                return _PIC;
            }

            set
            {
                _PIC = value;
                _ID = _PIC.ID;
            }
        }

        public ConversationRoute(PointInConversation P, ConversationRouteType R = ConversationRouteType.DEFAULT)
        {
            this.PIC = P;
            this.OverrideConversationRouteType = R;
        }
    }

    [Serializable()]
    public class PointInConversation
    {
        public int ID = 0;
        public String TargetText = "";
        public String ResponseText = "";
        
        public ObservableCollection<ConversationRoute> Routes = new ObservableCollection<ConversationRoute>();
        public PointInConversation RouteForwarder = null, BadRouteConvoPoint = null;

        internal void BindRoute(ConversationRoute route)
        {
            Routes.Add(route);
        }
        internal void BindRoutes(ConversationRoute[] routes)
        {
            foreach(ConversationRoute route in routes)
            {
                Routes.Add(route);
            }
        }
        internal void UnbindRoute(ConversationRoute route)
        {
            Routes.Remove(route);
        }

        public PointInConversation(int _ID, String _TargetText, String _ResponseText)
        {
            this.ID = _ID;
            this.TargetText = _TargetText;
            this.ResponseText = _ResponseText;
        }
    }

    [Serializable()]
    public class FauxConversationController 
    {
        public ObservableCollection<PointInConversation> ConversationPoints = new ObservableCollection<PointInConversation>();
        
        public FauxConversationController()
        {
            
            PointInConversation ConvoEntryHello_PIC = new PointInConversation(1, "Hi;Hello", "Hey!");

            PointInConversation ConvoEntryAgeRequest_PIC = new PointInConversation(2, "how old", "I am 24 years old. How old are you?");

            PointInConversation ConvoEntryAgeReceived_PIC = new PointInConversation(3, "*", "Thanks for sharing!");


            // - - - - - Route1: Said Hi or Hello to the bot - - - - - \\
            ConversationRoute ConvoEntryHelloRoute = new ConversationRoute(ConvoEntryHello_PIC, ConversationRouteType.EXACT_MATCH);

            // - - - - - Route2: Requested to know bot's age - - - - - \\
            ConversationRoute ConvoEntryAgeRequestRoute = new ConversationRoute(ConvoEntryAgeRequest_PIC, ConversationRouteType.SEARCH_MATCH);

            // - - - - - Route3: Responded with age - - - - - \\
            ConversationRoute ConvoEntryAgeReceivedRoute = new ConversationRoute(ConvoEntryAgeReceived_PIC, ConversationRouteType.SEARCH_MATCH);

            // - - - - - Bind conversation routes - - - - - \\
            ConversationRoute[] routes = { ConvoEntryHelloRoute, ConvoEntryAgeRequestRoute };
            ConvoEntryHello_PIC.BindRoutes(routes);

            ConvoEntryAgeRequest_PIC.BindRoute(ConvoEntryAgeReceivedRoute);
            
            ConvoEntryAgeReceived_PIC.BindRoutes(routes);

            // - - - - - Add conversation points - - - - - \\
            ConversationPoints.Add(ConvoEntryHello_PIC);
            ConversationPoints.Add(ConvoEntryAgeRequest_PIC);
            ConversationPoints.Add(ConvoEntryAgeReceived_PIC);
        }
    }
}