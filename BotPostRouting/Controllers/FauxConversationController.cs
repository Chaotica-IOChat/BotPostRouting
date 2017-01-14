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
                        if (/*_GET.ToLower().Contains(_GETValue.ToLower())*/ FauxConversationController.StringUtils.STR_SearchMatch(_GET.ToLower(), _GETValue.ToLower()) || wildcard_allowed == true && PIC.TargetText == "*")
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
                        if (/*_GET.ToLower().Contains(_GETValue.ToLower())*/ FauxConversationController.StringUtils.STR_SearchMatch(_GET.ToLower(), _GETValue.ToLower()) || wildcard_allowed == true && PIC.TargetText == "*")
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

        public static class StringUtils
        {
            public static bool STR_SearchMatch(String string1, String string2)
            {
                bool match = false;

                String[] splits = string2.Split(new string[] { "%s" }, StringSplitOptions.None);

                String tmpString = string1;
                foreach(String s in splits)
                {
                    int x = tmpString.IndexOf(s);

                    if(x>= 0)
                    {
                        tmpString = tmpString.Substring(x+s.Length, tmpString.Length-(x + s.Length));
                        match = true;
                    }else
                    {
                        match = false;
                       break;
                    }
                }

                return match;
            }
        }

        public ObservableCollection<PointInConversation> ConversationPoints = new ObservableCollection<PointInConversation>();
        
        public FauxConversationController()
        {
            // - - - - - Conversation points - - - - - \\
            PointInConversation ConvoEntryHello_PIC = new PointInConversation(1, "Hi;Hello", "Hey!");
            PointInConversation ConvoEntrySentiment_PIC = new PointInConversation(1, "how %s you %s feel;how are you", "I'm doing fine! Thanks for asking! :D");
            PointInConversation ConvoEntryAgeRequest_PIC = new PointInConversation(2, "how old;what %s your %s age", "I am 24 years old. How old are you?");
            PointInConversation ConvoEntryAgeReceived_PIC = new PointInConversation(3, "*", "Thanks for sharing!");


            // - - - - - Route1: Said Hi or Hello to the bot - - - - - \\
            ConversationRoute ConvoEntryHelloRoute = new ConversationRoute(ConvoEntryHello_PIC, ConversationRouteType.EXACT_MATCH);

            // - - - - - Route1: Asked the bot about it's state of sentiment - - - - - \\
            ConversationRoute ConvoEntrySentimentRoute = new ConversationRoute(ConvoEntrySentiment_PIC, ConversationRouteType.SEARCH_MATCH);

            // - - - - - Route2: Requested to know bot's age - - - - - \\
            ConversationRoute ConvoEntryAgeRequestRoute = new ConversationRoute(ConvoEntryAgeRequest_PIC, ConversationRouteType.SEARCH_MATCH);

            // - - - - - Route3: Responded with age - - - - - \\
            ConversationRoute ConvoEntryAgeReceivedRoute = new ConversationRoute(ConvoEntryAgeReceived_PIC, ConversationRouteType.SEARCH_MATCH);

            // - - - - - Bind conversation routes - - - - - \\
            ConversationRoute[] routes = { ConvoEntryHelloRoute, ConvoEntryAgeRequestRoute, ConvoEntrySentimentRoute };
            ConvoEntryHello_PIC.BindRoutes(routes);
            ConvoEntrySentiment_PIC.BindRoutes(routes);
            ConvoEntryAgeRequest_PIC.BindRoute(ConvoEntryAgeReceivedRoute);
            ConvoEntryAgeReceived_PIC.BindRoutes(routes);

            // - - - - - Add conversation points - - - - - \\
            ConversationPoints.Add(ConvoEntryHello_PIC);
            ConversationPoints.Add(ConvoEntryAgeRequest_PIC);
            ConversationPoints.Add(ConvoEntryAgeReceived_PIC);
            ConversationPoints.Add(ConvoEntrySentiment_PIC);
        }
    }
}